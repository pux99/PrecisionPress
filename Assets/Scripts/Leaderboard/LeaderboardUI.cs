using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LeaderboardUI : MonoBehaviour
{
    public TextMeshProUGUI leaderboardText;
    public LeaderboardManager manager;
    public TextMeshProUGUI playerScoreText;

    void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        leaderboardText.text = "";
        int rank = 1;
        
        foreach (var entry in manager.leaderboard.scores)
        {
            leaderboardText.text += rank + ". " + entry.initials + " - " + entry.score + "\n";
            rank++;
        }

        if (playerScoreText != null)
        {
            int playerScore = PlayerPrefs.GetInt("PlayerScore", -1);
            if (playerScore >= 0)
                playerScoreText.text = "Your Score: " + playerScore;
            else
                playerScoreText.text = "";
        }
    }
}