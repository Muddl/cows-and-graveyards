namespace CowsGraveyards.Menus;

using System;
using System.Collections.Generic;
using Godot;
using Godot.Collections;

/// <summary>
/// Persists up to 3 trip save slots to a JSON file via Godot FileAccess.
/// Slots are stored as an ordered JSON array; missing or corrupt entries
/// are returned as null rather than throwing.
/// </summary>
public class SaveManager
{
    private const int SlotCount = 3;

    private readonly string _savePath;

    public SaveManager() : this("user://trips.json") { }

    public SaveManager(string savePath)
    {
        _savePath = savePath;
    }

    public void SaveSlot(TripSave save)
    {
        var slots = LoadRawSlots();
        slots[save.SlotIndex] = save.ToDictionary();
        WriteSlots(slots);
    }

    public TripSave? LoadSlot(int slotIndex)
    {
        var slots = LoadRawSlots();
        var entry = slots[slotIndex];
        if (entry is null)
            return null;
        return TripSave.FromDictionary(entry);
    }

    public List<TripSave?> LoadAllSlots()
    {
        var slots = LoadRawSlots();
        var result = new List<TripSave?>(SlotCount);
        for (int i = 0; i < SlotCount; i++)
        {
            result.Add(slots[i] is { } entry ? TripSave.FromDictionary(entry) : null);
        }
        return result;
    }

    public void DeleteSlot(int slotIndex)
    {
        var slots = LoadRawSlots();
        slots[slotIndex] = null;
        WriteSlots(slots);
    }

    public void DeleteAll()
    {
        if (FileAccess.FileExists(_savePath))
            DirAccess.RemoveAbsolute(ProjectSettings.GlobalizePath(_savePath));
    }

    // ── Private helpers ──────────────────────────────────────────────────────

    /// <summary>
    /// Returns an array of <see cref="SlotCount"/> nullable dictionaries,
    /// one per slot. Returns all-nulls on missing or corrupt file.
    /// </summary>
    private Godot.Collections.Dictionary?[] LoadRawSlots()
    {
        var slots = new Godot.Collections.Dictionary?[SlotCount];

        if (!FileAccess.FileExists(_savePath))
            return slots;

        try
        {
            using var file = FileAccess.Open(_savePath, FileAccess.ModeFlags.Read);
            string json = file.GetAsText();

            var parsed = Json.ParseString(json);
            if (parsed.VariantType != Variant.Type.Array)
                return slots;

            var arr = parsed.AsGodotArray();
            for (int i = 0; i < SlotCount && i < arr.Count; i++)
            {
                if (arr[i].VariantType == Variant.Type.Dictionary)
                    slots[i] = arr[i].AsGodotDictionary();
            }
        }
        catch (Exception)
        {
            // Corrupt or unreadable file — return all-nulls
        }

        return slots;
    }

    private void WriteSlots(Godot.Collections.Dictionary?[] slots)
    {
        var arr = new Godot.Collections.Array();
        foreach (var slot in slots)
        {
            if (slot is not null)
                arr.Add(slot);
            else
                arr.Add(new Variant());
        }

        string json = Json.Stringify(arr);
        using var file = FileAccess.Open(_savePath, FileAccess.ModeFlags.Write);
        file.StoreString(json);
    }
}
