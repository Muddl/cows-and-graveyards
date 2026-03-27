namespace CowsGraveyards.Tests.Game;

using GdUnit4;
using Godot;
using static GdUnit4.Assertions;

[TestSuite]
[RequireGodotRuntime]
public class VisualEffectsTest
{
    private static readonly PackedScene GameScenePacked =
        GD.Load<PackedScene>("res://scenes/game/GameScene.tscn");

    private static readonly PackedScene CowScene =
        GD.Load<PackedScene>("res://scenes/game/CowEntity.tscn");

    [TestCase]
    public void HasRoadDustParticlesLeft()
    {
        var scene = AutoFree(GameScenePacked.Instantiate<Node3D>())!;

        var particles = scene.GetNodeOrNull<GpuParticles3D>("DustParticlesLeft");
        AssertThat(particles).IsNotNull();
        AssertThat(particles!.Emitting).IsTrue();
    }

    [TestCase]
    public void HasRoadDustParticlesRight()
    {
        var scene = AutoFree(GameScenePacked.Instantiate<Node3D>())!;

        var particles = scene.GetNodeOrNull<GpuParticles3D>("DustParticlesRight");
        AssertThat(particles).IsNotNull();
        AssertThat(particles!.Emitting).IsTrue();
    }

    [TestCase]
    public void CowHasIdleAnimationPlayer()
    {
        var cow = AutoFree(CowScene.Instantiate<Node3D>())!;

        var animPlayer = cow.GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
        AssertThat(animPlayer).IsNotNull();
    }

    [TestCase]
    public void CowIdleAnimationExists()
    {
        var cow = AutoFree(CowScene.Instantiate<Node3D>())!;

        var animPlayer = cow.GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
        AssertThat(animPlayer).IsNotNull();
        AssertThat(animPlayer!.HasAnimation("idle")).IsTrue();
    }

    [TestCase]
    public void HasSpeedLinesEffect()
    {
        var scene = AutoFree(GameScenePacked.Instantiate<Node3D>())!;

        var speedLines = scene.GetNodeOrNull<GpuParticles3D>("SpeedLines");
        AssertThat(speedLines).IsNotNull();
    }
}
