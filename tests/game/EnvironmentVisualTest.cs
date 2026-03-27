namespace CowsGraveyards.Tests.Game;

using GdUnit4;
using Godot;
using static GdUnit4.Assertions;

[TestSuite]
[RequireGodotRuntime]
public class EnvironmentVisualTest
{
    private static readonly PackedScene GameScenePacked =
        GD.Load<PackedScene>("res://scenes/game/GameScene.tscn");

    [TestCase]
    public void SkyMaterialHasSunDisk()
    {
        var scene = AutoFree(GameScenePacked.Instantiate<Node3D>())!;

        var worldEnv = scene.GetNodeOrNull<WorldEnvironment>("WorldEnvironment");
        AssertThat(worldEnv).IsNotNull();

        var env = worldEnv!.Environment;
        AssertThat(env).IsNotNull();

        var sky = env!.Sky;
        AssertThat(sky).IsNotNull();

        var skyMat = sky!.SkyMaterial as ProceduralSkyMaterial;
        AssertThat(skyMat).IsNotNull();

        // Sun disk should be visible (sun_angle_max > 0 indicates sun disk)
        AssertThat(skyMat!.SunAngleMax).IsGreater(0f);
    }

    [TestCase]
    public void SkyHasEnergyMultiplier()
    {
        var scene = AutoFree(GameScenePacked.Instantiate<Node3D>())!;

        var worldEnv = scene.GetNodeOrNull<WorldEnvironment>("WorldEnvironment");
        var env = worldEnv!.Environment;
        var sky = env!.Sky;
        var skyMat = sky!.SkyMaterial as ProceduralSkyMaterial;

        // Energy should be boosted above default 1.0
        AssertThat(skyMat!.SkyEnergyMultiplier).IsGreaterEqual(1.0f);
    }

    [TestCase]
    public void DirectionalLightHasShadows()
    {
        var scene = AutoFree(GameScenePacked.Instantiate<Node3D>())!;

        var light = scene.GetNodeOrNull<DirectionalLight3D>("DirectionalLight3D");
        AssertThat(light).IsNotNull();
        AssertThat(light!.ShadowEnabled).IsTrue();
    }

    [TestCase]
    public void HasFillLight()
    {
        var scene = AutoFree(GameScenePacked.Instantiate<Node3D>())!;

        var fillLight = scene.GetNodeOrNull<Light3D>("FillLight");
        AssertThat(fillLight).IsNotNull();
    }

    [TestCase]
    public void HasBackgroundScenery()
    {
        var scene = AutoFree(GameScenePacked.Instantiate<Node3D>())!;

        var scenery = scene.GetNodeOrNull<Node3D>("BackgroundScenery");
        AssertThat(scenery).IsNotNull();
        AssertThat(scenery!.GetChildCount()).IsGreater(0);
    }
}
