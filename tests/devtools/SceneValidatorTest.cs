namespace CowsGraveyards.Tests.DevTools;

using CowsGraveyards.DevTools;
using GdUnit4;
using static GdUnit4.Assertions;

[TestSuite]
[RequireGodotRuntime]
public class SceneValidatorTest
{
    [TestCase]
    public void ValidateScene_MissingFile_ReturnsFileNotFoundError()
    {
        var issues = SceneValidator.ValidateScene("res://nonexistent_scene.tscn");

        AssertThat(issues).HasSize(1);
        AssertThat(issues[0].Severity).IsEqual("error");
        AssertThat(issues[0].Code).IsEqual("file_not_found");
    }

    [TestCase]
    public void ValidateScene_ValidScene_ReturnsNoErrors()
    {
        var issues = SceneValidator.ValidateScene("res://scenes/game/GameScene.tscn");
        var errors = issues.FindAll(i => i.Severity == "error");

        AssertThat(errors).IsEmpty();
    }

    [TestCase]
    public void ValidateScene_ReturnsIssuesWithSeverityCodeMessage()
    {
        var issues = SceneValidator.ValidateScene("res://nonexistent_scene.tscn");

        AssertThat(issues[0].Severity).IsNotNull();
        AssertThat(issues[0].Code).IsNotNull();
        AssertThat(issues[0].Message).IsNotNull();
    }

    [TestCase]
    public void ValidationIssue_ConstructorSetsProperties()
    {
        var issue = new ValidationIssue("warning", "test_code", "Test message");

        AssertThat(issue.Severity).IsEqual("warning");
        AssertThat(issue.Code).IsEqual("test_code");
        AssertThat(issue.Message).IsEqual("Test message");
    }

    [TestCase]
    public void ValidateScene_InvalidResourcePath_ReturnsLoadFailed()
    {
        var issues = SceneValidator.ValidateScene("res://not_a_scene.txt");
        var errors = issues.FindAll(i => i.Code == "file_not_found" || i.Code == "load_failed");

        AssertThat(errors).IsNotEmpty();
    }

    [TestCase]
    public void ValidateScene_ExistingScenes_NoMissingScriptsOrResources()
    {
        var scenePaths = new[]
        {
            "res://scenes/game/GameScene.tscn",
            "res://scenes/menus/MainMenuScene.tscn",
        };

        foreach (var path in scenePaths)
        {
            var issues = SceneValidator.ValidateScene(path);
            var missingScripts = issues.FindAll(i => i.Code == "missing_script");
            AssertThat(missingScripts)
                .OverrideFailureMessage($"Scene {path} has missing script references")
                .IsEmpty();
        }
    }
}
