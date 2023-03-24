using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class LifeController : Timer
{
    public static LifeController Instance;

    public static Action OnInfiniteLife;

    public int Lives { get { return lives; } }
    public TMP_Text WaitTime { get { return timerText; } set { timerText = value; } }
    public int MaxLives { get { return maxLives; } }

    [Header("Life")]
    [SerializeField] int lives;
    [SerializeField] TMP_Text livesText;
    [SerializeField] GameObject imageInfiniteLife;

    int maxLives = 5;
    int minLives = 0;

    bool restartTimer = false;

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
            UpdateTimer(OnCountdownFinished);
        }
    }

    // This method is called when the counter or timer reaches zero.
    void OnCountdownFinished()
    {
        if (IsInfinite)
        {
            lives = maxLives;
            OnInfiniteLife?.Invoke();
            ChangeLives(maxLives);
            return;
        }

        if (hours > 0) timerText.text = string.Format("00:00:00");
        else timerText.text = string.Format("00:00");
        ChangeLives(1);
    }

    // Method responsible for diminishing or changing lives
    public void ChangeLives(int amount, bool isBuy = false)
    {
        lives = Mathf.Clamp(lives + amount, minLives, maxLives);
        livesText.text = lives.ToString();

        if (CheckRestartTimer())
        {
            if (!isBuy) SetTimer(waitTimeInMinutes);
            return;
        }

        timerText.text = "FULL";
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
        IsInfinite = !IsInfinite;
    }

    // Method tells us if the counter needs to be reset
    bool CheckRestartTimer() => lives < 5;
}