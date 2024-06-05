using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerSystem 
{

    #region Public Field

    public float StartTime { get { return _startTime; } }
    public float EndTime { get { return _endTime; } }
    public float CurrentTime { get { return _currentTime; } }
    public bool TimerEnded { get { return _timerEnded; } }

    #endregion


    #region Private Field
    public float _startTime;
    public float _endTime;
    public float _currentTime;
    public bool _timerEnded;
    #endregion


    public TimerSystem(float startTime) 
    {
        _startTime = startTime;    
    }


    public void TimerUpdate()
    {
        if (_timerEnded)
            return;

        _currentTime += Time.deltaTime;
    }


    

    public float StopTimer() 
    {        
        _timerEnded = true;
        _endTime = Time.time;
        return _currentTime;
    }



}
