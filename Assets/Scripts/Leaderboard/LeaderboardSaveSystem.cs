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
        
        LeaderboardData newLeaderboard = new LeaderboardData();
        newLeaderboard.scores.Add(new ScoreEntry("DEV", 100));
        Save(newLeaderboard);
        return newLeaderboard;
    }
}