namespace CowsGraveyards.Tests.Game;

using CowsGraveyards.Game;
using GdUnit4;
using Godot;
using static GdUnit4.Assertions;

[TestSuite]
[RequireGodotRuntime]
public class GraveyardEntityTest
{
    private GraveyardEntity _entity = null!;

    private static readonly Vector3 Start = new(-5f, 0f, -25f);
    private static readonly Vector3 End = new(-5f, 0f, 10f);

    [BeforeTest]
    public void Setup()
    {
        _entity = AutoFree(new GraveyardEntity())!;
    }

    [TestCase]
    public void StartsAtSpawnPositionAfterStartDrivePast()
    {
        _entity.StartDrivePast(Start, End);

        AssertThat(_entity.Position).IsEqual(Start);
    }

    [TestCase]
    public void MovesTowardEndPositionDuringProcess()
    {
        _entity.StartDrivePast(Start, End, duration: 3f);

        _entity._Process(1.0);  // one third of duration

        AssertThat(_entity.Position.Z).IsGreater(Start.Z);
        AssertThat(_entity.Position.Z).IsLess(End.Z);
    }

    [TestCase]
    public void IsQueuedFreeAfterDurationElapsed()
    {
        _entity.StartDrivePast(Start, End, duration: 3f);

        _entity._Process(3.0);  // full duration in one step

        AssertThat(_entity.IsQueuedForDeletion()).IsTrue();
    }

    [TestCase]
    public void AccumulatesDeltaAcrossMultipleProcessCalls()
    {
        _entity.StartDrivePast(Start, End, duration: 3f);

        _entity._Process(1.0);
        _entity._Process(1.0);
        _entity._Process(1.1);  // total 3.1s — past the end

        AssertThat(_entity.IsQueuedForDeletion()).IsTrue();
    }

    [TestCase]
    public void DoesNotMoveBeforeStartDrivePastIsCalled()
    {
        var initialPosition = _entity.Position;

        _entity._Process(1.0);

        AssertThat(_entity.Position).IsEqual(initialPosition);
    }
}
