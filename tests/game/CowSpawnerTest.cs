namespace CowsGraveyards.Tests.Game;

using CowsGraveyards.Game;
using GdUnit4;
using static GdUnit4.Assertions;

[TestSuite]
public class CowSpawnerTest
{
    private CowSpawner _spawner = null!;

    [BeforeTest]
    public void Setup()
    {
        _spawner = new CowSpawner();
    }

    [TestCase]
    [RequireGodotRuntime]
    public void LeftSpawnPositionHasNegativeX()
    {
        var position = _spawner.GetSpawnPosition(TapSide.Left);

        AssertThat(position.X).IsLess(0f);
    }

    [TestCase]
    [RequireGodotRuntime]
    public void RightSpawnPositionHasPositiveX()
    {
        var position = _spawner.GetSpawnPosition(TapSide.Right);

        AssertThat(position.X).IsGreater(0f);
    }

    [TestCase]
    [RequireGodotRuntime]
    public void SpawnPositionsAreSymmetric()
    {
        var left = _spawner.GetSpawnPosition(TapSide.Left);
        var right = _spawner.GetSpawnPosition(TapSide.Right);

        AssertThat(left.X).IsEqual(-right.X);
        AssertThat(left.Z).IsEqual(right.Z);
    }

    [TestCase]
    [RequireGodotRuntime]
    public void SpawnPositionIsAtHorizonDistance()
    {
        var position = _spawner.GetSpawnPosition(TapSide.Left);

        AssertThat(position.Z).IsLess(-20f);
    }

    [TestCase]
    [RequireGodotRuntime]
    public void SpawnPositionYIsAtGroundLevel()
    {
        var left = _spawner.GetSpawnPosition(TapSide.Left);
        var right = _spawner.GetSpawnPosition(TapSide.Right);

        AssertThat(left.Y).IsEqual(0f);
        AssertThat(right.Y).IsEqual(0f);
    }

    [TestCase]
    public void SpawnCountStartsAtZero()
    {
        AssertThat(_spawner.SpawnCount).IsEqual(0);
    }

    [TestCase]
    public void RecordSpawnIncrementsCount()
    {
        _spawner.RecordSpawn();
        _spawner.RecordSpawn();

        AssertThat(_spawner.SpawnCount).IsEqual(2);
    }

    [TestCase]
    [RequireGodotRuntime]
    public void GetEndPositionIsNearCamera()
    {
        var endPos = _spawner.GetEndPosition(TapSide.Left);

        AssertThat(endPos.Z).IsGreater(5f);
    }

    [TestCase]
    [RequireGodotRuntime]
    public void EndPositionPreservesSide()
    {
        var leftEnd = _spawner.GetEndPosition(TapSide.Left);
        var rightEnd = _spawner.GetEndPosition(TapSide.Right);

        AssertThat(leftEnd.X).IsLess(0f);
        AssertThat(rightEnd.X).IsGreater(0f);
    }
}
