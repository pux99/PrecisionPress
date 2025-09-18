using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    private List<Sprite> _forms = new List<Sprite>();
    private List<Color> _colors = new List<Color>();
    private List<Sprite> _patterns = new List<Sprite>();
    private List<Promt> promts = new List<Promt>();


    private Promt _currentPromt;
    private EditablePieces _currentPiece = new EditablePieces();
    private float currentTimerDuration;

    [SerializeField] private Health health;
    [SerializeField] private List<Piece> pieces;
    [SerializeField] private List<EditablePieces> EditablePieces = new List<EditablePieces>();

    [SerializeField] private TextMeshProUGUI PromtText;
    [SerializeField] private MonoTimer timer;
    [SerializeField] private float StartingTime;
    [SerializeField] private PointSystem pointSystem;
    [SerializeField] private float minTimerDuration = 1f;
    [SerializeField] private float speedIncreaseOnWin = 0.2f;
    [SerializeField] private float speedDecreaseOnLose = 0.2f;
    [SerializeField] private float shakeDuration = 0.2f;
    [SerializeField] private float shakeIntensity = 10f;

    [Header("Audio")] [SerializeField] private AudioSource bgmSource;
    [SerializeField] private float pitchIncreaseFactor = 1.2f;
    [SerializeField] private float maxPitch = 3f;
    [SerializeField] private AudioSource loseSfxSource;
    [SerializeField] private AudioSource winSfxSource;

    private Vector2 originalFormPosition;
    private RectTransform formRectTransform;

    void Start()
    {
        formRectTransform = EditablePieces[0].Form.GetComponent<RectTransform>();
        originalFormPosition = formRectTransform.anchoredPosition;
        currentTimerDuration = StartingTime;

        promts.Add(new ColorPromt());
        promts.Add(new FormPromt());
        promts.Add(new PatternPromt());
        foreach (var piece in pieces)
        {
            _forms.Add(piece.form);
            _colors.Add(piece.Color);
            _patterns.Add(piece.Pattern);
        }

        timer.TimerFinished += Lose;
        health.OnDeath += LossGame;

        if (bgmSource != null && !bgmSource.isPlaying)
        {
            bgmSource.pitch = 1f;
            bgmSource.loop = true;
            bgmSource.Play();
        }

        NextRound();
    }

    public void NextRound()
    {
        _currentPromt = promts[Random.Range(0, promts.Count)];
        SelectAEditablePiece();
        PromtText.text = "Press " + _currentPiece.Name + " " + _currentPromt.ToString();
        NextPiece();
        timer.StartTimer(currentTimerDuration);
    }

    private void SelectAEditablePiece()
    {
        var chanceSum=0;
        foreach (var piece in EditablePieces)
        {
            piece.ChanceToBeSelected.Sort((x, y) => x.Chance.CompareTo(y.Points));
            var toSum=0;
            foreach (var chance in piece.ChanceToBeSelected)
                if (chance.Points <= pointSystem.scoreValue)
                    toSum=chance.Chance;
            chanceSum+=toSum;
        }
        
        var random = Random.Range(1,chanceSum+1);
        
        foreach (var piece in EditablePieces)
        {
            var toRest=0;
            foreach (var chance in piece.ChanceToBeSelected)
                if (chance.Points <= pointSystem.scoreValue)
                    toRest=chance.Chance;
            random -= toRest;
            if (random <= 0)
            {
                _currentPiece=piece;
                break;
            }
        }
    }

    [ContextMenu("Next Piece")]
    public void NextPiece()
    {
        foreach (var piece in EditablePieces)
        {
            if (piece == _currentPiece) continue;
            piece.Change(
                _forms[Random.Range(0, _forms.Count)],
                _colors[Random.Range(0, _colors.Count)],
                _patterns[Random.Range(0, _patterns.Count)]);
        }
        _currentPiece.ForceChange(
            _forms[Random.Range(0, _forms.Count)],
            _colors[Random.Range(0, _colors.Count)],
            _patterns[Random.Range(0, _patterns.Count)]);
    }

    public void CheckPromt(Piece selected)
    {
        if (!_currentPromt.Check(_currentPiece, selected))
        {
            Lose();
            return;
        }
        Win();
    }

    private void Win()
    {
        pointSystem.Score();
        winSfxSource.Play();
        currentTimerDuration = Mathf.Max(minTimerDuration, currentTimerDuration - speedIncreaseOnWin);
        NextRound();
    }

    private void Lose()
    {
        health.TakeDamage();
        pointSystem.Fail();
        loseSfxSource.Play();
        IncreaseBGMPitch();
        StartCoroutine(ShakeForm());
        currentTimerDuration = Mathf.Min(StartingTime, currentTimerDuration + speedDecreaseOnLose);
        NextRound();
    }

    private void IncreaseBGMPitch()
    {
        if (bgmSource == null) return;
        bgmSource.pitch = Mathf.Min(maxPitch, bgmSource.pitch * pitchIncreaseFactor);
    }

    private IEnumerator ShakeForm()
    {
        float elapsed = 0f;
        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeIntensity;
            float y = Random.Range(-1f, 1f) * shakeIntensity;
            formRectTransform.anchoredPosition = originalFormPosition + new Vector2(x, y);
            elapsed += Time.deltaTime;
            yield return null;
        }

        formRectTransform.anchoredPosition = originalFormPosition;
    }

    private void LossGame()
    {
        SceneManager.LoadScene("Loss");
    }

    private void Update()
    {
        KeyCode[] keys = { KeyCode.Q, KeyCode.W, KeyCode.E, KeyCode.R, KeyCode.T };
        for (int i = 0; i < keys.Length && i < pieces.Count; i++)
        {
            if (Input.GetKeyDown(keys[i]))
            {
                CheckPromt(pieces[i]);
                break;
            }
        }
    }
}

[Serializable]
public class Piece
{
    public Sprite form;
    public Color Color;
    public Sprite Pattern;
}

[Serializable]
public class EditablePieces
{
    public string Name;
    public int ChangeFrequency;
    private int _changeCounter;

    public List<ChanceWhenPointsAreX> ChanceToBeSelected;
    //public int ChanceToBeSelected;
    public Image Form;
    public Image Pattern;

    public void Change(Sprite form, Color color, Sprite pattern)
    {
        _changeCounter++;
        if (_changeCounter >= ChangeFrequency)
        {
            _changeCounter = 0;
            Form.sprite = form;
            Form.color = color;
            Pattern.sprite = pattern;
        }
        
    }

    [Serializable]
    public struct ChanceWhenPointsAreX
    {
        public int Points;
        public int Chance;
    }
    public void ForceChange(Sprite form, Color color, Sprite pattern)
    {
        
        _changeCounter = 0;
        Form.sprite = form;
        Form.color = color;
        Pattern.sprite = pattern;
        
    }
}