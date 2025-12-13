using UnityEngine;
using UnityEngine.UI;

public class FilledClock : MonoBehaviour
{
    [SerializeField]private MonoTimer monoTimer;
    private Image _clock;
    void Awake()
    {
        _clock = GetComponent<Image>();
    }

    //void Update()
    //{
    //    _clock.fillAmount = monoTimer.GetTimePercent();
    //}
    void Update()
    {
        // --- Visual timer ---
        var visualTimer = monoTimer.GetTimePercent()-.05f;
        if (visualTimer < 0f) visualTimer = 0f;

        // Update UI fill (1 = full, 0 = empty)
        _clock.fillAmount = visualTimer / (1-.05f);;
        //Debug.Log(" "+visualTimer+" "+(monoTimer.GetMaxTime()-.1f)+" "+ _clock.fillAmount);
        
    }
}
