#!/usr/bin/env python3
"""Tests for the player testing harness runner."""

import json
import os
import tempfile
import unittest
from pathlib import Path
from unittest.mock import patch, MagicMock

# Will be implemented in harness_runner.py
from harness_runner import (
    load_agent_profiles,
    load_rubric_categories,
    AgentProfile,
    RubricCategory,
    AgentEvaluation,
    RoundConfig,
    RoundResult,
    build_agent_prompt,
    parse_agent_response,
    generate_round_report,
    create_round_directory,
)


class TestLoadAgentProfiles(unittest.TestCase):
    """Test that agent profiles are loaded from the markdown file."""

    def test_loads_all_profiles(self):
        profiles = load_agent_profiles(HARNESS_DIR / "agent_profiles.md")
        self.assertEqual(len(profiles), 7)

    def test_profile_has_required_fields(self):
        profiles = load_agent_profiles(HARNESS_DIR / "agent_profiles.md")
        for p in profiles:
            self.assertIsInstance(p, AgentProfile)
            self.assertTrue(p.name, f"Profile missing name")
            self.assertTrue(p.persona, f"Profile {p.name} missing persona")
            self.assertTrue(p.goals, f"Profile {p.name} missing goals")
            self.assertTrue(p.skill_level, f"Profile {p.name} missing skill_level")
            self.assertTrue(p.evaluation_focus, f"Profile {p.name} missing evaluation_focus")
            self.assertTrue(p.patience_level, f"Profile {p.name} missing patience_level")
            self.assertTrue(p.key_questions, f"Profile {p.name} missing key_questions")

    def test_known_agent_names(self):
        profiles = load_agent_profiles(HARNESS_DIR / "agent_profiles.md")
        names = [p.name for p in profiles]
        self.assertIn("Casey", names)
        self.assertIn("Milo", names)
        self.assertIn("Avery", names)
        self.assertIn("Jordan", names)
        self.assertIn("Reese", names)
        self.assertIn("Dakota", names)
        self.assertIn("Morgan", names)


class TestLoadRubricCategories(unittest.TestCase):
    """Test that rubric categories are parsed from the markdown file."""

    def test_loads_all_categories(self):
        categories = load_rubric_categories(HARNESS_DIR / "evaluation_rubric.md")
        self.assertEqual(len(categories), 6)

    def test_category_has_required_fields(self):
        categories = load_rubric_categories(HARNESS_DIR / "evaluation_rubric.md")
        for c in categories:
            self.assertIsInstance(c, RubricCategory)
            self.assertTrue(c.name, "Category missing name")
            self.assertTrue(c.criteria, f"Category {c.name} missing criteria")

    def test_known_category_names(self):
        categories = load_rubric_categories(HARNESS_DIR / "evaluation_rubric.md")
        names = [c.name for c in categories]
        self.assertIn("Core Gameplay Loop", names)
        self.assertIn("UX / Usability", names)
        self.assertIn("Visual Quality", names)
        self.assertIn("Game Feel", names)
        self.assertIn("Visual Correctness", names)
        self.assertIn("Performance Perception", names)


class TestBuildAgentPrompt(unittest.TestCase):
    """Test that evaluation prompts are correctly assembled."""

    def test_prompt_contains_agent_persona(self):
        profile = AgentProfile(
            name="TestAgent",
            persona="Casual Mobile Gamer",
            goals="Have fun",
            skill_level="Moderate",
            evaluation_focus="Tap responsiveness",
            patience_level="Medium",
            key_questions="Is it fun?",
        )
        categories = [RubricCategory(name="Game Feel", criteria="Responsiveness, feedback")]
        prompt = build_agent_prompt(profile, categories, {"commit": "abc123"})
        self.assertIn("TestAgent", prompt)
        self.assertIn("Casual Mobile Gamer", prompt)
        self.assertIn("Have fun", prompt)

    def test_prompt_contains_rubric_categories(self):
        profile = AgentProfile(
            name="TestAgent",
            persona="Tester",
            goals="Test",
            skill_level="High",
            evaluation_focus="Everything",
            patience_level="High",
            key_questions="Does it work?",
        )
        categories = [
            RubricCategory(name="Core Gameplay Loop", criteria="Clarity, satisfaction"),
            RubricCategory(name="Visual Quality", criteria="Art style, readability"),
        ]
        prompt = build_agent_prompt(profile, categories, {"commit": "abc123"})
        self.assertIn("Core Gameplay Loop", prompt)
        self.assertIn("Visual Quality", prompt)

    def test_prompt_contains_build_info(self):
        profile = AgentProfile(
            name="TestAgent",
            persona="Tester",
            goals="Test",
            skill_level="High",
            evaluation_focus="Everything",
            patience_level="High",
            key_questions="Does it work?",
        )
        categories = []
        prompt = build_agent_prompt(profile, categories, {"commit": "abc123", "branch": "master"})
        self.assertIn("abc123", prompt)


class TestParseAgentResponse(unittest.TestCase):
    """Test that structured agent responses are parsed correctly."""

    def test_parses_valid_json_response(self):
        response = json.dumps({
            "ratings": {
                "Core Gameplay Loop": {"score": 4, "observations": "Fun loop"},
                "Visual Quality": {"score": 3, "observations": "Decent"},
            },
            "overall_rating": 4,
            "top_strengths": ["Fun", "Simple", "Colorful"],
            "top_issues": ["No tutorial", "Small targets"],
            "would_continue": "Yes",
            "verdict": "A fun road trip companion",
        })
        evaluation = parse_agent_response(response, "Casey")
        self.assertIsInstance(evaluation, AgentEvaluation)
        self.assertEqual(evaluation.agent_name, "Casey")
        self.assertEqual(evaluation.overall_rating, 4)
        self.assertEqual(len(evaluation.ratings), 2)
        self.assertEqual(evaluation.ratings["Core Gameplay Loop"]["score"], 4)

    def test_rejects_invalid_json(self):
        with self.assertRaises(ValueError):
            parse_agent_response("not json at all", "Casey")

    def test_rejects_missing_required_fields(self):
        response = json.dumps({"ratings": {}})
        with self.assertRaises(ValueError):
            parse_agent_response(response, "Casey")


class TestGenerateRoundReport(unittest.TestCase):
    """Test that a full round report is generated from evaluations."""

    def test_generates_report_with_all_agents(self):
        evaluations = [
            AgentEvaluation(
                agent_name="Casey",
                ratings={"Core Gameplay Loop": {"score": 4, "observations": "Good"}},
                overall_rating=4,
                top_strengths=["Fun"],
                top_issues=["Small targets"],
                would_continue="Yes",
                verdict="Good game",
            ),
            AgentEvaluation(
                agent_name="Milo",
                ratings={"Core Gameplay Loop": {"score": 3, "observations": "OK"}},
                overall_rating=3,
                top_strengths=["Colorful"],
                top_issues=["Confusing"],
                would_continue="Maybe",
                verdict="Needs work for kids",
            ),
        ]
        config = RoundConfig(commit="abc123", branch="master", date="2026-03-28")
        report = generate_round_report(evaluations, config)
        self.assertIsInstance(report, str)
        self.assertIn("Casey", report)
        self.assertIn("Milo", report)
        self.assertIn("abc123", report)

    def test_report_contains_score_matrix(self):
        evaluations = [
            AgentEvaluation(
                agent_name="Casey",
                ratings={
                    "Core Gameplay Loop": {"score": 4, "observations": "Good"},
                    "Visual Quality": {"score": 5, "observations": "Great"},
                },
                overall_rating=4,
                top_strengths=["Fun"],
                top_issues=[],
                would_continue="Yes",
                verdict="Good",
            ),
        ]
        config = RoundConfig(commit="abc123", branch="master", date="2026-03-28")
        report = generate_round_report(evaluations, config)
        self.assertIn("Score Matrix", report)

    def test_report_contains_categorized_findings(self):
        evaluations = [
            AgentEvaluation(
                agent_name="Casey",
                ratings={"Core Gameplay Loop": {"score": 2, "observations": "Scoring is broken"}},
                overall_rating=2,
                top_strengths=[],
                top_issues=["Scoring broken"],
                would_continue="No",
                verdict="Needs fixes",
            ),
        ]
        config = RoundConfig(commit="abc123", branch="master", date="2026-03-28")
        report = generate_round_report(evaluations, config)
        self.assertIn("Categorized Findings", report)


class TestCreateRoundDirectory(unittest.TestCase):
    """Test that round output directories are correctly created."""

    def test_creates_timestamped_directory(self):
        with tempfile.TemporaryDirectory() as tmpdir:
            round_dir = create_round_directory(Path(tmpdir), "2026-03-28")
            self.assertTrue(round_dir.exists())
            self.assertIn("2026-03-28", str(round_dir))

    def test_creates_subdirectories(self):
        with tempfile.TemporaryDirectory() as tmpdir:
            round_dir = create_round_directory(Path(tmpdir), "2026-03-28")
            self.assertTrue((round_dir / "screenshots").exists())
            self.assertTrue((round_dir / "scene_trees").exists())
            self.assertTrue((round_dir / "performance").exists())
            self.assertTrue((round_dir / "evaluations").exists())

    def test_handles_duplicate_dates(self):
        with tempfile.TemporaryDirectory() as tmpdir:
            dir1 = create_round_directory(Path(tmpdir), "2026-03-28")
            dir2 = create_round_directory(Path(tmpdir), "2026-03-28")
            self.assertNotEqual(dir1, dir2)
            self.assertTrue(dir2.exists())


# Resolve harness directory relative to this test file
HARNESS_DIR = Path(__file__).parent


if __name__ == "__main__":
    unittest.main()
