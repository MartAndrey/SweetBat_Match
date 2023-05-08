using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CharacterBatUI : MonoBehaviour
{
    [SerializeField] TMP_Text[] textAmountGoals;
    [SerializeField] GameObject[] ObjectGoals;

    int firstGoal = 0;
    int secondGoal = 1;

    const int MAX_GOAL = 5;
    const int MIN_GOAL = 1;
    int maxGoal;
    int currentGoal = 1;

    /// <summary>
    /// Sets the minimum and maximum goal amounts, and updates the UI with the current and maximum goal amounts.
    /// </summary>
    void GetGoals()
    {
        maxGoal = Random.Range(MIN_GOAL, MAX_GOAL + 1);
        textAmountGoals[firstGoal].text = currentGoal.ToString();
        textAmountGoals[secondGoal].text = maxGoal.ToString();
    }
}
