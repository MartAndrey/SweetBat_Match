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
    [SerializeField] GameObject[] fruitObjectGoals;
    [SerializeField] GameObject[] checkFruitsGoal;
    [SerializeField] GameObject objectPlus;
    [SerializeField] int maxObjectiveFruits;

    List<GameObject> availableFruits;

    int firstGoal = 0;
    int secondGoal = 1;

    // Constants for maximum and minimum goals
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

    void SetFeedingObjective()
    {
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

    /// <summary>
    /// Updates the UI with the next objective to complete.
    /// </summary>
    void NextObjective()
    {
        if (currentGoal == maxGoal) return;

        ResetStatusFruitGoal();

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
        int amountObjective = UnityEngine.Random.Range(1, fruitObjectGoals.Length + 1);

        // Deactivates the second objective UI game object if only one fruit is required to collect.
        if (amountObjective == 1)
        {
            fruitObjectGoals[secondGoal].SetActive(false);
            objectPlus.SetActive(false);
        }
        else
        {
            fruitObjectGoals[secondGoal].SetActive(true);
            objectPlus.SetActive(true);
        }

        // Sets the fruits to collect and their objective amounts for each objective UI game object.
        for (int i = 0; i < fruitObjectGoals.Length; i++)
        {
            if (fruitObjectGoals[i].activeSelf)
            {
                // Sets the sprite for the fruit to collect.
                fruitObjectGoals[i].GetComponentInChildren<Image>().sprite = GetRandomFruitsAvailable();
                // Sets the amount of the fruit to collect.
                fruitObjectGoals[i].GetComponentInChildren<TMP_Text>().text = GetRandomAmountFruitObjective().ToString();
            }
        }

        // Make sure the second goal is not the same as the first goal
        while (fruitObjectGoals[secondGoal].activeSelf && IsSameFruit(fruitObjectGoals[firstGoal], fruitObjectGoals[secondGoal]))
        {
            fruitObjectGoals[secondGoal].GetComponentInChildren<Image>().sprite = GetRandomFruitsAvailable();
        }

        // Check if this is the last goal and there are multiple objectives to set
        if (amountObjective > 1 && currentGoal == maxGoal) CheckQuantitiesObjectiveFruit();
    }

    /// <summary>
    /// Checks the quantities of the objectives and makes sure they add up to the correct amount.
    /// </summary>
    void CheckQuantitiesObjectiveFruit()
    {
        int amountFirst = Convert.ToInt32(fruitObjectGoals[firstGoal].GetComponentInChildren<TMP_Text>().text);
        int amountSecond = Convert.ToInt32(fruitObjectGoals[secondGoal].GetComponentInChildren<TMP_Text>().text);

        if (amountSecond == 0)
        {
            int amountRandom = UnityEngine.Random.Range(1, amountFirst);

            amountFirst -= amountRandom;
            amountSecond += amountRandom;

            fruitObjectGoals[firstGoal].GetComponentInChildren<TMP_Text>().text = amountFirst.ToString();
            fruitObjectGoals[secondGoal].GetComponentInChildren<TMP_Text>().text = amountSecond.ToString();
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
        int amountObjectiveToFruit = Mathf.CeilToInt(UnityEngine.Random.Range(1f, (float)maxObjectiveFruits) / remainingGoals);

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

    /// <summary>
    /// Check the amount of fruits needed to complete a given objective.
    /// </summary>
    /// <param name="listMatchesFruits">List of fruits to check against the objective.</param>
    public void CheckAmountObjective(List<GameObject> listMatchesFruits)
    {
        // Check if first goal is active and update if not
        if (!checkFruitsGoal[firstGoal].activeInHierarchy)
        {
            ChangeAmountGoals(listMatchesFruits, firstGoal);
        }

        // Check if second goal is active and update if not
        if (!checkFruitsGoal[secondGoal].activeInHierarchy)
        {
            ChangeAmountGoals(listMatchesFruits, secondGoal);
        }

        // Check if both goals have been completed and update the objective if necessary
        if (IsChangeObjective()) NextObjective();
    }

    /// <summary>
    /// Changes the amount of a given fruit objective and updates the corresponding UI element.
    /// </summary>
    /// <param name="listMatchesFruits">List of fruits to check against the objective.</param>
    /// <param name="fruitGoal">Index of the fruit objective to update.</param>
    void ChangeAmountGoals(List<GameObject> listMatchesFruits, int fruitGoal)
    {
        // Find all fruits that match the current objective and update the goal amount
        List<GameObject> fistFruitObjective = listMatchesFruits.FindAll(
               fruit => fruit.GetComponentInChildren<SpriteRenderer>().sprite == fruitObjectGoals[fruitGoal].GetComponentInChildren<Image>().sprite);

        if (ThereAreFruits(fistFruitObjective))
        {
            int amountGoal = Convert.ToInt32(fruitObjectGoals[fruitGoal].GetComponentInChildren<TMP_Text>().text);

            amountGoal -= fistFruitObjective.Count;

            // Update the UI element if the goal has been reached or exceeded
            if (amountGoal <= 0)
                ChangeStatusCheckFruitGoal(fruitObjectGoals[fruitGoal].GetComponentInChildren<TMP_Text>(), checkFruitsGoal[fruitGoal]);
            else fruitObjectGoals[fruitGoal].GetComponentInChildren<TMP_Text>().text = amountGoal.ToString();
        }
    }

    /// <summary>
    /// Checks if there are any fruits left to complete a given objective.
    /// </summary>
    /// <param name="listFruits">List of fruits to check.</param>
    /// <returns>True if there are fruits left, false otherwise.</returns>
    bool ThereAreFruits(List<GameObject> listFruits) => listFruits.Count > 0;

    /// <summary>
    /// Checks if the objective needs to be updated based on the completion status of the current objectives.
    /// </summary>
    /// <returns>True if the objective needs to be updated, false otherwise.</returns>
    bool IsChangeObjective() => (checkFruitsGoal[firstGoal].activeInHierarchy && checkFruitsGoal[secondGoal].activeInHierarchy && objectPlus.activeInHierarchy)
                                 || (checkFruitsGoal[firstGoal].activeInHierarchy && !objectPlus.activeInHierarchy && !checkFruitsGoal[secondGoal].activeInHierarchy);


    /// <summary>
    /// Resets the status of all fruit objectives.
    /// </summary>
    void ResetStatusFruitGoal()
    {
        int currentGoal = firstGoal;

        // Deactivate all fruit objectives and update their corresponding UI elements
        foreach (GameObject fruit in checkFruitsGoal)
        {
            if (fruit.activeSelf)
            {
                fruit.SetActive(false);
                fruitObjectGoals[currentGoal].GetComponentInChildren<TMP_Text>().enabled = true;
            }

            currentGoal++;
        }
    }

    /// <summary>
    /// Changes the status of the check mark and text for a fruit goal.
    /// </summary>
    /// <param name="textGoal">The text component for the fruit goal.</param>
    /// <param name="checkGoal">The check mark component for the fruit goal.</param>
    void ChangeStatusCheckFruitGoal(TMP_Text textGoal, GameObject checkGoal)
    {
        textGoal.enabled = textGoal.enabled == true ? false : true;
        checkGoal.SetActive(checkGoal.activeSelf ? false : true);
    }
}