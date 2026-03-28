#!/usr/bin/env python3
"""
Player testing harness runner.

Orchestrates a full testing round: loads agent profiles, applies the evaluation
rubric via each agent persona, collects DevTools evidence, and produces a
structured report.

Usage:
    python testing/harness/harness_runner.py [--commit HASH] [--date DATE] [--dry-run]
"""

import argparse
import json
import re
import subprocess
import sys
from dataclasses import dataclass, field, asdict
from datetime import datetime
from pathlib import Path
from typing import Optional


# ---------------------------------------------------------------------------
# Data classes
# ---------------------------------------------------------------------------

@dataclass
class AgentProfile:
    name: str
    persona: str
    goals: str
    skill_level: str
    evaluation_focus: str
    patience_level: str
    key_questions: str


@dataclass
class RubricCategory:
    name: str
    criteria: str


@dataclass
class AgentEvaluation:
    agent_name: str
    ratings: dict  # {category_name: {"score": int, "observations": str}}
    overall_rating: int
    top_strengths: list[str]
    top_issues: list[str]
    would_continue: str  # "Yes" / "No" / "Maybe"
    verdict: str


@dataclass
class RoundConfig:
    commit: str
    branch: str
    date: str
    godot_version: str = "4.6.1"
    renderer: str = "Mobile"


@dataclass
class RoundResult:
    config: RoundConfig
    evaluations: list[AgentEvaluation]
    evidence_dir: Optional[Path] = None


# ---------------------------------------------------------------------------
# Loaders
# ---------------------------------------------------------------------------

def load_agent_profiles(profiles_path: Path) -> list[AgentProfile]:
    """Parse agent profiles from the markdown file."""
    text = profiles_path.read_text(encoding="utf-8")
    agents = []

    # Split on ## Agent N: headings
    sections = re.split(r"^## Agent \d+:", text, flags=re.MULTILINE)

    for section in sections[1:]:  # skip preamble before first agent
        name = _extract_field(section, r"\*\*Name:\*\*\s*(.+)")
        persona = section.split("\n")[0].strip()  # first line after heading is persona title
        goals = _extract_field(section, r"\*\*Goals:\*\*\s*(.+)")
        skill_level = _extract_field(section, r"\*\*Skill level:\*\*\s*(.+)")
        evaluation_focus = _extract_field(section, r"\*\*Evaluation focus:\*\*\s*(.+)")
        patience_level = _extract_field(section, r"\*\*Patience level:\*\*\s*(.+)")
        key_questions = _extract_field(section, r"\*\*Key questions:\*\*\s*(.+)")

        agents.append(AgentProfile(
            name=name,
            persona=persona,
            goals=goals,
            skill_level=skill_level,
            evaluation_focus=evaluation_focus,
            patience_level=patience_level,
            key_questions=key_questions,
        ))

    return agents


def _extract_field(text: str, pattern: str) -> str:
    """Extract a single field value from markdown text."""
    match = re.search(pattern, text)
    return match.group(1).strip() if match else ""


def load_rubric_categories(rubric_path: Path) -> list[RubricCategory]:
    """Parse evaluation categories from the rubric markdown file."""
    text = rubric_path.read_text(encoding="utf-8")
    categories = []

    # Split on ### N. headings (numbered category sections)
    sections = re.split(r"^### \d+\.\s+", text, flags=re.MULTILINE)

    for section in sections[1:]:  # skip preamble
        lines = section.strip().split("\n")
        name = lines[0].strip()

        # Collect criteria bullet points
        criteria_lines = []
        for line in lines[1:]:
            line = line.strip()
            if line.startswith("- **") and ":**" in line:
                criteria_lines.append(line.lstrip("- ").strip())
            elif line.startswith("**Rating:**"):
                break

        criteria = "; ".join(criteria_lines) if criteria_lines else name

        categories.append(RubricCategory(name=name, criteria=criteria))

    return categories


# ---------------------------------------------------------------------------
# Prompt building
# ---------------------------------------------------------------------------

def build_agent_prompt(
    profile: AgentProfile,
    categories: list[RubricCategory],
    build_info: dict,
) -> str:
    """Build the evaluation prompt for a single agent."""

    category_block = "\n".join(
        f"- **{c.name}**: {c.criteria}" for c in categories
    )

    return f"""You are {profile.name}, a {profile.persona}.

## Your Profile
- **Goals:** {profile.goals}
- **Skill level:** {profile.skill_level}
- **Evaluation focus:** {profile.evaluation_focus}
- **Patience level:** {profile.patience_level}
- **Key questions:** {profile.key_questions}

## Build Under Test
- Commit: {build_info.get('commit', 'unknown')}
- Branch: {build_info.get('branch', 'unknown')}

## Instructions
You are evaluating a mobile game called "Cows & Graveyards" — a visual aid for the classic cow-counting road trip car game. Players tap to count cows on their side of the road; graveyards reset a player's score.

Evaluate the game from your persona's perspective. For each category below, provide a score (1-5) and observations. Then provide an overall summary.

## Evaluation Categories
{category_block}

## Response Format
Respond with valid JSON only, using this exact structure:
{{
  "ratings": {{
    "Category Name": {{"score": <1-5>, "observations": "<your observations>"}},
    ...
  }},
  "overall_rating": <1-5>,
  "top_strengths": ["<strength 1>", "<strength 2>", "<strength 3>"],
  "top_issues": ["<issue 1>", "<issue 2>", "<issue 3>"],
  "would_continue": "Yes|No|Maybe",
  "verdict": "<one-sentence summary>"
}}
"""


# ---------------------------------------------------------------------------
# Response parsing
# ---------------------------------------------------------------------------

def parse_agent_response(response_text: str, agent_name: str) -> AgentEvaluation:
    """Parse a JSON agent evaluation response into an AgentEvaluation."""
    # Try to extract JSON from the response (may be wrapped in markdown fences)
    json_text = response_text.strip()
    if "```" in json_text:
        match = re.search(r"```(?:json)?\s*\n(.*?)\n```", json_text, re.DOTALL)
        if match:
            json_text = match.group(1)

    try:
        data = json.loads(json_text)
    except json.JSONDecodeError as e:
        raise ValueError(f"Invalid JSON in agent response for {agent_name}: {e}")

    required = ["ratings", "overall_rating", "top_strengths", "top_issues", "would_continue", "verdict"]
    missing = [f for f in required if f not in data]
    if missing:
        raise ValueError(f"Missing required fields in response for {agent_name}: {missing}")

    return AgentEvaluation(
        agent_name=agent_name,
        ratings=data["ratings"],
        overall_rating=data["overall_rating"],
        top_strengths=data["top_strengths"],
        top_issues=data["top_issues"],
        would_continue=data["would_continue"],
        verdict=data["verdict"],
    )


# ---------------------------------------------------------------------------
# Report generation
# ---------------------------------------------------------------------------

def generate_round_report(
    evaluations: list[AgentEvaluation],
    config: RoundConfig,
) -> str:
    """Generate a full round report from agent evaluations."""
    lines = []
    lines.append("# Testing Round Report")
    lines.append("")
    lines.append(f"**Date:** {config.date}")
    lines.append(f"**Build:** {config.commit} (`{config.branch}`)")
    lines.append(f"**Engine:** Godot {config.godot_version}")
    lines.append(f"**Renderer:** {config.renderer}")
    lines.append("")
    lines.append("---")
    lines.append("")

    # Per-agent feedback
    lines.append("## Per-Agent Feedback")
    lines.append("")

    for ev in evaluations:
        lines.append(f"### {ev.agent_name}")
        lines.append("")
        lines.append("| Category | Rating | Key Observation |")
        lines.append("|----------|--------|-----------------|")
        for cat_name, rating in ev.ratings.items():
            score = rating.get("score", "N/A")
            obs = rating.get("observations", "")
            # Truncate long observations for the table
            obs_short = obs[:80] + "..." if len(obs) > 80 else obs
            lines.append(f"| {cat_name} | {score}/5 | {obs_short} |")
        lines.append(f"| **Overall** | **{ev.overall_rating}/5** | |")
        lines.append("")

        if ev.top_strengths:
            lines.append("**Top strengths:**")
            for s in ev.top_strengths:
                lines.append(f"1. {s}")
            lines.append("")

        if ev.top_issues:
            lines.append("**Top issues:**")
            for i in ev.top_issues:
                lines.append(f"1. {i}")
            lines.append("")

        lines.append(f"**Would continue playing?** {ev.would_continue}")
        lines.append(f"**Verdict:** {ev.verdict}")
        lines.append("")
        lines.append("---")
        lines.append("")

    # Score Matrix
    lines.append("## Score Matrix")
    lines.append("")

    all_categories = set()
    for ev in evaluations:
        all_categories.update(ev.ratings.keys())
    all_categories = sorted(all_categories)

    agent_names = [ev.agent_name for ev in evaluations]
    header = "| Category | " + " | ".join(agent_names) + " | **Avg** |"
    sep = "|" + "---|" * (len(agent_names) + 2)
    lines.append(header)
    lines.append(sep)

    for cat in all_categories:
        scores = []
        cells = []
        for ev in evaluations:
            s = ev.ratings.get(cat, {}).get("score")
            if s is not None:
                scores.append(s)
                cells.append(str(s))
            else:
                cells.append("N/A")
        avg = f"{sum(scores) / len(scores):.1f}" if scores else "N/A"
        lines.append(f"| {cat} | " + " | ".join(cells) + f" | **{avg}** |")

    # Overall row
    overall_scores = [ev.overall_rating for ev in evaluations]
    overall_cells = [str(s) for s in overall_scores]
    overall_avg = f"{sum(overall_scores) / len(overall_scores):.1f}" if overall_scores else "N/A"
    lines.append(f"| **Overall** | " + " | ".join(overall_cells) + f" | **{overall_avg}** |")
    lines.append("")

    # Categorized Findings
    lines.append("## Categorized Findings")
    lines.append("")

    # Collect all issues across agents
    all_issues = []
    for ev in evaluations:
        for issue in ev.top_issues:
            all_issues.append({"text": issue, "agent": ev.agent_name})

    # Group by severity heuristic: issues from agents with low overall scores are higher severity
    for severity in ["Critical", "High", "Medium", "Low"]:
        lines.append(f"### {severity}")
        lines.append("")
        lines.append("| # | Finding | Agents | Recommendation |")
        lines.append("|---|---------|--------|----------------|")
        # Placeholder — actual categorization happens in Phase 4 analysis
        lines.append("| | _(to be categorized during analysis)_ | | |")
        lines.append("")

    # Recommendations placeholder
    lines.append("## Recommendations")
    lines.append("")
    lines.append("_(To be completed during analysis phase)_")
    lines.append("")

    lines.append("---")
    lines.append("")
    lines.append("_Generated by player testing harness._")

    return "\n".join(lines)


# ---------------------------------------------------------------------------
# Output directory
# ---------------------------------------------------------------------------

def create_round_directory(base_dir: Path, date: str) -> Path:
    """Create a timestamped round directory with subdirectories for evidence."""
    round_dir = base_dir / date

    # Handle duplicates by appending a counter
    if round_dir.exists():
        counter = 2
        while True:
            candidate = base_dir / f"{date}_{counter}"
            if not candidate.exists():
                round_dir = candidate
                break
            counter += 1

    round_dir.mkdir(parents=True, exist_ok=True)
    (round_dir / "screenshots").mkdir()
    (round_dir / "scene_trees").mkdir()
    (round_dir / "performance").mkdir()
    (round_dir / "evaluations").mkdir()

    return round_dir


# ---------------------------------------------------------------------------
# DevTools integration
# ---------------------------------------------------------------------------

def collect_devtools_evidence(project_path: Path, round_dir: Path) -> dict:
    """Collect screenshots, scene trees, and performance data via DevTools CLI."""
    devtools = project_path / "tools" / "devtools.py"
    evidence = {"screenshot": None, "scene_tree": None, "performance": None}

    try:
        # Screenshot
        result = subprocess.run(
            [sys.executable, str(devtools), "-p", str(project_path), "screenshot"],
            capture_output=True, text=True, timeout=15,
        )
        if result.returncode == 0:
            evidence["screenshot"] = result.stdout.strip()

        # Scene tree
        result = subprocess.run(
            [sys.executable, str(devtools), "-p", str(project_path), "scene-tree"],
            capture_output=True, text=True, timeout=15,
        )
        if result.returncode == 0:
            tree_path = round_dir / "scene_trees" / "scene_tree.json"
            tree_path.write_text(result.stdout, encoding="utf-8")
            evidence["scene_tree"] = str(tree_path)

        # Performance
        result = subprocess.run(
            [sys.executable, str(devtools), "-p", str(project_path), "performance"],
            capture_output=True, text=True, timeout=15,
        )
        if result.returncode == 0:
            perf_path = round_dir / "performance" / "metrics.txt"
            perf_path.write_text(result.stdout, encoding="utf-8")
            evidence["performance"] = str(perf_path)

    except (subprocess.TimeoutExpired, FileNotFoundError) as e:
        print(f"Warning: DevTools evidence collection failed: {e}", file=sys.stderr)

    return evidence


# ---------------------------------------------------------------------------
# CLI
# ---------------------------------------------------------------------------

def main():
    parser = argparse.ArgumentParser(description="Player testing harness runner")
    parser.add_argument("--commit", help="Build commit hash", default="HEAD")
    parser.add_argument("--branch", help="Build branch", default="master")
    parser.add_argument("--date", help="Round date (YYYY-MM-DD)", default=datetime.now().strftime("%Y-%m-%d"))
    parser.add_argument("--dry-run", action="store_true", help="Validate setup without executing evaluations")
    parser.add_argument("--project", "-p", help="Path to Godot project", default=".")
    args = parser.parse_args()

    harness_dir = Path(__file__).parent
    project_path = Path(args.project).resolve()
    rounds_dir = harness_dir.parent / "rounds"

    # Load artifacts
    print("Loading agent profiles...")
    profiles = load_agent_profiles(harness_dir / "agent_profiles.md")
    print(f"  Loaded {len(profiles)} profiles")

    print("Loading rubric categories...")
    categories = load_rubric_categories(harness_dir / "evaluation_rubric.md")
    print(f"  Loaded {len(categories)} categories")

    # Resolve commit
    commit = args.commit
    if commit == "HEAD":
        try:
            result = subprocess.run(
                ["git", "rev-parse", "--short", "HEAD"],
                capture_output=True, text=True, cwd=str(project_path),
            )
            commit = result.stdout.strip() if result.returncode == 0 else "unknown"
        except FileNotFoundError:
            commit = "unknown"

    config = RoundConfig(commit=commit, branch=args.branch, date=args.date)

    # Create round directory
    round_dir = create_round_directory(rounds_dir, args.date)
    print(f"Round directory: {round_dir}")

    if args.dry_run:
        print("\n--- DRY RUN ---")
        print(f"Build: {config.commit} ({config.branch})")
        print(f"Agents: {', '.join(p.name for p in profiles)}")
        print(f"Categories: {', '.join(c.name for c in categories)}")
        print(f"Output: {round_dir}")

        # Write build metadata
        meta = {
            "commit": config.commit,
            "branch": config.branch,
            "date": config.date,
            "godot_version": config.godot_version,
            "renderer": config.renderer,
            "agents": [p.name for p in profiles],
            "categories": [c.name for c in categories],
            "dry_run": True,
        }
        (round_dir / "build_metadata.json").write_text(
            json.dumps(meta, indent=2), encoding="utf-8"
        )

        # Generate sample prompts
        for profile in profiles:
            prompt = build_agent_prompt(profile, categories, {"commit": config.commit, "branch": config.branch})
            prompt_path = round_dir / "evaluations" / f"{profile.name.lower()}_prompt.txt"
            prompt_path.write_text(prompt, encoding="utf-8")

        print(f"\nDry run complete. Prompts written to {round_dir / 'evaluations'}")
        print("To execute a full round, run without --dry-run.")
        return

    # Full execution would integrate with an LLM API here.
    # For now, print instructions for manual execution.
    print("\n--- FULL ROUND ---")
    print("Full automated execution requires LLM integration.")
    print("Use --dry-run to generate prompts for manual evaluation.")


if __name__ == "__main__":
    main()
