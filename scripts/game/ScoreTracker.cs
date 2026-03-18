namespace CowsGraveyards.Game;

public class ScoreTracker
{
    public int LeftScore { get; private set; }

    public int RightScore { get; private set; }

    public void IncrementLeft() => LeftScore++;

    public void IncrementRight() => RightScore++;

    public void Reset()
    {
        LeftScore = 0;
        RightScore = 0;
    }
}
