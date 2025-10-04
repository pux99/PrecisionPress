using TMPro;
using UnityEngine;

public class PointSystem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI score;
    [SerializeField] private TextMeshProUGUI multiplier;
    [SerializeField] private int minMultiplierValue = 3;
    [SerializeField] private int maxMultiplierValue = 5;
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
        if (streak >= minMultiplierValue)
        {
            multiplierValue++;
            if (multiplierValue >= maxMultiplierValue)
                multiplierValue = maxMultiplierValue;
            streak = 0;
        }
    }

    private void UpdateText()
    {
        score.text = scoreValue.ToString();
        multiplier.text = "X" + multiplierValue;
    }
}
