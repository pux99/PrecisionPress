using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
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
    [SerializeField] private float bgScrollSpeedIncrease = 10f;
    [SerializeField] private string scoreScene;

    [Header("Audio")] [SerializeField] private AudioSource bgmSource;
    [SerializeField] private float pitchIncreaseFactor = 1.2f;
    [SerializeField] private float maxPitch = 3f;
    [SerializeField] private AudioSource loseSfxSource;
    [SerializeField] private AudioSource winSfxSource;

    [Header("Input")]
    [SerializeField] private float inputCooldown = 0.2f;
    private float _lastInputTime = -Mathf.Infinity;

    private Vector2 originalFormPosition;
    private RectTransform formRectTransform;

    [Header("Fade")]
    [SerializeField] private Fade fade;
    private bool PauseInput = true;
    

    void Start()
    {
        fade.FinishFade+=UnpauseInputs;
        fade.FinishFade+=StartTimer;
        fade.FadeIn();
        inputCooldown = Mathf.Max(0f, inputCooldown);
        EditablePieces firstWithForm = null;
        foreach (var e in EditablePieces)
        {
            if (e != null && e.Form != null)
            {
                firstWithForm = e;
                break;
            }
        }

        if (firstWithForm != null)
        {
            formRectTransform = firstWithForm.Form.GetComponent<RectTransform>();
            originalFormPosition = formRectTransform.anchoredPosition;
        }
        else
        {
            if (EditablePieces.Count > 0 && EditablePieces[0] != null && EditablePieces[0].Form != null)
            {
                formRectTransform = EditablePieces[0].Form.GetComponent<RectTransform>();
                originalFormPosition = formRectTransform.anchoredPosition;
            }
            else
            {
                formRectTransform = null;
                originalFormPosition = Vector2.zero;
            }
        }

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

        foreach (var piece in EditablePieces)
        {
            if (piece != null && piece.IsTileable)
            {
                piece.SetRandomScrollDirection();
            }
        }

        if (EditablePieces != null && EditablePieces.Count > 0)
        {
            _currentPiece = EditablePieces[0];
        }

        FirstRound();
    }

    public void NextRound()
    {
        _currentPromt = promts[Random.Range(0, promts.Count)];
        SelectAEditablePiece();

        if (_currentPiece == null)
        {
            Debug.LogWarning("NextRound: no EditablePiece available, skipping round setup.");
            PromtText.text = "Press ? " + _currentPromt.ToString();
            return;
        }

        PromtText.text = _currentPromt.ToString();
        NextPiece();
        
        _currentPiece?.SetRandomScrollDirection();
        
        timer.StartTimer(currentTimerDuration);
    }
    private void FirstRound()
    {
        _currentPromt = promts[Random.Range(0, promts.Count)];
        SelectAEditablePiece();

        if (_currentPiece == null)
        {
            Debug.LogWarning("NextRound: no EditablePiece available, skipping round setup.");
            PromtText.text = "Press ? " + _currentPromt.ToString();
            return;
        }

        PromtText.text = _currentPromt.ToString();
        NextPiece();
        
        _currentPiece?.SetRandomScrollDirection();
        
        
    }
    private void StartTimer()
    {
        timer.StartTimer(currentTimerDuration);
        fade.FinishFade-=StartTimer;
    }

    private void SelectAEditablePiece()
    {
        var chanceSum = 0;
        if (EditablePieces == null || EditablePieces.Count == 0)
        {
            _currentPiece = null;
            return;
        }

        foreach (var piece in EditablePieces)
        {
            if (piece == null || piece.ChanceToBeSelected == null) continue;
            try
            {
                piece.ChanceToBeSelected.Sort((x, y) => x.Chance.CompareTo(y.Points));
            }
            catch
            {
                //Ignore sort errors
            }

            var toSum = 0;
            foreach (var chance in piece.ChanceToBeSelected)
                if (chance.Points <= pointSystem.scoreValue)
                    toSum = chance.Chance;
            chanceSum += toSum;
        }

        if (chanceSum <= 0)
        {
            foreach (var piece in EditablePieces)
            {
                if (piece != null)
                {
                    _currentPiece = piece;
                    return;
                }
            }

            _currentPiece = null;
            return;
        }

        var random = Random.Range(1, chanceSum + 1);

        foreach (var piece in EditablePieces)
        {
            if (piece == null || piece.ChanceToBeSelected == null) continue;
            var toRest = 0;
            foreach (var chance in piece.ChanceToBeSelected)
                if (chance.Points <= pointSystem.scoreValue)
                    toRest = chance.Chance;
            random -= toRest;
            if (random <= 0)
            {
                _currentPiece = piece;
                break;
            }
        }

        if (_currentPiece == null)
        {
            foreach (var piece in EditablePieces)
            {
                if (piece != null)
                {
                    _currentPiece = piece;
                    break;
                }
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
        if (_currentPromt == null)
        {
            Debug.LogWarning("CheckPromt called but _currentPromt is null");
            return;
        }

        if (_currentPiece == null || selected == null)
        {
            Debug.LogWarning("CheckPromt called with null pieces");
            return;
        }

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
        StartCoroutine(pointSystem.ShakeMultiplier());
        if (winSfxSource != null) winSfxSource.Play();
        currentTimerDuration = Mathf.Max(minTimerDuration, currentTimerDuration - speedIncreaseOnWin);
        NextRound();
    }

    private void Lose()
    {
        health.TakeDamage();
        pointSystem.Fail();
        if (loseSfxSource != null) loseSfxSource.Play();
        IncreaseBGMPitch();
        IncreaseBackgroundScrollSpeed();
        StartCoroutine(ShakeForm());
        currentTimerDuration = Mathf.Min(StartingTime, currentTimerDuration + speedDecreaseOnLose);
        NextRound();
    }

    private void IncreaseBGMPitch()
    {
        if (bgmSource == null) return;
        bgmSource.pitch = Mathf.Min(maxPitch, bgmSource.pitch * pitchIncreaseFactor);
    }

    private void IncreaseBackgroundScrollSpeed()
    {
        foreach (var piece in EditablePieces)
        {
            if (piece.IsTileable)
            {
                piece.TileScrollSpeed += bgScrollSpeedIncrease;
            }
        }
    }

    private IEnumerator ShakeForm()
    {
        if (formRectTransform == null)
        {
            yield break;
        }

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
        PlayerPrefs.SetInt("PlayerScore", pointSystem.scoreValue);
        fade.FinishFade += ChangeToScoreBoard;
        fade.FadeOut();
    }

    private void ChangeToScoreBoard()
    {
        fade.FinishFade -= ChangeToScoreBoard;
        SceneManager.LoadScene(scoreScene);
    }

    public void ButtonInput(/*int code,*/InputAction.CallbackContext context)
    {
        Debug.Log(context.action.name);
        if (!context.performed||PauseInput) return;

        if (Time.time - _lastInputTime < inputCooldown) return;
        _lastInputTime = Time.time;

        switch (context.action.name)
        {
            case "Form1":
                if (pieces != null && pieces.Count > 0) CheckPromt(pieces[0]);
                break;
            case "Form2":
                if (pieces != null && pieces.Count > 1) CheckPromt(pieces[1]);
                break;
            case "Form3":
                if (pieces != null && pieces.Count > 2) CheckPromt(pieces[2]);
                break;
            case "Form4":
                if (pieces != null && pieces.Count > 3) CheckPromt(pieces[3]);
                break;
            case "Form5":
                if (pieces != null && pieces.Count > 4) CheckPromt(pieces[4]);
                break;
            case "Form6":
                if (pieces != null && pieces.Count > 5) CheckPromt(pieces[5]);
                break;
        }
    }

    private void UnpauseInputs()
    {
        PauseInput = false;
        fade.FinishFade-=UnpauseInputs;
    }
    private void Update()
    {
        //KeyCode[] keys = { KeyCode.Q, KeyCode.W, KeyCode.E, KeyCode.R, KeyCode.T };
        //for (int i = 0; i < keys.Length && i < pieces.Count; i++)
        //{
        //    
        //    if (Input.GetKeyDown(keys[i]))
        //    {
        //        CheckPromt(pieces[i]);
        //        break;
        //    }
        //}

        foreach (var piece in EditablePieces)
        {
            piece.UpdateTileScroll(Time.deltaTime);
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
    public bool IsTileable = false;
    public float FormTileScale = 1f;
    public float TileScrollSpeed = 50f;
    public float TileSpacing = 0f;

    private GameObject tilesContainer;
    private List<GameObject> tileObjects = new List<GameObject>();
    private Dictionary<GameObject, Vector2> tileBasePositions = new Dictionary<GameObject, Vector2>();
    
    private Color _currentColor;
    private Sprite _currentForm;
    private Sprite _currentPattern;
    
    private Vector2 _scrollDirection = Vector2.zero;
    private Vector2 _currentOffset = Vector2.zero;
    private Vector2 _effectiveTileSize = Vector2.zero;
    
    public Color CurrentColor => _currentColor;
    public Sprite CurrentForm => _currentForm;
    public Sprite CurrentPattern => _currentPattern;

    public void Change(Sprite form, Color color, Sprite pattern)
    {
        _changeCounter++;
        if (_changeCounter >= ChangeFrequency)
        {
            _changeCounter = 0;
            
            _currentForm = form;
            _currentColor = color;
            _currentPattern = pattern;
            
            if (IsTileable)
            {
                CreateTiledPiecesWithPatterns(form, color, pattern);
            }
            else
            {
                Form.sprite = form;
                Form.color = color;
                Form.type = Image.Type.Simple;
                Pattern.sprite = pattern;
                Pattern.type = Image.Type.Simple;
            }
        }
    }

    [Serializable]
    public struct ChanceWhenPointsAreX
    {
        public int Points;
        public int Chance;
    }
    
    // csharp
private void CreateTiledPiecesWithPatterns(Sprite form, Color color, Sprite pattern)
{
    if (Form == null) return;

    // Keep the original/hidden images in sync so any code that reads them (prompts, checks) sees correct values
    Form.sprite = form;
    Form.color = color;
    Form.type = Image.Type.Simple;
    Pattern.sprite = pattern;
    Pattern.type = Image.Type.Simple;
    Pattern.color = Color.white; // pattern usually uses its own colors/alpha, keep it white to preserve sprite colors

    ClearTiles();

    if (tilesContainer == null)
    {
        tilesContainer = new GameObject("TilesContainer");
        RectTransform containerRect = tilesContainer.AddComponent<RectTransform>();
        containerRect.SetParent(Form.transform.parent, false);

        containerRect.anchorMin = Vector2.zero;
        containerRect.anchorMax = Vector2.one;
        containerRect.offsetMin = Vector2.zero;
        containerRect.offsetMax = Vector2.zero;
        containerRect.pivot = new Vector2(0.5f, 0.5f);

        tilesContainer.AddComponent<RectMask2D>();

        int formIndex = Form.transform.GetSiblingIndex();
        containerRect.SetSiblingIndex(formIndex);
    }

    // Hide the original images (we already updated their values)
    Form.enabled = false;
    Pattern.enabled = false;

    RectTransform parentRect = Form.GetComponent<RectTransform>();
    float containerWidth = parentRect.rect.width;
    float containerHeight = parentRect.rect.height;

    float spriteWidth = form.rect.width;
    float spriteHeight = form.rect.height;
    float pixelsPerUnit = form.pixelsPerUnit;

    float tileWidth = (spriteWidth / pixelsPerUnit) * 100f / FormTileScale;
    float tileHeight = (spriteHeight / pixelsPerUnit) * 100f / FormTileScale;

    float effectiveTileWidth = tileWidth + TileSpacing;
    float effectiveTileHeight = tileHeight + TileSpacing;

    _effectiveTileSize = new Vector2(effectiveTileWidth, effectiveTileHeight);

    int tilesX = Mathf.CeilToInt(containerWidth / effectiveTileWidth) + 2;
    int tilesY = Mathf.CeilToInt(containerHeight / effectiveTileHeight) + 2;

    float startX = -(tilesX * effectiveTileWidth) / 2f + effectiveTileWidth / 2f;
    float startY = -(tilesY * effectiveTileHeight) / 2f + effectiveTileHeight / 2f;

    for (int y = 0; y < tilesY; y++)
    {
        for (int x = 0; x < tilesX; x++)
        {
            GameObject tileObj = new GameObject($"Tile_{x}_{y}");
            RectTransform tileRect = tileObj.AddComponent<RectTransform>();
            tileRect.SetParent(tilesContainer.transform, false);
            tileObj.AddComponent<Mask>();

            Vector2 basePos = new Vector2(startX + x * effectiveTileWidth, startY + y * effectiveTileHeight);

            tileRect.sizeDelta = new Vector2(tileWidth, tileHeight);
            tileRect.anchoredPosition = basePos;
            tileRect.pivot = new Vector2(0.5f, 0.5f);

            Image formImage = tileObj.AddComponent<Image>();
            formImage.sprite = form;
            formImage.color = color;
            formImage.type = Image.Type.Simple;
            formImage.raycastTarget = false;

            GameObject patternObj = new GameObject("Pattern");
            RectTransform patternRect = patternObj.AddComponent<RectTransform>();
            patternRect.SetParent(tileRect, false);

            patternRect.anchorMin = Vector2.zero;
            patternRect.anchorMax = Vector2.one;
            patternRect.offsetMin = Vector2.zero;
            patternRect.offsetMax = Vector2.zero;
            patternRect.pivot = new Vector2(0.5f, 0.5f);

            Image patternImage = patternObj.AddComponent<Image>();
            patternImage.sprite = pattern;
            patternImage.type = Image.Type.Simple;
            patternImage.color = Color.white;
            patternImage.raycastTarget = false;

            tileObjects.Add(tileObj);
            tileBasePositions[tileObj] = basePos;
        }
    }

    _currentOffset = Vector2.zero;
}



    
private void ClearTiles()
{
    foreach (var tile in tileObjects)
    {
        if (tile != null)
        {
            UnityEngine.Object.Destroy(tile);
        }
    }
    tileObjects.Clear();
    tileBasePositions.Clear();

    if (tilesContainer != null)
    {
        UnityEngine.Object.Destroy(tilesContainer);
        tilesContainer = null;
    }
}
    
    public void ForceChange(Sprite form, Color color, Sprite pattern)
    {
        _changeCounter = 0;
        
        _currentForm = form;
        _currentColor = color;
        _currentPattern = pattern;
        
        if (IsTileable)
        {
            CreateTiledPiecesWithPatterns(form, color, pattern);
        }
        else
        {
            Form.sprite = form;
            Form.color = color;
            Form.type = Image.Type.Simple;
            Pattern.sprite = pattern;
            Pattern.type = Image.Type.Simple;
            
            Form.enabled = true;
            Pattern.enabled = true;
            
            ClearTiles();
        }
    }

    public void SetRandomScrollDirection()
    {
        if (!IsTileable) return;

        int direction = Random.Range(0, 4);
        switch (direction)
        {
            case 0:
                _scrollDirection = Vector2.right;
                break;
            case 1:
                _scrollDirection = Vector2.left;
                break;
            case 2:
                _scrollDirection = Vector2.up;
                break;
            case 3:
                _scrollDirection = Vector2.down;
                break;
        }
    }

    public void UpdateTileScroll(float deltaTime)
    {
        if (!IsTileable || tilesContainer == null || _scrollDirection == Vector2.zero || tileObjects.Count == 0) return;
        if (_effectiveTileSize == Vector2.zero) return;
        
        _currentOffset += _scrollDirection * TileScrollSpeed * deltaTime;
        
        foreach (var tileObj in tileObjects)
        {
            if (tileObj != null && tileBasePositions.ContainsKey(tileObj))
            {
                RectTransform tileRect = tileObj.GetComponent<RectTransform>();
                if (tileRect != null)
                {
                    Vector2 basePosition = tileBasePositions[tileObj];
                    Vector2 offset = _currentOffset;
                    
                    if (_scrollDirection.x != 0)
                    {
                        offset.x = offset.x % _effectiveTileSize.x;
                    }
                    
                    if (_scrollDirection.y != 0)
                    {
                        offset.y = offset.y % _effectiveTileSize.y;
                    }
                    
                    tileRect.anchoredPosition = basePosition + offset;
                }
            }
        }
    }
}
