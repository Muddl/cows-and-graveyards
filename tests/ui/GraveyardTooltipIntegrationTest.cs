namespace CowsGraveyards.Tests.UI;

using CowsGraveyards.Game;
using GdUnit4;
using static GdUnit4.Assertions;

[TestSuite]
[RequireGodotRuntime]
public class GraveyardTooltipIntegrationTest
{
    private GameScene _game = null!;

    [BeforeTest]
    public void Setup()
    {
        _game = AutoFree(new GameScene())!;
    }

    [TestCase]
    [RequireGodotRuntime]
    public void FirstGraveyardPressBlockedWhenNotExplained()
    {
        _game.EnableGraveyardTooltip();

        // Score a cow on the right first
        _game.SimulateCowTap(800f);
        AssertThat(_game.Scores.RightScore).IsEqual(1);

        // First graveyard press should be blocked by tooltip
        _game.SimulateGraveyardPress(TapSide.Left);

        AssertThat(_game.Scores.RightScore).IsEqual(1);
        AssertThat(_game.IsGraveyardTooltipShowing).IsTrue();
    }

    [TestCase]
    [RequireGodotRuntime]
    public void GraveyardWorksNormallyAfterTooltipConfirmed()
    {
        _game.EnableGraveyardTooltip();

        _game.SimulateCowTap(800f);
        _game.SimulateGraveyardPress(TapSide.Left); // blocked
        _game.ConfirmGraveyardTooltip();

        // Now graveyard should work
        _game.SimulateGraveyardPress(TapSide.Left);

        AssertThat(_game.Scores.RightScore).IsEqual(0);
    }

    [TestCase]
    [RequireGodotRuntime]
    public void GraveyardWorksImmediatelyWhenAlreadyExplained()
    {
        // Don't enable tooltip — default state is explained

        _game.SimulateCowTap(800f);
        _game.SimulateGraveyardPress(TapSide.Left);

        AssertThat(_game.Scores.RightScore).IsEqual(0);
    }
}
