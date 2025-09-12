using System;
using UnityEngine;

public class Timer
{
    private float _currentTime;
    private float _setTime;
    private bool _counting;

    public Timer(float time=0)
    {
        _setTime = time;
    }
    public event Action TimerFinished;
    
    public void SetTimer(float time)
    {
        _setTime = time;
        _currentTime = time;
    }

    public void SetCurrentTime(float time)
    {
        _currentTime = time;
    }
    public void AddTime(float time)
    {
        _currentTime += time;
    }

    public void SubtractTime(float time)
    {
        _currentTime -= time;
    }

    public void StartTimer()
    {
        _counting = true;
    }

    public void StartTimer(float time)
    {
        SetTimer(time);
        StartTimer();
    }

    public void StopTimer()
    {
        _counting = false;
    }

    public void ResetTimer()
    {
        StopTimer();
        StartTimer(_setTime);
    }
    
    public float GetTime()
    {
        return _currentTime;
    }

    public float GetMaxTime()
    {
        return _setTime;
    }

    public float GetTimePercent()
    {
        return _currentTime / _setTime;
    }
    
    public void Count()
    {
        if (_counting)
        {
            _currentTime -= Time.deltaTime;
            if (_currentTime <= 0)
            {
                OnTimerFinished();
            }
        }
    }

    protected virtual void OnTimerFinished()
    {
        _currentTime = 0;
        _counting = false;
        TimerFinished?.Invoke();
    }
}