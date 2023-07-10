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
        GetLives();
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

        FullLives();
    }

    /// <summary>
    /// Updates the UI to display "FULL" for lives.
    /// </summary>
    void FullLives()
    {
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

    /// <summary>
    /// Retrieves the number of lives from the CollectiblesData dictionary and sets the initial coins.
    /// If the lives data is not available, it sets the lives to 0 and initializes coins accordingly.
    /// </summary>
    void GetLives()
    {
        Dictionary<string, object> data = GameManager.Instance.CollectiblesData;
        if (data != null && data.Count > 0)
        {
            if (data.ContainsKey("lives"))
            {
                int lives = Convert.ToInt32(data["lives"]);

                SetInitialLives(lives);

                return;
            }
        }

        CloudFirestore.Instance.SetCollectible(new Dictionary<string, object> { { "lives", 0 } });
        SetInitialLives(0);
    }

    /// <summary>
    /// Sets the initial number of lives and updates the lives text UI element.
    /// If the restart timer condition is not met, calls FullLives method; otherwise, sets the timer.
    /// </summary>
    /// <param name="lives">The number of lives to set.</param>
    void SetInitialLives(int lives)
    {
        this.lives = lives;
        livesText.text = lives.ToString();

        if (!CheckRestartTimer()) FullLives();
        else SetTimer(waitTimeInMinutes);
    }
}