using TMPro;
using UnityEngine;
using System.Collections;

public class PointSystem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI score;
    [SerializeField] private TextMeshProUGUI multiplier;
    [SerializeField] private int minMultiplierValue = 3;
    [SerializeField] private int maxMultiplierValue = 5;
    [SerializeField] private float shakeDuration = 0.2f;
    [SerializeField] private float baseShakeIntensity = 10f;
    [SerializeField] private float shakeIncrease = 2f;
    public int scoreValue;
    public int multiplierValue;
    public int streak;

    private Vector2 originalMultiplierPosition;
    private RectTransform multiplierRectTransform;

    void Start()
    {
        if (multiplier != null)
        {
            multiplierRectTransform = multiplier.GetComponent<RectTransform>();
            originalMultiplierPosition = multiplierRectTransform.anchoredPosition;
        }
    }

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
           // StartCoroutine(ShakeMultiplier());
        }
    }

    private void UpdateText()
    {
        score.text = scoreValue.ToString();
        multiplier.text = "X" + multiplierValue;
    }

    public IEnumerator ShakeMultiplier()
    {
        if (multiplierRectTransform == null)
        {
            yield break;
        }

        float intensity = baseShakeIntensity + (shakeIncrease * (multiplierValue - 1));
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * intensity;
            float y = Random.Range(-1f, 1f) * intensity;

            multiplierRectTransform.anchoredPosition = originalMultiplierPosition + new Vector2(x, y);

            elapsed += Time.deltaTime;
            yield return null;
        }

        multiplierRectTransform.anchoredPosition = originalMultiplierPosition;
    }
}
