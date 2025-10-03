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
    public bool IsTileable = false;
    [Range(0.1f, 10f)]
    public float FormTileScale = 1f;

    private GameObject tilesContainer;
    private List<GameObject> tileObjects = new List<GameObject>();

    public void Change(Sprite form, Color color, Sprite pattern)
    {
        _changeCounter++;
        if (_changeCounter >= ChangeFrequency)
        {
            _changeCounter = 0;
            
            if (IsTileable)
            {
                CreateTiledPiecesWithPatterns(form, color, pattern);
            }
            else
            {
                // Comportamiento normal para piezas no tileables
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
    
    private void CreateTiledPiecesWithPatterns(Sprite form, Color color, Sprite pattern)
    {
        if (Form == null) return;
        
        // Limpiar tiles anteriores
        ClearTiles();
        
        // Crear contenedor si no existe
        if (tilesContainer == null)
        {
            tilesContainer = new GameObject("TilesContainer");
            RectTransform containerRect = tilesContainer.AddComponent<RectTransform>();
            containerRect.SetParent(Form.transform.parent, false);
            
            // Configurar el contenedor para que ocupe todo el espacio
            containerRect.anchorMin = Vector2.zero;
            containerRect.anchorMax = Vector2.one;
            containerRect.offsetMin = Vector2.zero;
            containerRect.offsetMax = Vector2.zero;
            containerRect.pivot = new Vector2(0.5f, 0.5f);
            
            // Asegurar que esté en la misma posición en la jerarquía
            int formIndex = Form.transform.GetSiblingIndex();
            containerRect.SetSiblingIndex(formIndex);
        }
        
        // Ocultar las imágenes originales
        Form.enabled = false;
        Pattern.enabled = false;
        
        // Obtener el tamaño del contenedor
        RectTransform parentRect = Form.GetComponent<RectTransform>();
        float containerWidth = parentRect.rect.width;
        float containerHeight = parentRect.rect.height;
        
        // Calcular el tamaño de cada tile
        float spriteWidth = form.rect.width;
        float spriteHeight = form.rect.height;
        float pixelsPerUnit = form.pixelsPerUnit;
        
        float tileWidth = (spriteWidth / pixelsPerUnit) * 100f / FormTileScale;
        float tileHeight = (spriteHeight / pixelsPerUnit) * 100f / FormTileScale;
        
        // Calcular cuántos tiles necesitamos
        int tilesX = Mathf.CeilToInt(containerWidth / tileWidth) + 1;
        int tilesY = Mathf.CeilToInt(containerHeight / tileHeight) + 1;
        
        // Calcular el offset inicial para centrar
        float startX = -(tilesX * tileWidth) / 2f + tileWidth / 2f;
        float startY = -(tilesY * tileHeight) / 2f + tileHeight / 2f;
        
        // Crear cada tile
        for (int y = 0; y < tilesY; y++)
        {
            for (int x = 0; x < tilesX; x++)
            {
                GameObject tileObj = new GameObject($"Tile_{x}_{y}");
                RectTransform tileRect = tileObj.AddComponent<RectTransform>();
                tileRect.SetParent(tilesContainer.transform, false);
                
                // Configurar posición y tamaño del tile
                tileRect.sizeDelta = new Vector2(tileWidth, tileHeight);
                tileRect.anchoredPosition = new Vector2(
                    startX + x * tileWidth,
                    startY + y * tileHeight
                );
                tileRect.pivot = new Vector2(0.5f, 0.5f);
                
                // Agregar imagen de forma
                Image formImage = tileObj.AddComponent<Image>();
                formImage.sprite = form;
                formImage.color = color;
                formImage.type = Image.Type.Simple;
                formImage.raycastTarget = false;
                
                // Crear patrón como hijo
                GameObject patternObj = new GameObject("Pattern");
                RectTransform patternRect = patternObj.AddComponent<RectTransform>();
                patternRect.SetParent(tileRect, false);
                
                // El patrón ocupa todo el tile
                patternRect.anchorMin = Vector2.zero;
                patternRect.anchorMax = Vector2.one;
                patternRect.offsetMin = Vector2.zero;
                patternRect.offsetMax = Vector2.zero;
                patternRect.pivot = new Vector2(0.5f, 0.5f);
                
                Image patternImage = patternObj.AddComponent<Image>();
                patternImage.sprite = pattern;
                patternImage.type = Image.Type.Simple;
                patternImage.raycastTarget = false;
                
                tileObjects.Add(tileObj);
            }
        }
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
    }
    
    public void ForceChange(Sprite form, Color color, Sprite pattern)
    {
        _changeCounter = 0;
        
        if (IsTileable)
        {
            CreateTiledPiecesWithPatterns(form, color, pattern);
        }
        else
        {
            // Comportamiento normal para piezas no tileables
            Form.sprite = form;
            Form.color = color;
            Form.type = Image.Type.Simple;
            Pattern.sprite = pattern;
            Pattern.type = Image.Type.Simple;
            
            // Asegurar que las imágenes originales estén visibles
            Form.enabled = true;
            Pattern.enabled = true;
            
            // Limpiar tiles si existen
            ClearTiles();
        }
    }
}