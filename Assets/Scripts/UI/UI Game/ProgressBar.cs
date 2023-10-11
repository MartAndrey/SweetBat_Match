using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    public static ProgressBar Instance;

    [SerializeField] Slider slider;
    [SerializeField] GameObject[] stars;
    [Tooltip("Score to achieve the goal")]
    [SerializeField] int scoreGoal;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
        scoreGoal = GameManager.Instance.ScoreBar;
    }

    // Method in charge of updating the score bar and activating the stars if necessary
    public void ChangeBarScore(int currentScore)
    {
        float percent = (float)currentScore / (float)scoreGoal; // It returns a value between 0.0 and 1

        slider.value = percent;

        switch (percent)
        {
            case float n when ((n >= 0.25f && n < 0.6f) && (!stars[0].activeSelf)): // activate the first star if 50% is exceeded
                stars[0].SetActive(true);
                break;
            case float n when ((n >= 0.6f && n < 1) && (!stars[0].activeSelf || !stars[1].activeSelf)): // activate the second star if 70% is exceeded
                stars[0].SetActive(true);
                stars[1].SetActive(true);
                break;
            case float n when ((n >= 1) && (!stars[0].activeSelf || !stars[1].activeSelf || !stars[2].activeSelf)): // activate the third star if 100% is reached
                stars[0].SetActive(true);
                stars[1].SetActive(true);
                stars[2].SetActive(true);
                GUIManager.Instance.CompleteTimeToMatchObjective();
                break;
        }
    }

    /// <summary>
    /// Returns the number of active stars in the progress bar
    /// </summary>
    /// <returns>Number of active stars</returns>
    public int GetActiveStars()
    {
        int activeStars = 0;

        // Check the number of active stars in the array
        for (int i = 0; i < stars.Length; i++)
        {
            if (stars[i].activeSelf) activeStars++;
        }

        return activeStars;
    }
}