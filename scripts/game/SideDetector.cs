namespace CowsGraveyards.Game;

public class SideDetector
{
    public Side Detect(float tapX, float screenWidth)
    {
        return tapX < screenWidth / 2f ? Side.Left : Side.Right;
    }
}
