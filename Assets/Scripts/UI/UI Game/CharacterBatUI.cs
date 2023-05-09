using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class CharacterBatUI : MonoBehaviour
{
    [SerializeField] GameObject amountsGoalsObject;
    [SerializeField] TMP_Text[] textAmountGoals;
    [SerializeField] GameObject[] objectGoals;
    [SerializeField] GameObject objectPlus;
    [SerializeField] int maxObjectiveFruits;

    List<GameObject> availableFruits;

    int firstGoal = 0;
    int secondGoal = 1;

    const int MAX_GOAL = 5;
    const int MIN_GOAL = 1;
    int maxGoal;
    int remainingGoals;
    int currentGoal;


    void OnEnable()
    {
        GameManager.Instance.OnFeedingObjective.AddListener(SetFeedingObjective);
    }

    void OnDisable()
    {
        GameManager.Instance.OnFeedingObjective.RemoveListener(SetFeedingObjective);
    }
    [ContextMenu("Set Objective")]
    void SetFeedingObjective()
    {
        Debug.ClearDeveloperConsole();
        // Gets the list of available fruits from the game manager.
        availableFruits = GameManager.Instance.AvailableFruits;
        // Activates the UI game object to show the current and maximum goal amounts.
        amountsGoalsObject.SetActive(true);

        // Sets the minimum and maximum goal amounts.
        GetGoals();

        // Sets the fruits to collect and their objective amounts.
        SetFruitsObjective();
    }

    /// <summary>
    /// Sets the minimum and maximum goal amounts, and updates the UI with the current and maximum goal amounts.
    /// </summary>
    void GetGoals()
    {
        // Generates a random number between MIN_GOAL and MAX_GOAL (inclusive) for the maximum goal amount.
        maxGoal = UnityEngine.Random.Range(MIN_GOAL, MAX_GOAL + 1);
        // Sets the remaining goals to the maximum goal amount.
        remainingGoals = maxGoal;
        // Sets the current goal to the minimum goal amount.
        currentGoal = MIN_GOAL;

        // Updates the UI with the current and maximum goal amounts.
        textAmountGoals[firstGoal].text = currentGoal.ToString();
        textAmountGoals[secondGoal].text = maxGoal.ToString();
    }
    [ContextMenu("Next Objective")]
    /// <summary>
    /// Updates the UI with the next objective to complete.
    /// </summary>
    void NextObjective()
    {
        Debug.ClearDeveloperConsole();
        // Increases the current goal by one and decreases the remaining goals by one.
        currentGoal++;
        remainingGoals--;
        // Updates the UI with the current goal amount.
        textAmountGoals[firstGoal].text = currentGoal.ToString();

        // Sets the fruits to collect and their objective amounts.
        SetFruitsObjective();
    }

    /// <summary>
    /// Sets the fruits to collect and their objective amounts, and updates the UI with the information.
    /// </summary>
    void SetFruitsObjective()
    {
        // Generates a random number between 1 and the number of objectives (inclusive) for the amount of fruits to collect.
        int amountObjective = UnityEngine.Random.Range(1, objectGoals.Length + 1);

        // Deactivates the second objective UI game object if only one fruit is required to collect.
        if (amountObjective == 1)
        {
            objectGoals[secondGoal].SetActive(false);
            objectPlus.SetActive(false);
        }
        else
        {
            objectGoals[secondGoal].SetActive(true);
            objectPlus.SetActive(true);
        }

        // Sets the fruits to collect and their objective amounts for each objective UI game object.
        for (int i = 0; i < objectGoals.Length; i++)
        {
            if (objectGoals[i].activeSelf)
            {
                // Sets the sprite for the fruit to collect.
                objectGoals[i].GetComponentInChildren<Image>().sprite = GetRandomFruitsAvailable();
                // Sets the amount of the fruit to collect.
                objectGoals[i].GetComponentInChildren<TMP_Text>().text = GetRandomAmountFruitObjective().ToString();
            }
        }

        // Make sure the second goal is not the same as the first goal
        while (objectGoals[secondGoal].activeSelf && IsSameFruit(objectGoals[firstGoal], objectGoals[secondGoal]))
        {
            objectGoals[secondGoal].GetComponentInChildren<Image>().sprite = GetRandomFruitsAvailable();
        }

        // Check if this is the last goal and there are multiple objectives to set
        if (amountObjective > 1 && currentGoal == maxGoal) CheckQuantitiesObjectiveFruit();
    }

    /// <summary>
    /// Checks the quantities of the objectives and makes sure they add up to the correct amount.
    /// </summary>
    void CheckQuantitiesObjectiveFruit()
    {
        int amountFirst = Convert.ToInt32(objectGoals[firstGoal].GetComponentInChildren<TMP_Text>().text);
        int amountSecond = Convert.ToInt32(objectGoals[secondGoal].GetComponentInChildren<TMP_Text>().text);

        if (amountSecond == 0)
        {
            int amountRandom = UnityEngine.Random.Range(1, amountFirst + 1);

            amountFirst -= amountRandom;
            amountSecond = amountRandom;

            objectGoals[firstGoal].GetComponentInChildren<TMP_Text>().text = amountFirst.ToString();
            objectGoals[secondGoal].GetComponentInChildren<TMP_Text>().text = amountSecond.ToString();
        }
    }

    /// <summary>
    /// Returns a random fruit sprite that is available to use for an objective.
    /// </summary>
    /// <returns>A random fruit sprite.</returns>
    Sprite GetRandomFruitsAvailable()
    {
        int fruitRandom = UnityEngine.Random.Range(0, availableFruits.Count);

        return availableFruits[fruitRandom].GetComponentInChildren<SpriteRenderer>().sprite;
    }

    /// <summary>
    /// Returns a random amount of fruits to be collected for an objective.
    /// </summary>
    /// <returns>A random amount of fruits.</returns>
    int GetRandomAmountFruitObjective()
    {
        int amountObjectiveToFruit = Mathf.CeilToInt(UnityEngine.Random.Range(1, maxObjectiveFruits + 1) / remainingGoals);
        maxObjectiveFruits -= amountObjectiveToFruit;

        if (currentGoal == maxGoal)
        {
            amountObjectiveToFruit += maxObjectiveFruits;
            maxObjectiveFruits -= maxObjectiveFruits;
        };

        return amountObjectiveToFruit;
    }

    /// <summary>
    /// Checks if two game objects have the same fruit image.
    /// </summary>
    /// <param name="firstFruit">The first game object to compare.</param>
    /// <param name="secondFruits">The second game object to compare.</param>
    /// <returns>True if both game objects have the same fruit image; false otherwise.</returns>
    bool IsSameFruit(GameObject firstFruit, GameObject secondFruits) =>
        firstFruit.GetComponentInChildren<Image>().sprite == secondFruits.GetComponentInChildren<Image>().sprite;
}