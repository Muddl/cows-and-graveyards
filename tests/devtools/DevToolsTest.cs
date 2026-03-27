namespace CowsGraveyards.Tests.DevTools;

using CowsGraveyards.DevTools;
using GdUnit4;
using Godot;
using static GdUnit4.Assertions;

[TestSuite]
[RequireGodotRuntime]
public class DevToolsTest
{
    private DevTools _devTools = null!;

    [BeforeTest]
    public void Setup()
    {
        _devTools = AutoFree(new DevTools())!;
    }

    // ── Ping ────────────────────────────────────────────────────────────────

    [TestCase]
    public void HandleCommand_Ping_ReturnsSuccess()
    {
        var result = _devTools.HandleCommand("ping", default);

        AssertThat(result.Success).IsTrue();
        AssertThat(result.Message).IsEqual("pong");
    }

    // ── Unknown command ─────────────────────────────────────────────────────

    [TestCase]
    public void HandleCommand_UnknownCommand_ReturnsFalse()
    {
        var result = _devTools.HandleCommand("nonexistent_command", default);

        AssertThat(result.Success).IsFalse();
        AssertThat(result.Message).Contains("Unknown command");
    }

    // ── Performance ─────────────────────────────────────────────────────────

    [TestCase]
    public void HandleCommand_Performance_ReturnsSuccessWithData()
    {
        var result = _devTools.HandleCommand("performance", default);

        AssertThat(result.Success).IsTrue();
        AssertThat(result.Data).IsNotNull();
    }

    // ── Validate scene ──────────────────────────────────────────────────────

    [TestCase]
    public void HandleCommand_ValidateScene_RequiresPathArg()
    {
        var result = _devTools.HandleCommand("validate_scene", default);

        AssertThat(result.Success).IsFalse();
        AssertThat(result.Message).Contains("path");
    }

    [TestCase]
    public void HandleCommand_ValidateScene_ValidSceneReturnsSuccess()
    {
        var json = System.Text.Json.JsonDocument.Parse("{\"path\":\"res://scenes/game/GameScene.tscn\"}");
        var result = _devTools.HandleCommand("validate_scene", json.RootElement);

        AssertThat(result.Success).IsTrue();
    }

    // ── Get state ───────────────────────────────────────────────────────────

    [TestCase]
    public void HandleCommand_GetState_NoCurrentScene_ReturnsFalse()
    {
        var result = _devTools.HandleCommand("get_state", default);

        AssertThat(result.Success).IsFalse();
        AssertThat(result.Message).Contains("No current scene");
    }

    // ── Set state ───────────────────────────────────────────────────────────

    [TestCase]
    public void HandleCommand_SetState_MissingArgs_ReturnsFalse()
    {
        var result = _devTools.HandleCommand("set_state", default);

        AssertThat(result.Success).IsFalse();
    }

    // ── Screenshot ──────────────────────────────────────────────────────────

    [TestCase]
    public void HandleCommand_Screenshot_IsRegistered()
    {
        // Screenshot needs a viewport, just verify it doesn't return "Unknown command"
        var result = _devTools.HandleCommand("screenshot", default);

        AssertThat(result.Message).IsNotEqual("Unknown command: screenshot");
    }

    // ── Scene tree ──────────────────────────────────────────────────────────

    [TestCase]
    public void HandleCommand_SceneTree_NoCurrentScene_ReturnsFalse()
    {
        var result = _devTools.HandleCommand("scene_tree", default);

        AssertThat(result.Success).IsFalse();
        AssertThat(result.Message).Contains("No current scene");
    }

    // ── Input commands registered ───────────────────────────────────────────

    [TestCase]
    public void HandleCommand_InputPress_RequiresActionArg()
    {
        var result = _devTools.HandleCommand("input_press", default);

        AssertThat(result.Success).IsFalse();
        AssertThat(result.Message).Contains("action");
    }

    [TestCase]
    public void HandleCommand_InputRelease_RequiresActionArg()
    {
        var result = _devTools.HandleCommand("input_release", default);

        AssertThat(result.Success).IsFalse();
        AssertThat(result.Message).Contains("action");
    }

    [TestCase]
    public void HandleCommand_InputTap_RequiresActionArg()
    {
        var result = _devTools.HandleCommand("input_tap", default);

        AssertThat(result.Success).IsFalse();
        AssertThat(result.Message).Contains("action");
    }

    [TestCase]
    public void HandleCommand_InputClear_ReturnsSuccess()
    {
        var result = _devTools.HandleCommand("input_clear", default);

        AssertThat(result.Success).IsTrue();
    }

    [TestCase]
    public void HandleCommand_InputActions_ReturnsSuccess()
    {
        var result = _devTools.HandleCommand("input_actions", default);

        AssertThat(result.Success).IsTrue();
    }
}
