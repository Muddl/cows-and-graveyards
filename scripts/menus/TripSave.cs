namespace CowsGraveyards.Menus;

using Godot;

/// <summary>
/// Immutable data record for a single saved trip slot.
/// SlotIndex must be 0–2.
/// </summary>
public class TripSave
{
    public int SlotIndex { get; }
    public int LeftScore { get; }
    public int RightScore { get; }

    public TripSave(int slotIndex, int leftScore, int rightScore)
    {
        SlotIndex = slotIndex;
        LeftScore = leftScore;
        RightScore = rightScore;
    }

    public Godot.Collections.Dictionary ToDictionary()
    {
        return new Godot.Collections.Dictionary
        {
            { "slot", SlotIndex },
            { "left", LeftScore },
            { "right", RightScore },
        };
    }

    public static TripSave FromDictionary(Godot.Collections.Dictionary dict)
    {
        int slot = dict["slot"].AsInt32();
        int left = dict["left"].AsInt32();
        int right = dict["right"].AsInt32();
        return new TripSave(slot, left, right);
    }
}
