using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LifeController : MonoBehaviour
{
    [Header("Life")]
    [SerializeField] int lives;
    [SerializeField] TMP_Text livesText;

    [Header("Timer")]
    [SerializeField] float waitTime;
    [SerializeField] TMP_Text timerLivesText;

    int maxLives = 5;
    int minLives = 0;

    // remaining time in seconds left on the counter
    float timeRemainingInSeconds;

    // Boolean that tells us if the counter should be restarted
    bool restartTimer = false;

    int minutes;
    int seconds;

    void Start()
    {
        ChangeLives(0);
    }

    void Update()
    {
        if (restartTimer)
        {
            timeRemainingInSeconds -= Time.deltaTime;
            minutes = Mathf.FloorToInt(timeRemainingInSeconds / 60);
            seconds = Mathf.FloorToInt(timeRemainingInSeconds % 60);

            timerLivesText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

            if (timeRemainingInSeconds <= 0)
            {
                timerLivesText.text = string.Format("00:00");
                ChangeLives(1);
            }
        }
    }

    // Method responsible for diminishing or changing lives
    void ChangeLives(int amount)
    {
        lives = Mathf.Clamp(lives + amount, minLives, maxLives);
        livesText.text = lives.ToString();

        if (CheckRestartTimer())
        {
            restartTimer = true;
            timeRemainingInSeconds = waitTime * 60;
            return;
        }

        timerLivesText.text = "FULL";
        restartTimer = false;
    }

    // Method tells us if the counter needs to be reset
    bool CheckRestartTimer() => lives < 5;
}