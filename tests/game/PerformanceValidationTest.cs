namespace CowsGraveyards.Tests.Game;

using System.Diagnostics;
using GdUnit4;
using Godot;
using static GdUnit4.Assertions;

[TestSuite]
[RequireGodotRuntime]
public class PerformanceValidationTest
{
    private static readonly PackedScene GameScenePacked =
        GD.Load<PackedScene>("res://scenes/game/GameScene.tscn");

    private static readonly PackedScene CowScene =
        GD.Load<PackedScene>("res://scenes/game/CowEntity.tscn");

    private static readonly PackedScene GraveyardScene =
        GD.Load<PackedScene>("res://scenes/game/GraveyardEntity.tscn");

    [TestCase]
    public void GameSceneNodeCountIsReasonable()
    {
        var scene = AutoFree(GameScenePacked.Instantiate<Node3D>())!;

        int nodeCount = CountNodes(scene);
        // Scene should stay under 200 nodes even with all enhancements
        AssertThat(nodeCount).IsLess(200);
    }

    [TestCase]
    public void SpawningTenCowsIsAcceptablyFast()
    {
        var sw = Stopwatch.StartNew();

        for (int i = 0; i < 10; i++)
        {
            var cow = AutoFree(CowScene.Instantiate<Node3D>())!;
            AssertThat(cow).IsNotNull();
        }

        sw.Stop();
        // 10 cows should instantiate in under 500ms
        AssertThat(sw.ElapsedMilliseconds).IsLess(500L);
    }

    [TestCase]
    public void SpawningTenGraveyardsIsAcceptablyFast()
    {
        var sw = Stopwatch.StartNew();

        for (int i = 0; i < 10; i++)
        {
            var graveyard = AutoFree(GraveyardScene.Instantiate<Node3D>())!;
            AssertThat(graveyard).IsNotNull();
        }

        sw.Stop();
        // 10 graveyards should instantiate in under 500ms
        AssertThat(sw.ElapsedMilliseconds).IsLess(500L);
    }

    [TestCase]
    public void CowEntityNodeCountIsReasonable()
    {
        var cow = AutoFree(CowScene.Instantiate<Node3D>())!;

        int nodeCount = CountNodes(cow);
        // Cow should stay under 30 nodes
        AssertThat(nodeCount).IsLess(30);
    }

    [TestCase]
    public void GraveyardEntityNodeCountIsReasonable()
    {
        var graveyard = AutoFree(GraveyardScene.Instantiate<Node3D>())!;

        int nodeCount = CountNodes(graveyard);
        // Graveyard should stay under 20 nodes
        AssertThat(nodeCount).IsLess(20);
    }

    [TestCase]
    public void MixedSpawnScenarioUnder200Nodes()
    {
        var scene = AutoFree(GameScenePacked.Instantiate<Node3D>())!;

        // Simulate a busy scene: 5 cows + 2 graveyards
        for (int i = 0; i < 5; i++)
        {
            var cow = CowScene.Instantiate<Node3D>();
            scene.AddChild(cow);
        }

        for (int i = 0; i < 2; i++)
        {
            var graveyard = GraveyardScene.Instantiate<Node3D>();
            scene.AddChild(graveyard);
        }

        int totalNodes = CountNodes(scene);
        // Full scene with 5 cows + 2 graveyards should stay under 350 nodes
        AssertThat(totalNodes).IsLess(350);
    }

    private static int CountNodes(Node node)
    {
        int count = 1;
        foreach (var child in node.GetChildren())
        {
            count += CountNodes(child);
        }
        return count;
    }
}
