using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimerGame : MonoBehaviour
{
    [SerializeField] TMP_Text timerText;

    float timeRemaining;

    int minutes;
    int seconds;

    /// <summary>
    /// Starts the timer coroutine.
    /// </summary>
    public void StartTimer()
    {
        StartCoroutine(UpdateTimer());
    }

    /// <summary>
    /// Coroutine for updating the timer UI.
    /// </summary>
    /// <returns>An IEnumerator for coroutine execution.</returns>
    IEnumerator UpdateTimer()
    {
        timeRemaining = GameManager.Instance.TotalSeconds;

        while (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;

            minutes = Mathf.FloorToInt(timeRemaining / 60);
            seconds = Mathf.FloorToInt(timeRemaining % 60);

            if (minutes > 0) timerText.text = string.Format("{0}:{1:00}", minutes, seconds);
            else timerText.text = string.Format("{0:00}", seconds);

            yield return null;
        }

        timerText.text = "00";

        yield return null;
    }
}