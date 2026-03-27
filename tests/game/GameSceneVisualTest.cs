namespace CowsGraveyards.Tests.Game;

using GdUnit4;
using Godot;
using static GdUnit4.Assertions;

[TestSuite]
[RequireGodotRuntime]
public class GameSceneVisualTest
{
    private static readonly PackedScene GameScenePacked =
        GD.Load<PackedScene>("res://scenes/game/GameScene.tscn");

    [TestCase]
    public void CarHasBodyChild()
    {
        var scene = AutoFree(GameScenePacked.Instantiate<Node3D>())!;

        var body = scene.GetNodeOrNull<Node3D>("Car/Body");
        AssertThat(body).IsNotNull();
    }

    [TestCase]
    public void CarHasRoofChild()
    {
        var scene = AutoFree(GameScenePacked.Instantiate<Node3D>())!;

        var roof = scene.GetNodeOrNull<Node3D>("Car/Roof");
        AssertThat(roof).IsNotNull();
    }

    [TestCase]
    public void CarHasFourWheels()
    {
        var scene = AutoFree(GameScenePacked.Instantiate<Node3D>())!;

        var wFL = scene.GetNodeOrNull<Node3D>("Car/WheelFrontLeft");
        var wFR = scene.GetNodeOrNull<Node3D>("Car/WheelFrontRight");
        var wBL = scene.GetNodeOrNull<Node3D>("Car/WheelBackLeft");
        var wBR = scene.GetNodeOrNull<Node3D>("Car/WheelBackRight");

        AssertThat(wFL).IsNotNull();
        AssertThat(wFR).IsNotNull();
        AssertThat(wBL).IsNotNull();
        AssertThat(wBR).IsNotNull();
    }

    [TestCase]
    public void CarHasWindshield()
    {
        var scene = AutoFree(GameScenePacked.Instantiate<Node3D>())!;

        var windshield = scene.GetNodeOrNull<Node3D>("Car/Windshield");
        AssertThat(windshield).IsNotNull();
    }

    [TestCase]
    public void RoadHasLaneMarkings()
    {
        var scene = AutoFree(GameScenePacked.Instantiate<Node3D>())!;

        var laneMarkings = scene.GetNodeOrNull<Node3D>("LaneMarkings");
        AssertThat(laneMarkings).IsNotNull();
    }

    [TestCase]
    public void RoadNodeExists()
    {
        var scene = AutoFree(GameScenePacked.Instantiate<Node3D>())!;

        var road = scene.GetNodeOrNull<MeshInstance3D>("Road");
        AssertThat(road).IsNotNull();
    }

    [TestCase]
    public void GrassNodesExist()
    {
        var scene = AutoFree(GameScenePacked.Instantiate<Node3D>())!;

        var grassLeft = scene.GetNodeOrNull<MeshInstance3D>("GrassLeft");
        var grassRight = scene.GetNodeOrNull<MeshInstance3D>("GrassRight");
        AssertThat(grassLeft).IsNotNull();
        AssertThat(grassRight).IsNotNull();
    }

    [TestCase]
    public void GrassUsesShaderMaterial()
    {
        var scene = AutoFree(GameScenePacked.Instantiate<Node3D>())!;

        var grassLeft = scene.GetNodeOrNull<MeshInstance3D>("GrassLeft");
        AssertThat(grassLeft).IsNotNull();

        var mesh = grassLeft!.Mesh;
        AssertThat(mesh).IsNotNull();

        var material = mesh!.SurfaceGetMaterial(0);
        AssertThat(material is ShaderMaterial).IsTrue();
    }
}
