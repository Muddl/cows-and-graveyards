namespace CowsGraveyards.Game;

public class GameState
{
    public ScoreTracker Scores { get; private set; } = new();

    public bool IsPlaying { get; private set; } = true;

    public void Reset()
    {
        Scores.Reset();
        IsPlaying = true;
    }
}
