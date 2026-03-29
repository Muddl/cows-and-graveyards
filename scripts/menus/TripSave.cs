namespace CowsGraveyards.Menus;

using Godot;

/// <summary>
/// Immutable data record for a single saved trip slot.
/// Extends RefCounted so it can be carried as a Godot signal parameter.
/// SlotIndex must be 0–2.
/// </summary>
public partial class TripSave : RefCounted
{
    public int SlotIndex { get; }
    public int LeftScore { get; }
    public int RightScore { get; }
    public bool TutorialSeen { get; }
    public bool GraveyardExplained { get; }

    public TripSave(int slotIndex, int leftScore, int rightScore,
        bool tutorialSeen = false, bool graveyardExplained = false)
    {
        SlotIndex = slotIndex;
        LeftScore = leftScore;
        RightScore = rightScore;
        TutorialSeen = tutorialSeen;
        GraveyardExplained = graveyardExplained;
    }

    public TripSave WithTutorialReset() =>
        new(SlotIndex, LeftScore, RightScore, tutorialSeen: false, graveyardExplained: false);

    public Godot.Collections.Dictionary ToDictionary()
    {
        return new Godot.Collections.Dictionary
        {
            { "slot", SlotIndex },
            { "left", LeftScore },
            { "right", RightScore },
            { "tutorial_seen", TutorialSeen },
            { "graveyard_explained", GraveyardExplained },
        };
    }

    public static TripSave FromDictionary(Godot.Collections.Dictionary dict)
    {
        int slot = dict["slot"].AsInt32();
        int left = dict["left"].AsInt32();
        int right = dict["right"].AsInt32();
        bool tutorialSeen = dict.ContainsKey("tutorial_seen") && dict["tutorial_seen"].AsBool();
        bool graveyardExplained = dict.ContainsKey("graveyard_explained") && dict["graveyard_explained"].AsBool();
        return new TripSave(slot, left, right, tutorialSeen, graveyardExplained);
    }
}
