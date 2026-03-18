namespace CowsGraveyards.Tests.Game;

using CowsGraveyards.Game;
using GdUnit4;
using static GdUnit4.Assertions;

[TestSuite]
[RequireGodotRuntime]
public class GraveyardButtonTest
{
    [TestCase]
    public void LeftButtonHasLeftSide()
    {
        var button = AutoFree(new GraveyardButton())!;
        button.Side = TapSide.Left;

        AssertThat(button.Side).IsEqual(TapSide.Left);
    }

    [TestCase]
    public void RightButtonHasRightSide()
    {
        var button = AutoFree(new GraveyardButton())!;
        button.Side = TapSide.Right;

        AssertThat(button.Side).IsEqual(TapSide.Right);
    }

    [TestCase]
    public void ActivatedSignalCarriesLeftSide()
    {
        var button = AutoFree(new GraveyardButton())!;
        button.Side = TapSide.Left;

        TapSide? received = null;
        button.Activated += (int side) => received = (TapSide)side;
        button.OnButtonPressed();

        AssertThat(received).IsNotNull();
        AssertThat(received).IsEqual(TapSide.Left);
    }

    [TestCase]
    public void ActivatedSignalCarriesRightSide()
    {
        var button = AutoFree(new GraveyardButton())!;
        button.Side = TapSide.Right;

        TapSide? received = null;
        button.Activated += (int side) => received = (TapSide)side;
        button.OnButtonPressed();

        AssertThat(received).IsNotNull();
        AssertThat(received).IsEqual(TapSide.Right);
    }
}
