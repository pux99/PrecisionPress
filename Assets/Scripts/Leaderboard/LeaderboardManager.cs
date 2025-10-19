using UnityEngine;

public class LeaderboardManager : MonoBehaviour
{
    public int maxEntries = 10;
    public LeaderboardData leaderboard;

    private void Awake()
    {
        leaderboard = LeaderboardSaveSystem.Load();
    }

    public void AddScore(string initials, int score)
    {
        leaderboard.scores.Add(new ScoreEntry(initials, score));
        leaderboard.scores.Sort((a, b) => b.score.CompareTo(a.score));
        if (leaderboard.scores.Count > maxEntries)
            leaderboard.scores.RemoveAt(leaderboard.scores.Count - 1);

        LeaderboardSaveSystem.Save(leaderboard);
    }
}