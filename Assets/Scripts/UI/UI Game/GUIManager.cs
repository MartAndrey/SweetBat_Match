using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

/// <summary>
/// Manages the graphical user interface (GUI) of the game.
/// </summary>
public class GUIManager : MonoBehaviour
{
    public static GUIManager Instance;

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
                StartCoroutine(GameOver());
            }
        }
    }

    public int MultiplicationFactor { set { multiplicationFactorText.text = value.ToString(); } }

    [SerializeField] TMP_Text movesText, scoreText, multiplicationFactorText;

    [Header("Screens")]
    [SerializeField] GameObject menuGameOver;

    [SerializeField] int moveCounter, score;

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        scoreText.text = score.ToString();
        movesText.text = moveCounter.ToString();
    }

    /// <summary>
    /// Waits until the board has finished shifting before showing the game over menu.
    /// </summary>
    IEnumerator GameOver()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitUntil(() => !BoardManager.Instance.IsShifting);
        yield return new WaitForSeconds(0.25f);

        menuGameOver.GetComponent<GameOverController>().OnGameOver();
    }

    /// <summary>
    /// Updates the score display with a smooth animation.
    /// </summary>
    IEnumerator UpdateScore()
    {
        int scoreDisplay = Convert.ToInt32(scoreText.text);

        while (scoreDisplay < score)
        {
            scoreDisplay++;
            scoreText.text = scoreDisplay.ToString();
            yield return new WaitForSeconds(0.01f);
        }
        yield return null;
    }
}