namespace CowsGraveyards.Tests.Game;

using GdUnit4;
using Godot;
using static GdUnit4.Assertions;

[TestSuite]
[RequireGodotRuntime]
public class GraveyardEntityVisualTest
{
    private static readonly PackedScene GraveyardScene =
        GD.Load<PackedScene>("res://scenes/game/GraveyardEntity.tscn");

    [TestCase]
    public void SceneLoadsSuccessfully()
    {
        AssertThat(GraveyardScene).IsNotNull();

        var graveyard = AutoFree(GraveyardScene.Instantiate<Node3D>())!;
        AssertThat(graveyard).IsNotNull();
    }

    [TestCase]
    public void HasHeadstoneChild()
    {
        var graveyard = AutoFree(GraveyardScene.Instantiate<Node3D>())!;

        var headstone = graveyard.GetNodeOrNull<Node3D>("Headstone");
        AssertThat(headstone).IsNotNull();
    }

    [TestCase]
    public void HasBaseSlabChild()
    {
        var graveyard = AutoFree(GraveyardScene.Instantiate<Node3D>())!;

        var baseSlab = graveyard.GetNodeOrNull<Node3D>("Headstone/BaseSlab");
        AssertThat(baseSlab).IsNotNull();
    }

    [TestCase]
    public void HasArchTopChild()
    {
        var graveyard = AutoFree(GraveyardScene.Instantiate<Node3D>())!;

        var archTop = graveyard.GetNodeOrNull<Node3D>("Headstone/ArchTop");
        AssertThat(archTop).IsNotNull();
    }

    [TestCase]
    public void HasCrossChild()
    {
        var graveyard = AutoFree(GraveyardScene.Instantiate<Node3D>())!;

        var cross = graveyard.GetNodeOrNull<Node3D>("Headstone/Cross");
        AssertThat(cross).IsNotNull();
    }

    [TestCase]
    public void HasGraveyardEntityScript()
    {
        var graveyard = AutoFree(GraveyardScene.Instantiate())!;

        AssertThat(graveyard is CowsGraveyards.Game.GraveyardEntity).IsTrue();
    }

    [TestCase]
    public void HeadstoneUsesShaderMaterial()
    {
        var graveyard = AutoFree(GraveyardScene.Instantiate<Node3D>())!;

        var headstone = graveyard.GetNodeOrNull<MeshInstance3D>("Headstone");
        AssertThat(headstone).IsNotNull();

        var mesh = headstone!.Mesh;
        AssertThat(mesh).IsNotNull();

        var material = mesh!.SurfaceGetMaterial(0);
        AssertThat(material is ShaderMaterial).IsTrue();
    }

    [TestCase]
    public void HeadstoneContainsMesh()
    {
        var graveyard = AutoFree(GraveyardScene.Instantiate<Node3D>())!;

        var headstone = graveyard.GetNodeOrNull<Node3D>("Headstone");
        AssertThat(headstone).IsNotNull();

        bool hasMesh = headstone is MeshInstance3D or CsgShape3D;
        if (!hasMesh)
        {
            foreach (var child in headstone!.GetChildren())
            {
                if (child is MeshInstance3D or CsgShape3D)
                {
                    hasMesh = true;
                    break;
                }
            }
        }

        AssertThat(hasMesh).IsTrue();
    }
}
