using System;
using UnityEngine;

public class MonoTimer : MonoBehaviour
{
    private readonly Timer _timer = new Timer();
    public event Action TimerFinished;

    private void Start()
    {
        _timer.TimerFinished += OnTimerFinished;
    }

    public void SetTimer(float time)
    {
        _timer.SetTimer(time);
    }

    public void SetCurrentTime(float time)
    {
        _timer.SetTimer(time);
    }
    public void AddTime(float time)
    {
        _timer.AddTime(time);
    }

    public void SubtractTime(float time)
    {
        _timer.SubtractTime(time);
    }

    public void StartTimer()
    {
        _timer.StartTimer();
    }

    public void StartTimer(float time)
    {
        _timer.StartTimer(time);
    }

    public void StopTimer()
    {
        _timer.StopTimer();
    }

    public void ResetTimer()
    {
        _timer.ResetTimer();
    }
    
    public float GetTime()
    {
        return _timer.GetTime();
    }

    public float GetMaxTime()
    {
        return _timer.GetMaxTime();
    }

    public float GetTimePercent()
    {
        return _timer.GetTimePercent();
    }
    
    void Update()
    {
        _timer.Count();
    }

    protected virtual void OnTimerFinished()
    {
        TimerFinished?.Invoke();
    }
}