namespace CowsGraveyards.Game;

public class SideDetector
{
    public TapSide Detect(float tapX, float screenWidth)
    {
        return tapX < screenWidth / 2f ? TapSide.Left : TapSide.Right;
    }
}
