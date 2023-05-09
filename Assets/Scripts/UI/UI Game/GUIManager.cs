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

    public int Score
    {
        get { return score; }
        set
        {
            score = value;
            StartCoroutine(UpdateScore());
            ProgressBar.Instance.ChangeBarScore(score);
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

    [SerializeField] TMP_Text movesText, scoreText, multiplicationFactorText;
    [SerializeField] GameObject imageInfiniteMoves;

    [Header("Screens")]
    [SerializeField] GameOverController menuGameOver;
    [SerializeField] CompleteGameController menuCompleteGame;
    [Header("UI")]
    // Serialized time bar UI element
    [SerializeField] GameObject timeBarUI;

    [SerializeField] int moveCounter, score;
    // Serialized timer fields
    [SerializeField] float timeToMatch, currentTime;

    void OnEnable()
    {
        GameManager.Instance.OnFeedingObjective.AddListener(UpdateStateGamePlayMode);
    }

    void OnDisable()
    {
        GameManager.Instance.OnFeedingObjective.RemoveListener(UpdateStateGamePlayMode);
    }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this.gameObject);
    }

    void Start()
    {
        scoreText.text = score.ToString();
        movesText.text = moveCounter.ToString();
    }

    /// <summary>
    /// Updates the game play mode and UI based on the randomly generated game play mode.
    /// </summary>
    public void UpdateStateGamePlayMode()
    {
        gamePlayMode = GameManager.Instance.GetRandomGamePlayMode();

        if (gamePlayMode == GamePlayMode.TimedMatch)
        {
            timeBarUI.SetActive(true);
            imageInfiniteMoves.SetActive(true);
            movesText.enabled = false;
        }
        else if (gamePlayMode == GamePlayMode.MovesLimited)
        {
            timeBarUI.SetActive(false);
            imageInfiniteMoves.SetActive(false);
            movesText.enabled = true;
        }
    }

    /// <summary>
    /// Wait until the board has finished changing before displaying the finished game or game completed menu.
    /// </summary>
    IEnumerator CheckGameStatus()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitUntil(() => !BoardManager.Instance.IsShifting);
        yield return new WaitForSeconds(0.3f);
        if (ProgressBar.Instance.GetActiveStars() >= 1) menuCompleteGame.OnCompleteGame();
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
            if (currentTime > timeToMatch) StartCoroutine(CheckGameStatus());
            yield return null;
        }

        yield return null;
    }
}