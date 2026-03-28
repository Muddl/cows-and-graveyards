# Player Testing Harness

Reusable harness for running player testing rounds against Cows & Graveyards builds. Uses diverse simulated player agent personas to evaluate gameplay, UX, visuals, performance, and game feel.

## Quick Start

```bash
# Dry run — validates setup and generates agent prompts without executing
python testing/harness/harness_runner.py --dry-run

# Full dry run with explicit build info
python testing/harness/harness_runner.py --dry-run --commit abc1234 --branch master --date 2026-04-15
```

## Directory Structure

```
testing/
  harness/
    agent_profiles.md       # 7 player personas (Casey, Milo, Avery, Jordan, Reese, Dakota, Morgan)
    evaluation_rubric.md    # 6-category rubric with 1-5 rating scale
    report_template.md      # Template for round reports
    harness_runner.py       # Orchestration script
    test_harness_runner.py  # Unit tests (18 tests)
  rounds/
    2026-03-28/             # First round output
      build_metadata.json   # Pinned build info
      evaluations/          # Per-agent JSON feedback
      screenshots/          # DevTools screenshots (when game is running)
      scene_trees/          # DevTools scene tree dumps
      performance/          # DevTools performance metrics
      report.md             # Final analysis report
```

## Running a New Round

### 1. Pin the build

Decide which commit to test. The harness auto-detects HEAD if not specified.

### 2. Run the harness (dry run)

```bash
python testing/harness/harness_runner.py --dry-run --date YYYY-MM-DD
```

This creates `testing/rounds/{date}/` with build metadata and agent evaluation prompts.

### 3. Execute agent evaluations

Currently, agent evaluations are executed by passing each prompt to an LLM agent that role-plays the persona and writes structured JSON feedback. Use Claude Code agents or similar.

Each agent receives:
- Their persona profile (from `agent_profiles.md`)
- The evaluation rubric categories (from `evaluation_rubric.md`)
- A description of the current game state
- Instructions to output structured JSON

Output files go to `testing/rounds/{date}/evaluations/{agent_name}.json`.

### 4. Collect DevTools evidence (optional)

If the game is running with the DevTools autoload:

```bash
python tools/devtools.py screenshot
python tools/devtools.py scene-tree
python tools/devtools.py performance
```

Store outputs in the round's `screenshots/`, `scene_trees/`, and `performance/` directories.

### 5. Analyze and report

Aggregate the JSON evaluations into a report using the report template. The report includes:
- Per-agent feedback tables
- Cross-agent score matrix
- Categorized findings (critical/high/medium/low)
- Recommended follow-up tracks

## Running Tests

```bash
cd testing/harness
python -m unittest test_harness_runner -v
```

## Agent Profiles

| Agent | Persona | Focus |
|-------|---------|-------|
| Casey | Casual Mobile Gamer | Onboarding, tap feel, visual appeal |
| Milo | Young Child (5-8) | Visual delight, large targets, immediate feedback |
| Avery | Accessibility-Focused | Contrast, target sizes, non-visual feedback |
| Jordan | First-Time Player | Discoverability, rules clarity, scoring |
| Reese | Impatient Commuter | Launch-to-gameplay speed, friction, performance |
| Dakota | Genre Enthusiast | Game feel, juice, polish, depth |
| Morgan | Backseat Parent | Setup simplicity, child-proofing, glanceability |

## Evaluation Categories

1. Core Gameplay Loop
2. UX / Usability
3. Visual Quality
4. Game Feel
5. Visual Correctness
6. Performance Perception

Each rated 1-5 with free-text observations.
