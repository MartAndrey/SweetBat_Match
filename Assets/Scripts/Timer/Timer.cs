using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public abstract class Timer : MonoBehaviour
{
    // Property to check if the timer should run indefinitely or not
    public bool IsInfinite { get; set; }
    // Property to set the previously allotted time for the timer
    public DateTime PreviouslyAllottedTime { set { previouslyAllottedTime = value; } }

    [Header("Timer")]
    [SerializeField] protected float waitTimeInMinutes;
    [SerializeField] protected TMP_Text timerText;

    // remaining time in seconds left on the counter
    protected float timeRemainingInSeconds;

    protected int hours;
    protected int minutes;
    protected int seconds;

    // Variables to keep track of time
    protected DateTime previouslyAllottedTime;
    protected DateTime currentTime;

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

    // Coroutine to wait for the current time from the internet and subtract it from the timer
    protected IEnumerator WaitForGetCurrentTime()
    {
        yield return StartCoroutine(GetCurrentTime());

        SubtractTimeTimer();
    }

    // Coroutine to get the current time from the internet
    protected IEnumerator GetCurrentTime()
    {
        yield return StartCoroutine(InternetTime.Instance.GetInternetTime(currentTime =>
        {
            this.currentTime = currentTime;
        }));
    }

    // Method to subtract the time difference between the previously allotted time and the current time
    protected void SubtractTimeTimer()
    {
        float timeDifference = (float)(currentTime.Subtract(previouslyAllottedTime)).TotalSeconds;
        timeRemainingInSeconds -= timeDifference;
    }
}