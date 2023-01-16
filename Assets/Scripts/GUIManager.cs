using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GUIManager : MonoBehaviour
{
    public static GUIManager Instance;

    public int MoveCounter { get { return moveCounter; } set { moveCounter = value; movesText.text = moveCounter.ToString(); } }
    public int Score { get { return score; } set { score = value; scoreText.text = score.ToString(); } }

    int moveCounter;
    int score;

    [SerializeField] TMP_Text movesText, scoreText;

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
        
        moveCounter = 30;
        movesText.text = moveCounter.ToString();
    }
}