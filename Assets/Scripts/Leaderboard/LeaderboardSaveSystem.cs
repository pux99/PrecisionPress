using UnityEngine;
using System.IO;

public static class LeaderboardSaveSystem
{
    private static string _filePath = Application.persistentDataPath + "/leaderboard.json";

    public static void Save(LeaderboardData data)
    {
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(_filePath, json);
    }

    public static LeaderboardData Load()
    {
        if (File.Exists(_filePath))
        {
            string json = File.ReadAllText(_filePath);
            return JsonUtility.FromJson<LeaderboardData>(json);
        }
        return new LeaderboardData();
    }
}