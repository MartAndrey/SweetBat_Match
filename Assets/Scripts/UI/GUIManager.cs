using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GUIManager : MonoBehaviour
{
    public static GUIManager Instance;

    public int Score
    {
        get { return score; }
        set
        {
            score = value;
            scoreText.text = score.ToString();
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
    [SerializeField] GameObject gameOverScreen;

    int moveCounter, score;

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

        score = 0;
        scoreText.text = score.ToString();

        moveCounter = 50;
        movesText.text = moveCounter.ToString();
    }

    IEnumerator GameOver()
    {
        yield return new WaitUntil(() => !BoardManager.Instance.isShifting);
        yield return new WaitForSeconds(0.25f);
        gameOverScreen.SetActive(true);
    }
}