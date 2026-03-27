namespace CowsGraveyards.Tests.Game;

using GdUnit4;
using Godot;
using static GdUnit4.Assertions;

[TestSuite]
[RequireGodotRuntime]
public class CowEntityVisualTest
{
    private static readonly PackedScene CowScene =
        GD.Load<PackedScene>("res://scenes/game/CowEntity.tscn");

    [TestCase]
    public void SceneLoadsSuccessfully()
    {
        AssertThat(CowScene).IsNotNull();

        var cow = AutoFree(CowScene.Instantiate<Node3D>())!;
        AssertThat(cow).IsNotNull();
    }

    [TestCase]
    public void HasBodyMeshChild()
    {
        var cow = AutoFree(CowScene.Instantiate<Node3D>())!;

        var body = cow.GetNodeOrNull<Node3D>("Body");
        AssertThat(body).IsNotNull();
    }

    [TestCase]
    public void HasHeadChild()
    {
        var cow = AutoFree(CowScene.Instantiate<Node3D>())!;

        var head = cow.GetNodeOrNull<Node3D>("Body/Head");
        AssertThat(head).IsNotNull();
    }

    [TestCase]
    public void HasLegNodes()
    {
        var cow = AutoFree(CowScene.Instantiate<Node3D>())!;

        var legFL = cow.GetNodeOrNull<Node3D>("Body/LegFrontLeft");
        var legFR = cow.GetNodeOrNull<Node3D>("Body/LegFrontRight");
        var legBL = cow.GetNodeOrNull<Node3D>("Body/LegBackLeft");
        var legBR = cow.GetNodeOrNull<Node3D>("Body/LegBackRight");

        AssertThat(legFL).IsNotNull();
        AssertThat(legFR).IsNotNull();
        AssertThat(legBL).IsNotNull();
        AssertThat(legBR).IsNotNull();
    }

    [TestCase]
    public void HasTailChild()
    {
        var cow = AutoFree(CowScene.Instantiate<Node3D>())!;

        var tail = cow.GetNodeOrNull<Node3D>("Body/Tail");
        AssertThat(tail).IsNotNull();
    }

    [TestCase]
    public void HasCowEntityScript()
    {
        var cow = AutoFree(CowScene.Instantiate())!;

        AssertThat(cow is CowsGraveyards.Game.CowEntity).IsTrue();
    }

    [TestCase]
    public void HasBlackPatchSpots()
    {
        var cow = AutoFree(CowScene.Instantiate<Node3D>())!;

        // Cow should have at least one spot/patch child under Body
        var spot = cow.GetNodeOrNull<Node3D>("Body/Spot1");
        AssertThat(spot).IsNotNull();
    }

    [TestCase]
    public void BodyContainsMeshInstance()
    {
        var cow = AutoFree(CowScene.Instantiate<Node3D>())!;

        var body = cow.GetNodeOrNull<Node3D>("Body");
        AssertThat(body).IsNotNull();

        // Body itself or a child should be a MeshInstance3D or CSG node
        bool hasMesh = body is MeshInstance3D or CsgShape3D;
        if (!hasMesh)
        {
            foreach (var child in body!.GetChildren())
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
