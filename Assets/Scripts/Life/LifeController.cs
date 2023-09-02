using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Firebase.Firestore;
using UnityEngine.UI;

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
    [SerializeField] Image imageInfiniteLife;

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

        DontDestroyOnLoad(gameObject);
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

    /// <summary>
    /// Changes the number of lives and updates UI accordingly.
    /// </summary>
    /// <param name="amount">The amount of lives to change by</param>
    /// <param name="isBuy">Whether the change is due to a purchase</param>
    public void ChangeLives(int amount, bool isBuy = false)
    {
        lives = Mathf.Clamp(lives + amount, minLives, maxLives);

        if (GameManager.Instance.currentGameState == GameState.LevelMenu)
            UpdateLivesUI();

        if ((amount < 0 && lives < 4) || isBuy)
        {
            SaveLivesDataBase((waitTimeInMinutes * 60) - timeRemainingInSeconds);

            if (isBuy && !CheckRestartTimer()) FullLives();
            return;
        }
        else SaveLivesDataBase();

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
    /// Checks if the player has lives.
    /// </summary>
    public bool HasLives => lives > 0;

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

    /// <summary>
    /// Toggles the Infinite Life feature and updates the UI accordingly.
    /// </summary>
    void ChangeInfiniteLife()
    {
        ChangeInfiniteLifeUI();
        IsInfinite = !IsInfinite;
    }

    /// <summary>
    /// Toggles the visibility of UI elements related to Infinite Life.
    /// </summary>
    void ChangeInfiniteLifeUI()
    {
        livesText.enabled = !livesText.isActiveAndEnabled;
        imageInfiniteLife.enabled = !imageInfiniteLife.isActiveAndEnabled;
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

                Dictionary<string, object> infiniteInfo = lives["infinite"] as Dictionary<string, object>;
                IsInfinite = (bool)infiniteInfo["is infinite"];

                // Get the current internet time and calculate the time difference for lives
                StartCoroutine(InternetTime.Instance.GetInternetTime(time =>
                {
                    currentTime = time;
                    Timestamp timestamp = (Timestamp)lives["last date"];
                    DateTime utcDate = timestamp.ToDateTime();
                    DateTime localDate = utcDate.ToLocalTime();
                    previouslyAllottedTime = localDate;
                    GetDifferenceLives(auxLives, Convert.ToInt32(infiniteInfo["time"]));
                }));
                return;
            }
        }

        SetInitialLives(maxLives, true);
    }

    /// <summary>
    /// Updates the lives UI element with the current lives count.
    /// </summary>
    public void UpdateLivesUI()
    {
        if (livesText == null) livesText = GameObject.FindGameObjectWithTag("Number Life").GetComponent<TMP_Text>();
        if (timerText == null) timerText = GameObject.FindGameObjectWithTag("Timer Life").GetComponent<TMP_Text>();
        if (imageInfiniteLife == null) imageInfiniteLife = GameObject.FindGameObjectWithTag("Infinite Life").GetComponent<Image>();

        if (livesText != null) livesText.text = lives.ToString();

        if (IsInfinite) ChangeInfiniteLifeUI();
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
    public void SaveLivesDataBase(float subtractTime = 0, float timeInfinite = 0)
    {
        if (Lives >= maxLives) subtractTime = 0;

        StartCoroutine(InternetTime.Instance.GetInternetTime(time =>
        {
            DateTime newTime = time.AddSeconds(-subtractTime);
            previouslyAllottedTime = newTime;
            Dictionary<string, object> infinite = new Dictionary<string, object> { { "is infinite", IsInfinite }, { "time", timeInfinite } };
            Dictionary<string, object> data = new Dictionary<string, object> { { "amount", this.lives }, { "last date", newTime }, { "infinite", infinite } };
            Dictionary<string, object> lives = new Dictionary<string, object> { { "lives", data } };
            CloudFirestore.Instance.SetCollectible(lives);
        }));
    }

    /// <summary>
    /// Calculates and applies the difference in lives based on time elapsed.
    /// </summary>
    /// <param name="auxLives">The auxiliary lives count.</param>
    /// <param name="timeInfinite">The remaining time for Infinite Life.</param>
    void GetDifferenceLives(int auxLives, int timeInfinite)
    {
        // Calculate the time difference between the current and previously allotted time.
        float timeDifference = (float)(currentTime.Subtract(previouslyAllottedTime)).TotalSeconds;

        if (timeInfinite == 0)
        {
            // Calculate the number of lives to add based on the elapsed time.
            int livesToAdd = (int)(timeDifference / (waitTimeInMinutes * 60));
            float remainingSeconds = timeDifference % (waitTimeInMinutes * 60);

            // Update the auxiliary lives count and determine if the database should be saved.
            auxLives += livesToAdd;
            bool saveDataBase = livesToAdd > 0;
            SetInitialLives(auxLives, saveDataBase, remainingSeconds);
        }
        else
        {
            // Toggle Infinite Life UI and set the timer for Infinite Life.
            ChangeInfiniteLifeUI();
            SetTimer(timeInfinite, timeDifference);
        }
    }
}