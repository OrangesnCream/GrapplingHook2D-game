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

/// <summary>Last saved player position for a single scene.</summary>
[Serializable]
public class ScenePosition
{
    public string sceneName;
    public Vector2 position;
}

/// <summary>Everything that gets written to save.json.</summary>
[Serializable]
public class SaveData
{
    // JsonUtility can't serialize Dictionary, so we use Lists instead.
    public List<ScenePosition> scenePositions = new List<ScenePosition>();
    public List<LevelRecord> levelRecords = new List<LevelRecord>();

    public bool TryGetPosition(string sceneName, out Vector2 position)
    {
        foreach (var entry in scenePositions)
        {
            if (entry.sceneName == sceneName)
            {
                position = entry.position;
                return true;
            }
        }
        position = Vector2.zero;
        return false;
    }

    public void SetPosition(string sceneName, Vector2 position)
    {
        foreach (var entry in scenePositions)
        {
            if (entry.sceneName == sceneName)
            {
                entry.position = position;
                return;
            }
        }
        scenePositions.Add(new ScenePosition { sceneName = sceneName, position = position });
    }

    /// <summary>Removes the saved position for one scene only. Returns true if there was one to remove.</summary>
    public bool ClearPosition(string sceneName)
    {
        for (int i = 0; i < scenePositions.Count; i++)
        {
            if (scenePositions[i].sceneName == sceneName)
            {
                scenePositions.RemoveAt(i);
                return true;
            }
        }
        return false;
    }

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