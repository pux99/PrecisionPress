using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    private List<Sprite> _forms=new List<Sprite>();
    private List<Color> _colors=new List<Color>();
    private List<Sprite> _patterns=new List<Sprite>();
    private List<Promt> promts = new List<Promt>();
    
    private Promt _currentPromt;
    private Piece _currentPiece=new Piece();
    private float _time;
    
    [SerializeField] Health health;
    [SerializeField] private List<Piece> pieces;
    [SerializeField] private Image Form;
    [SerializeField] private Image Pattern;
    [SerializeField] private TextMeshProUGUI PromtText;
    [SerializeField] MonoTimer timer;
    [SerializeField] float StartingTime;
    [SerializeField] float timeSubstraction;
    [SerializeField] private PointSystem pointSystem;
    
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
        _time = StartingTime;
        NextRound();
    }

    public void NextRound()
    {
        _currentPromt=promts[Random.Range(0, promts.Count)];
        PromtText.text="Follow The " + _currentPromt.ToString();
        NextPiece();
        timer.StartTimer(_time);
        _time-=timeSubstraction;
    }

    [ContextMenu("Next Piece")]
    public void NextPiece()
    {
        _currentPiece.form = _forms[Random.Range(0, _forms.Count)];
        _currentPiece.Color= _colors[Random.Range(0, _colors.Count)];
        _currentPiece.Pattern = _patterns[Random.Range(0, _patterns.Count)];
        Form.sprite = _currentPiece.form;
        Form.color = _currentPiece.Color;
        Pattern.sprite = _currentPiece.Pattern;
    }

    public void CheckPromt(Piece selected)
    {
        if (_currentPromt.Check(_currentPiece, selected))
        {
            Win();
        }
        else
        {
            Lose();
        }
    }

    private void Win()
    {
        pointSystem.Score();
        NextRound();
    }
    private void Lose()
    {
        health.TakeDamage();
        pointSystem.Fail();
        NextRound();
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