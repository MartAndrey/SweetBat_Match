using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Firebase.Firestore;

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
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
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

        SaveLivesDataBase();

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

    /// <summary>
    /// Sets a timer for a given time with an optional subtraction of time in seconds.
    /// </summary>
    /// <param name="time">The main time to set the timer for, in minutes.</param>
    /// <param name="timeInSecond">Additional time to subtract in seconds.</param>
    public void SetTimer(float time, float timeInSecond = 0)
    {
        restartTimer = true;
        timeRemainingInSeconds = time * 60;

        if (timeInSecond != 0)
            timeRemainingInSeconds -= timeInSecond;

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
    /// Retrieves lives data from the GameManager and processes it.
    /// </summary>
    void GetLives()
    {
        Dictionary<string, object> data = GameManager.Instance.CollectiblesData;

        if (data != null && data.Count > 0)
        {
            if (data.ContainsKey("lives"))
            {
                Dictionary<string, object> lives = data["lives"] as Dictionary<string, object>;
                int auxLives = Convert.ToInt32(lives["amount"]);

                // Get the current internet time and calculate the time difference for lives
                StartCoroutine(InternetTime.Instance.GetInternetTime(time =>
                {
                    currentTime = time;
                    Timestamp timestamp = (Timestamp)lives["Last Date"];
                    DateTime utcDate = timestamp.ToDateTime();
                    DateTime localDate = utcDate.ToLocalTime();
                    previouslyAllottedTime = localDate;
                    GetDifferenceLives(auxLives);
                }));
                return;
            }
        }

        int initialLives = maxLives;
        SetInitialLives(initialLives, true);
    }

    /// <summary>
    /// Sets initial lives, updates UI, and handles timer.
    /// </summary>
    /// <param name="lives">The initial lives to set.</param>
    /// <param name="saveDataBase">Indicates whether to save data to the database.</param>
    /// <param name="timeInSecond">Additional time to subtract in seconds.</param>
    void SetInitialLives(int lives, bool saveDataBase, float timeInSecond = 0)
    {
        // Clamp the number of lives between minLives and maxLives
        this.lives = Mathf.Clamp(lives, minLives, maxLives);
        livesText.text = this.lives.ToString();

        if (saveDataBase)
            SaveLivesDataBase(timeInSecond);

        if (!CheckRestartTimer()) FullLives();
        else SetTimer(waitTimeInMinutes, timeInSecond);
    }

    /// <summary>
    /// Saves lives data to the database with optional subtracted time.
    /// </summary>
    /// <param name="subtractTime">Time to subtract in seconds.</param>
    public void SaveLivesDataBase(float subtractTime = 0)
    {
        if (Lives >= maxLives) subtractTime = 0;

        StartCoroutine(InternetTime.Instance.GetInternetTime(time =>
        {
            DateTime newTime = time.AddSeconds(-subtractTime);
            previouslyAllottedTime = newTime;
            Dictionary<string, object> data = new Dictionary<string, object> { { "amount", this.lives }, { "Last Date", newTime } };
            Dictionary<string, object> lives = new Dictionary<string, object> { { "lives", data } };
            CloudFirestore.Instance.SetCollectible(lives);
        }));
    }

    /// <summary>
    /// Calculates and applies the difference in lives based on time elapsed.
    /// </summary>
    /// <param name="auxLives">The auxiliary lives count.</param>
    void GetDifferenceLives(int auxLives)
    {
        float timeDifference = (float)(currentTime.Subtract(previouslyAllottedTime)).TotalSeconds;
        int livesToAdd = (int)(timeDifference / (waitTimeInMinutes * 60));
        float remainingSeconds = timeDifference % (waitTimeInMinutes * 60);

        auxLives += livesToAdd;
        bool saveDataBase = livesToAdd > 0;
        SetInitialLives(auxLives, saveDataBase, remainingSeconds);
    }
}