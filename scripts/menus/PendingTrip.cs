namespace CowsGraveyards.Menus;

/// <summary>
/// Static handoff for trip slot data between MainMenuScene and GameScene.
/// Written by MainMenuScene before a scene change; read by GameScene._Ready().
/// </summary>
public static class PendingTrip
{
    public static int SlotIndex { get; private set; }
    public static TripSave? Save { get; private set; }

    public static void Set(int slotIndex, TripSave? save)
    {
        SlotIndex = slotIndex;
        Save = save;
    }

    public static void Clear()
    {
        SlotIndex = 0;
        Save = null;
    }
}
