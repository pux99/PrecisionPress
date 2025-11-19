using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Fade : MonoBehaviour
{
    
    [SerializeField] private Image FadeImage;
    public float FadeDuration;
    public event Action FinishFade;
    
    public void FadeIn()
    {
        StartCoroutine(Fading(1, 0));
    }
    public void FadeOut()
    {
        StartCoroutine(Fading(0, 1));
    }
    IEnumerator Fading(float from, float to)
    {
        float t = 0f;
        Color c = FadeImage.color;

        while (t < FadeDuration)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(from, to, t / FadeDuration);
            FadeImage.color = new Color(c.r, c.g, c.b, a);
            yield return null;
        }

        FadeImage.color = new Color(c.r, c.g, c.b, to);
        FinishFade?.Invoke();
    }
}
