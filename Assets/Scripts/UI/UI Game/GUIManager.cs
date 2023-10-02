using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class GUIManager : MonoBehaviour
{
    public static GUIManager Instance;

    public GamePlayMode GamePlayMode { get { return gamePlayMode; } set { gamePlayMode = value; } }
    public float CurrentTime { get { return currentTime; } set { currentTime = value; } }
    public TimerGame TimerGame { get { return bannerTime.GetComponent<TimerGame>(); } }

    public int Score
    {
        get { return score; }
        set
        {
            score = value;
            StartCoroutine(UpdateScore());
            ProgressBar.Instance.ChangeBarScore(score);
            if (GameManager.Instance.GameMode == GameMode.ScoringObjective)
            {
                StopCoroutine(characterBatUI.RemainingScore());
                StartCoroutine(characterBatUI.RemainingScore());
            }
        }
    }

    public int MoveCounter
    {
        get { return moveCounter; }
        set
        {
            moveCounter = value;
            movesText.text = moveCounter.ToString();
            if (moveCounter <= 0)
            {
                moveCounter = 0;
                StartCoroutine(CheckGameStatus());
            }
        }
    }

    public int MultiplicationFactor { set { multiplicationFactorText.text = value.ToString(); } }

    [SerializeField] GamePlayMode gamePlayMode;

    [SerializeField] GameObject bannerMove;
    [SerializeField] GameObject bannerTime;

    [SerializeField] TMP_Text movesText, scoreText, multiplicationFactorText;
    [SerializeField] GameObject imageInfiniteMoves;

    [Header("Screens")]
    [SerializeField] GameOverController menuGameOver;
    [SerializeField] CompleteGameController menuCompleteGame;
    [Header("UI")]
    // Serialized time bar UI element
    [SerializeField] GameObject timeBarUI;
    [SerializeField] GameObject multiplicationFactor;
    [SerializeField] CharacterBatUI characterBatUI;

    int moveCounter, score;
    float timeToMatch, currentTime;

    // Dictionary to map game modes to corresponding objective setting methods.
    Dictionary<GameMode, Action> gameModeHandlers;

    void OnEnable()
    {
        GameManager.Instance.OnGameMode.AddListener(OnGameMode);
    }

    void OnDisable()
    {
        GameManager.Instance.OnGameMode.RemoveListener(OnGameMode);
    }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this.gameObject);

        moveCounter = GameManager.Instance.MoveCounter;
        timeToMatch = GameManager.Instance.TimeToMatch;

        gameModeHandlers = new Dictionary<GameMode, Action>()
        {
            { GameMode.FeedingObjective, SetFeedingObjective },
            { GameMode.ScoringObjective, SetScoringObjective },
            { GameMode.TimeObjective, SetTimeObjective },
            { GameMode.CollectionObjective, SetCollectionObjective }
        };
    }

    void Start()
    {
        scoreText.text = score.ToString();
        movesText.text = moveCounter.ToString();
    }

    void OnGameMode(GameMode gameMode)
    {
        if (gameModeHandlers.ContainsKey(gameMode)) gameModeHandlers[gameMode]();
    }

    /// <summary>
    /// Sets the feeding objective for the game mode.
    /// </summary>
    void SetFeedingObjective()
    {
        UpdateStateGamePlayMode();
    }

    /// <summary>
    /// Sets the scoring objective for the game mode.
    /// </summary>
    void SetScoringObjective()
    {
        UpdateStateGamePlayMode();
        multiplicationFactor.SetActive(true);
    }

    /// <summary>
    /// Sets the game's play mode to Timed Match and activates the necessary UI elements.
    /// </summary>
    void SetTimeObjective()
    {
        gamePlayMode = GamePlayMode.TimedMatch;
        bannerTime.SetActive(true);
        timeBarUI.SetActive(true);
    }

    void SetCollectionObjective()
    {
        gamePlayMode = GamePlayMode.MovesLimited;
        MovesLimited();
    }

    /// <summary>
    /// Updates the game play mode and UI based on the randomly generated game play mode.
    /// </summary>
    public void UpdateStateGamePlayMode()
    {
        bannerMove.SetActive(true);

        gamePlayMode = GameManager.Instance.GetRandomGamePlayMode();

        if (gamePlayMode == GamePlayMode.TimedMatch)
        {
            timeBarUI.SetActive(true);
            imageInfiniteMoves.SetActive(true);
            movesText.enabled = false;
        }
        else if (gamePlayMode == GamePlayMode.MovesLimited)
        {
            MovesLimited();
        }
    }

    /// <summary>
    /// Disables unnecessary UI elements and enables the move-related UI elements.
    /// </summary>
    void MovesLimited()
    {
        bannerTime.SetActive(false);
        timeBarUI.SetActive(false);
        imageInfiniteMoves.SetActive(false);
        bannerMove.SetActive(true);
        movesText.enabled = true;
    }

    /// <summary>
    /// Wait until the board has finished changing before displaying the finished game or game completed menu.
    /// </summary>
    public IEnumerator CheckGameStatus()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitUntil(() => !BoardManager.Instance.IsShifting);
        yield return new WaitForSeconds(0.3f);

        if (ProgressBar.Instance.GetActiveStars() >= 1 && GameManager.Instance.ObjectiveComplete)
            menuCompleteGame.OnCompleteGame();
        else menuGameOver.OnGameOver();
    }

    /// <summary>
    /// Updates the score display with a smooth animation.
    /// </summary>
    IEnumerator UpdateScore()
    {
        if (gamePlayMode == GamePlayMode.TimedMatch) currentTime = 0;

        int scoreDisplay = Convert.ToInt32(scoreText.text);

        while (scoreDisplay < score)
        {
            scoreDisplay++;
            scoreText.text = scoreDisplay.ToString();
            yield return new WaitForSeconds(0.01f);
        }
        yield return null;
    }

    /// <summary>
    /// Coroutine to update the time to match UI.
    /// </summary>
    public IEnumerator TimeToMatchCoroutine()
    {
        float factor;

        while (currentTime < timeToMatch)
        {
            currentTime += Time.deltaTime;
            factor = Mathf.Clamp(currentTime / timeToMatch, 0, 1);
            UITimeBar.Instance.ChangeTimeBar(factor);

            if (currentTime > timeToMatch)
            {
                yield return new WaitUntil(() => !BoardManager.Instance.IsShifting);

                if (currentTime >= timeToMatch) StartCoroutine(CheckGameStatus());
            }

            yield return null;
        }

        yield return null;
    }

    /// <summary>
    /// Changes the 'timeToMatch' value, clamping it between 3 and the GameManager's 'TimeToMatch' property.
    /// </summary>
    /// <param name="amount">The amount by which to adjust the 'timeToMatch' value.</param>
    public void ChangeTimeToMatch(float amount)
    {
        // Calculate the new 'timeToMatch' value and clamp it to the desired range.
        timeToMatch = Math.Clamp(timeToMatch += amount, 3, GameManager.Instance.TimeToMatch);

        // Check if 'timeToMatch' is equal to the GameManager's 'TimeToMatch' value and update 'IsTimeToMatchPenalty' accordingly.
        if (timeToMatch == GameManager.Instance.TimeToMatch)
            GameManager.Instance.IsTimeToMatchPenalty = false;
    }

    /// <summary>
    /// Checks whether the game objective is complete and whether the player earned at least three stars, then starts the CheckGameStatus coroutine.
    /// </summary>
    public void CompleteTimeToMatchObjective()
    {
        // Check whether the game objective is complete, the player earned at least three stars, and the game mode is TimedMatch.
        if (GameManager.Instance.ObjectiveComplete && ProgressBar.Instance.GetActiveStars() >= 3)
            StartCoroutine(CheckGameStatus());
    }
}