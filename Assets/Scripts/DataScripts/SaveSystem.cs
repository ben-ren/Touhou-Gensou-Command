using System.IO;
using UnityEngine;

/// <summary>
/// Handles writing/reading save files to disk.
///
/// Save slot system:
/// save_0.json
/// save_1.json
/// save_2.json
/// etc.
/// </summary>
public static class SaveSystem
{
    /// <summary>
    /// Save folder location
    /// Unity-safe persistent location
    /// </summary>
    private static string SaveFolder =>
        Application.persistentDataPath + "/Saves/";

    /// <summary>
    /// Save current game into a slot
    /// </summary>
    public static void SaveGame(SaveGameData data, int slot)
    {
        if (!Directory.Exists(SaveFolder))
            Directory.CreateDirectory(SaveFolder);

        string path = SaveFolder + $"save_{slot}.json";

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);

        Debug.Log($"Saved Game: {path}");
    }

    /// <summary>
    /// Load save file from slot
    /// </summary>
    public static SaveGameData LoadGame(int slot)
    {
        string path = SaveFolder + $"save_{slot}.json";

        if (!File.Exists(path))
        {
            Debug.LogWarning("Save file does not exist.");
            return null;
        }

        string json = File.ReadAllText(path);

        SaveGameData data =
            JsonUtility.FromJson<SaveGameData>(json);

        Debug.Log($"Loaded Game: {path}");

        return data;
    }

    /// <summary>
    /// Check if slot exists
    /// </summary>
    public static bool SaveExists(int slot)
    {
        string path = SaveFolder + $"save_{slot}.json";
        return File.Exists(path);
    }

    /// <summary>
    /// Delete save slot
    /// </summary>
    public static void DeleteSave(int slot)
    {
        string path = SaveFolder + $"save_{slot}.json";

        if (File.Exists(path))
            File.Delete(path);
    }
}