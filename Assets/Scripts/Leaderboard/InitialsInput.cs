using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class InitialsInput : MonoBehaviour
{
    public string menuScene;
    public TextMeshProUGUI[] letters;
    [SerializeField] private Image upArrow;
    [SerializeField] private Image downArrow;
    [SerializeField] private float arrowPos;
    [SerializeField] private Color arrowPressedColor = Color.yellow;
    private int[] letterIndices = new int[3];
    private int currentIndex = 0;
    private char[] alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
    private bool hasSubmitted = false;

    void Start()
    {
        UpdateLetters();
        UpdateArrows();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            letterIndices[currentIndex] = (letterIndices[currentIndex] + 1) % alphabet.Length;
            UpdateLetters();
            StartCoroutine(FlashArrow(upArrow, arrowPressedColor));
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            letterIndices[currentIndex] = (letterIndices[currentIndex] - 1 + alphabet.Length) % alphabet.Length;
            UpdateLetters();
            StartCoroutine(FlashArrow(downArrow, arrowPressedColor));
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (currentIndex < 2)
            {
                currentIndex++;
                UpdateArrows();
            }
            else if (!hasSubmitted)
            {
                hasSubmitted = true;
                string initials = new string(new char[] {
                    alphabet[letterIndices[0]],
                    alphabet[letterIndices[1]],
                    alphabet[letterIndices[2]]
                });

                var manager = FindFirstObjectByType<LeaderboardManager>();
                if (manager != null)
                {
                    int score = PlayerPrefs.GetInt("PlayerScore", 0);
                    manager.AddScore(initials, score);

                    var ui = FindFirstObjectByType<LeaderboardUI>();
                    if (ui != null)
                    {
                        ui.Refresh();
                    }

                    PlayerPrefs.DeleteKey("PlayerScore");
                }

                StartCoroutine(LoadSceneAfterDelay());
            }
        }
    }

    void UpdateLetters()
    {
        for (int i = 0; i < 3; i++)
        {
            if (letters != null && i < letters.Length && letters[i] != null)
                letters[i].text = alphabet[letterIndices[i]].ToString();
        }
    }

    void UpdateArrows()
    {
        if (currentIndex >= 0 && currentIndex < letters.Length && letters[currentIndex] != null)
        {
            RectTransform letterRect = letters[currentIndex].GetComponent<RectTransform>();
            float arrowOffset = arrowPos;

            upArrow.rectTransform.anchoredPosition = letterRect.anchoredPosition + new Vector2(0, letterRect.rect.height / 2 + arrowOffset);
            downArrow.rectTransform.anchoredPosition = letterRect.anchoredPosition - new Vector2(0, letterRect.rect.height / 2 + arrowOffset);

            upArrow.gameObject.SetActive(true);
            downArrow.gameObject.SetActive(true);
        }
        else
        {
            upArrow.gameObject.SetActive(false);
            downArrow.gameObject.SetActive(false);
        }
    }

    private System.Collections.IEnumerator FlashArrow(Image arrow, Color targetColor)
    {
        Color originalColor = arrow.color;
        arrow.color = targetColor;

        yield return new WaitForSeconds(0.2f);

        arrow.color = originalColor;
    }

    private IEnumerator LoadSceneAfterDelay()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(menuScene);
    }
}