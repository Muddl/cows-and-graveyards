namespace CowsGraveyards.Menus;

using Godot;

/// <summary>
/// Immutable record of a completed trip stored in trip history.
/// CompletedAt is Unix epoch seconds (UTC).
/// </summary>
public class CompletedTripRecord
{
    public int LeftScore { get; }
    public int RightScore { get; }
    public long CompletedAt { get; }

    public CompletedTripRecord(int leftScore, int rightScore, long completedAt)
    {
        LeftScore = leftScore;
        RightScore = rightScore;
        CompletedAt = completedAt;
    }

    public Godot.Collections.Dictionary ToDictionary() => new()
    {
        { "left", LeftScore },
        { "right", RightScore },
        { "completed_at", CompletedAt },
    };

    public static CompletedTripRecord FromDictionary(Godot.Collections.Dictionary dict) =>
        new(dict["left"].AsInt32(), dict["right"].AsInt32(), dict["completed_at"].AsInt64());
}
