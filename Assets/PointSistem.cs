using TMPro;
using UnityEngine;

public class PointSystem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI score;
    [SerializeField] private TextMeshProUGUI multiplier;
    public int scoreValue;
    public int multiplierValue;
    public int streak;

    public void Score()
    {
        scoreValue += 1 * multiplierValue;
        CheckStreak();
        UpdateText();
    }

    public void Fail()
    {
        multiplierValue = 1;
        UpdateText();
    }

    private void CheckStreak()
    {
        streak++;
        if (streak >= 3)
        {
            multiplierValue++;
            if (multiplierValue >= 5)
                multiplierValue = 5;
            streak = 0;
        }
    }

    private void UpdateText()
    {
        score.text = scoreValue.ToString();
        multiplier.text = "X" + multiplierValue;
    }
}
