using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
/// <summary>Best time for a single level.</summary>
[Serializable]
public class LevelRecord
{
    public string levelId;
    public float bestTime;
}
 
/// <summary>Everything that gets written to save.json.</summary>
[Serializable]
public class SaveData
{
    public bool hasSavedPosition;
    public string lastSceneName;
    public Vector3 lastPosition;
 
    // JsonUtility can't serialize Dictionary, so we use a List instead.
    public List<LevelRecord> levelRecords = new List<LevelRecord>();
 
    public bool TryGetBestTime(string levelId, out float bestTime)
    {
        foreach (var record in levelRecords)
        {
            if (record.levelId == levelId)
            {
                bestTime = record.bestTime;
                return true;
            }
        }
        bestTime = 0f;
        return false;
    }
 
    /// <summary>Stores the time if it's the first one or beats the old best. Returns true if it's a new record.</summary>
    public bool TrySetBestTime(string levelId, float time)
    {
        foreach (var record in levelRecords)
        {
            if (record.levelId != levelId) continue;
 
            if (time < record.bestTime)
            {
                record.bestTime = time;
                return true;
            }
            return false;
        }
 
        levelRecords.Add(new LevelRecord { levelId = levelId, bestTime = time });
        return true;
    }
}
 
public static class SaveSystem
{
    private static SaveData _current;
 
    // Not a static field initializer: Unity APIs are safest to call lazily.
    private static string SavePath => Path.Combine(Application.persistentDataPath, "save.json");
 
    /// <summary>The cached save data. Loaded from disk the first time it's accessed.</summary>
    public static SaveData Current => _current ??= Load();
 
    public static SaveData Load()
    {
        try
        {
            if (File.Exists(SavePath))
            {
                string json = File.ReadAllText(SavePath);
                var data = JsonUtility.FromJson<SaveData>(json);
                if (data != null) return data;
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Failed to load save, starting fresh: {e.Message}");
        }
 
        return new SaveData();
    }
 
    public static void Save()
    {
        try
        {
            string json = JsonUtility.ToJson(Current, true); // true = pretty print
            string tempPath = SavePath + ".tmp";
 
            // Write to a temp file first so a crash mid-write can't corrupt the real save.
            File.WriteAllText(tempPath, json);
 
            // Swap the finished temp file in. File.Replace does this as a single
            // operation, so there is no moment where save.json is missing.
            if (File.Exists(SavePath))
                File.Replace(tempPath, SavePath, null);
            else
                File.Move(tempPath, SavePath);
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save game: {e.Message}");
        }
    }
 
    public static void DeleteSave()
    {
        _current = new SaveData();
        if (File.Exists(SavePath)) File.Delete(SavePath);
    }
}