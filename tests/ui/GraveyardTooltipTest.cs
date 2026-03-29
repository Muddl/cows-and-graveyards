namespace CowsGraveyards.Tests.UI;

using CowsGraveyards.UI;
using GdUnit4;
using static GdUnit4.Assertions;

[TestSuite]
[RequireGodotRuntime]
public class GraveyardTooltipTest
{
    private GraveyardTooltip _tooltip = null!;

    [BeforeTest]
    public void Setup()
    {
        _tooltip = AutoFree(new GraveyardTooltip())!;
    }

    [TestCase]
    [RequireGodotRuntime]
    public void ShowsOnFirstPressWhenNotExplained()
    {
        _tooltip.Initialize(graveyardExplained: false);

        bool shouldBlock = _tooltip.ShouldBlockAction();

        AssertThat(shouldBlock).IsTrue();
    }

    [TestCase]
    [RequireGodotRuntime]
    public void DoesNotShowOnPressWhenAlreadyExplained()
    {
        _tooltip.Initialize(graveyardExplained: true);

        bool shouldBlock = _tooltip.ShouldBlockAction();

        AssertThat(shouldBlock).IsFalse();
    }

    [TestCase]
    [RequireGodotRuntime]
    public void ConfirmSetsExplainedToTrue()
    {
        _tooltip.Initialize(graveyardExplained: false);
        _tooltip.ShouldBlockAction();

        _tooltip.Confirm();

        AssertThat(_tooltip.IsExplained).IsTrue();
    }

    [TestCase]
    [RequireGodotRuntime]
    public void ConfirmEmitsConfirmedSignal()
    {
        _tooltip.Initialize(graveyardExplained: false);
        _tooltip.ShouldBlockAction();
        bool confirmed = false;
        _tooltip.Confirmed += () => confirmed = true;

        _tooltip.Confirm();

        AssertThat(confirmed).IsTrue();
    }

    [TestCase]
    [RequireGodotRuntime]
    public void DoesNotBlockOnSubsequentPressesAfterConfirm()
    {
        _tooltip.Initialize(graveyardExplained: false);
        _tooltip.ShouldBlockAction();
        _tooltip.Confirm();

        bool shouldBlock = _tooltip.ShouldBlockAction();

        AssertThat(shouldBlock).IsFalse();
    }

    [TestCase]
    [RequireGodotRuntime]
    public void ShouldBlockReturnsFalseWhenInitializedExplained()
    {
        _tooltip.Initialize(graveyardExplained: true);

        AssertThat(_tooltip.ShouldBlockAction()).IsFalse();
        AssertThat(_tooltip.IsExplained).IsTrue();
    }
}
