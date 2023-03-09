using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class LifeController : MonoBehaviour
{
    public static LifeController Instance;

    public static Action OnInfiniteLife;

    public int Lives { get { return lives; } }
    public TMP_Text WaitTime { get { return timerLivesText; } set { timerLivesText = value; } }
    public bool InfiniteLife { get; set; }
    public int MaxLives { get { return maxLives; } }

    [Header("Life")]
    [SerializeField] int lives;
    [SerializeField] TMP_Text livesText;
    [SerializeField] GameObject imageInfiniteLife;

    [Header("Timer")]
    [SerializeField] float waitTimeInMinutes;
    [SerializeField] TMP_Text timerLivesText;

    int maxLives = 5;
    int minLives = 0;

    // remaining time in seconds left on the counter
    float timeRemainingInSeconds;

    // Boolean that tells us if the counter should be restarted
    bool restartTimer = false;
    int hours;
    int minutes;
    int seconds;

    void OnEnable()
    {
        OnInfiniteLife += ChangeInfiniteLife;
    }

    void OnDisable()
    {
        OnInfiniteLife -= ChangeInfiniteLife;
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        ChangeLives(0);
    }

    void Update()
    {

        if (restartTimer)
        {
            timeRemainingInSeconds -= Time.deltaTime;

            hours = Mathf.FloorToInt(timeRemainingInSeconds / 3600);
            minutes = Mathf.FloorToInt((timeRemainingInSeconds - hours * 3600) / 60);
            seconds = Mathf.FloorToInt(timeRemainingInSeconds % 60);

            if (hours > 0)
            {
                timerLivesText.text = string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
            }
            else
            {
                timerLivesText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            }

            if (timeRemainingInSeconds <= 0)
            {
                if (InfiniteLife)
                {
                    lives = maxLives;
                    OnInfiniteLife?.Invoke();
                    ChangeLives(maxLives);
                    return;
                }

                if (hours > 0) timerLivesText.text = string.Format("00:00:00");
                else timerLivesText.text = string.Format("00:00");
                ChangeLives(1);
            }
        }
    }

    // Method responsible for diminishing or changing lives
    public void ChangeLives(int amount)
    {
        lives = Mathf.Clamp(lives + amount, minLives, maxLives);
        livesText.text = lives.ToString();

        if (CheckRestartTimer())
        {
            SetTimer(waitTimeInMinutes);
            return;
        }

        timerLivesText.text = "FULL";
        restartTimer = false;
    }

    // Method in charge of configuring the timer
    public void SetTimer(float time)
    {
        restartTimer = true;
        timeRemainingInSeconds = time * 60;
    }

    // Method in charge of changing the UI to infinity and vice versa
    void ChangeInfiniteLife()
    {
        livesText.enabled = !livesText.isActiveAndEnabled;
        imageInfiniteLife.SetActive(!imageInfiniteLife.activeSelf);
        InfiniteLife = !InfiniteLife;
    }

    // Method tells us if the counter needs to be reset
    bool CheckRestartTimer() => lives < 5;
}