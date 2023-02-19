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

    // public int MultiplicationFactor
    // {
    //     get { return multiplicationFactor; }
    //     set
    //     {
    //         multiplicationFactorGameObject.SetActive(true);
    //         multiplicationFactor = value;
    //         probabilityFactorText.text = multiplicationFactor.ToString();
    //     }
    // }

    [SerializeField] TMP_Text movesText, scoreText, probabilityFactorText;
    [SerializeField] GameObject multiplicationFactorGameObject;

    [Header("Screens")]
    [SerializeField] GameObject gameOverScreen;

    int moveCounter, score/*, multiplicationFactor*/;

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

        moveCounter = 25;
        movesText.text = moveCounter.ToString();

        // multiplicationFactor = 0;
    }

    IEnumerator GameOver()
    {
        yield return new WaitUntil(() => !BoardManager.Instance.isShifting);
        yield return new WaitForSeconds(0.25f);
        gameOverScreen.SetActive(true);
    }
}