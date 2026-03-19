namespace CowsGraveyards.Menus;

using System;
using System.Collections.Generic;
using Godot;

/// <summary>
/// Persists completed trip records to a JSON array at user://trip_history.json.
/// Missing or corrupt files return an empty list rather than throwing.
/// </summary>
public class TripHistoryManager
{
    private readonly string _savePath;

    public TripHistoryManager() : this("user://trip_history.json") { }

    public TripHistoryManager(string savePath)
    {
        _savePath = savePath;
    }

    public void Append(CompletedTripRecord record)
    {
        var raw = LoadRaw();
        raw.Add(record.ToDictionary());
        WriteRaw(raw);
    }

    public List<CompletedTripRecord> LoadAll()
    {
        var raw = LoadRaw();
        var result = new List<CompletedTripRecord>(raw.Count);
        foreach (var dict in raw)
            result.Add(CompletedTripRecord.FromDictionary(dict));
        return result;
    }

    public void DeleteAll()
    {
        if (FileAccess.FileExists(_savePath))
            DirAccess.RemoveAbsolute(ProjectSettings.GlobalizePath(_savePath));
    }

    // ── Private helpers ──────────────────────────────────────────────────────

    private List<Godot.Collections.Dictionary> LoadRaw()
    {
        var result = new List<Godot.Collections.Dictionary>();

        if (!FileAccess.FileExists(_savePath))
            return result;

        try
        {
            using var file = FileAccess.Open(_savePath, FileAccess.ModeFlags.Read);
            string json = file.GetAsText();

            var parsed = Json.ParseString(json);
            if (parsed.VariantType != Variant.Type.Array)
                return result;

            var arr = parsed.AsGodotArray();
            foreach (var item in arr)
            {
                if (item.VariantType == Variant.Type.Dictionary)
                    result.Add(item.AsGodotDictionary());
            }
        }
        catch (Exception)
        {
            // Corrupt or unreadable file — return empty list
        }

        return result;
    }

    private void WriteRaw(List<Godot.Collections.Dictionary> records)
    {
        var arr = new Godot.Collections.Array();
        foreach (var dict in records)
            arr.Add(dict);

        string json = Json.Stringify(arr);
        using var file = FileAccess.Open(_savePath, FileAccess.ModeFlags.Write);
        file.StoreString(json);
    }
}
