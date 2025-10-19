using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LeaderboardUI : MonoBehaviour
{
    public TextMeshProUGUI leaderboardText;
    public LeaderboardManager manager;

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
    }
}