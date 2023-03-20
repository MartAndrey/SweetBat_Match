using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [Header("Timer")]
    [SerializeField] protected float waitTimeInMinutes;
    [SerializeField] protected TMP_Text timerText;

    // remaining time in seconds left on the counter
    protected float timeRemainingInSeconds;

    protected bool restartTimer = false;

    protected int hours;
    protected int minutes;
    protected int seconds;

    // Method in charge of updating the timer, subtracting 1 every second
    protected void UpdateTimer(Action OnCountdownFinished)
    {
        timeRemainingInSeconds -= Time.deltaTime;

        hours = Mathf.FloorToInt(timeRemainingInSeconds / 3600);
        minutes = Mathf.FloorToInt((timeRemainingInSeconds - hours * 3600) / 60);
        seconds = Mathf.FloorToInt(timeRemainingInSeconds % 60);

        if (hours > 0)
        {
            timerText.text = string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
        }
        else
        {
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }

        if (timeRemainingInSeconds <= 0)
        {
            OnCountdownFinished();
        }
    }
}