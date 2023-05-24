using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using DG.Tweening;

public class CharacterBatUI : MonoBehaviour
{
    [Header("Feeding Objective")]
    [SerializeField] GameObject objectFeedingObjective;
    [SerializeField] GameObject amountsGoalsObject;
    [SerializeField] TMP_Text[] textAmountGoals;
    [SerializeField] GameObject[] fruitObjectGoals;
    [SerializeField] GameObject[] checkFruitsGoal;
    [SerializeField] GameObject objectPlus;

    [Header("Scoring Objective")]
    [SerializeField] GameObject objectScoringObjective;
    [SerializeField] TMP_Text remainingScoreText;

    [Header("Time Objective")]
    [SerializeField] GameObject objectTimeObjective;
    [SerializeField] TMP_Text remainingMatchText;

    [Header("Collection Objective")]
    [SerializeField] GameObject objectCollectionObjective;
    [SerializeField] GameObject fruitCollectionGoal;
    [SerializeField] GameObject checkFruitCollectionGoal;

    Dictionary<GameMode, Action> gameModeHandlers;

    //========================Feeding Objective===========================//
    int maxObjectiveFruits;

    // List of available fruit GameObjects to choose from.
    List<GameObject> availableFruits;

    AudioSource audioSource;

    // Indices for the first and second goals in the array of goals.
    int firstGoal = 0;
    int secondGoal = 1;

    // Constants for maximum and minimum goals
    const int MAX_GOAL = 5;
    const int MIN_GOAL = 1;

    // Integer variables for the maximum number of goals, remaining goals, and current goal.
    int maxGoal;
    int remainingGoals;
    int currentGoal;
    // Boolean variable to track whether the current goal has been completed.
    bool goalComplete;

    bool isAnimationChangeObjective;

    //========================Scoring Objective===========================//
    int remainingScoreObjective;

    //========================Time Objective===========================//
    int remainingMatch;

    //========================Collection Objective===========================//
    int amountFruitCollection;

    void OnEnable()
    {
        GameManager.Instance.OnGameMode.AddListener(OnGameMode);

        if (GameManager.Instance.GameMode == GameMode.TimeObjective)
            GameManager.Instance.OnUniqueMatches.AddListener(RemainingMatchObjective);
    }

    void OnDisable()
    {
        GameManager.Instance.OnGameMode.RemoveListener(OnGameMode);

        if (GameManager.Instance.GameMode == GameMode.TimeObjective)
            GameManager.Instance.OnUniqueMatches.RemoveListener(RemainingMatchObjective);
    }

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        gameModeHandlers = new Dictionary<GameMode, Action>()
        {
            { GameMode.FeedingObjective, SetFeedingObjective },
            { GameMode.ScoringObjective, SetScoringObjective },
            { GameMode.TimeObjective, SetTimeObjective },
            { GameMode.CollectionObjective, SetCollectionObjective },
        };
    }

    void OnGameMode(GameMode gameMode)
    {
        if (gameModeHandlers.ContainsKey(gameMode)) gameModeHandlers[gameMode]();
    }

    void SetFeedingObjective()
    {
        GameManager.Instance.ObjectiveComplete = false;

        objectFeedingObjective.SetActive(true);

        maxObjectiveFruits = GameManager.Instance.MaxFeedingObjective;
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
    /// Activates the scoring objective game object and sets the maximum score objective.
    /// </summary>
    void SetScoringObjective()
    {
        GameManager.Instance.ObjectiveComplete = false;

        // Activate the scoring objective game object.
        objectScoringObjective.SetActive(true);
        // Set the maximum score objective based on the GameManager's MaxScoreObjective property.
        remainingScoreObjective = GameManager.Instance.MaxScoreObjective;
        // Set the remaining score text to display the maximum score objective as a string.
        remainingScoreText.text = remainingScoreObjective.ToString();
    }

    /// <summary>
    /// Activates the time objective object, resets the objective completion flag,
    /// and updates the remaining match count on the UI.
    /// </summary>
    void SetTimeObjective()
    {
        objectTimeObjective.SetActive(true);
        GameManager.Instance.ObjectiveComplete = false;
        remainingMatch = GameManager.Instance.MatchObjectiveAmount;
        remainingMatchText.text = remainingMatch.ToString();
    }

    /// <summary>
    /// Sets the collection objective for fruits.
    /// </summary>
    void SetCollectionObjective()
    {
        // Activate the collection objective object
        objectCollectionObjective.SetActive(true);
        // Get the fruit collection amount from the game manager
        amountFruitCollection = GameManager.Instance.FruitCollectionAmount;
        // Start the coroutine to set the collection objective
        StartCoroutine(SetCollectionObjectiveRutiner());
    }

    #region Feeding Objective

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
            fruitObjectGoals[secondGoal].transform.parent.gameObject.SetActive(false);
            objectPlus.SetActive(false);
        }
        else
        {
            fruitObjectGoals[secondGoal].transform.parent.gameObject.SetActive(true);
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

                EnterAnimateGoal(fruitObjectGoals[i]);
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
    public IEnumerator CheckAmountObjective(List<GameObject> listMatchesFruits)
    {
        if (isAnimationChangeObjective) yield break;

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
        if (IsChangeObjective())
        {
            // If the current goal is not yet complete, play an audio cue and mark the goal as complete if it is the last goal.
            if (!goalComplete)
            {
                audioSource.Play();

                if (currentGoal == maxGoal)
                {
                    GameManager.Instance.ObjectiveComplete = true;
                    goalComplete = true;
                    GUIManager.Instance.CompleteTimeToMatchObjective();
                }
            }

            // Animate the exit of all active goals, wait for a short period of time, then move on to the next objective.
            ExitAnimateGoal(fruitObjectGoals);

            yield return new WaitForSeconds(.4f);

            NextObjective();
        }
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

    /// <summary>
    /// Animate the entrance of a goal with a slide up and fade in effect.
    /// </summary>
    /// <param name="goal">The goal GameObject to animate.</param>
    void EnterAnimateGoal(GameObject goal)
    {
        // Get the RectTransform and CanvasGroup components of the goal GameObject.
        RectTransform goalRectTransform = goal.GetComponent<RectTransform>();
        CanvasGroup goalCanvasGroup = goal.GetComponent<CanvasGroup>();

        // Set the initial local position of the RectTransform to a position just below the screen.
        goalRectTransform.localPosition = new Vector2(goalRectTransform.anchoredPosition.x, -30);
        goalRectTransform.DOLocalMoveY(goalRectTransform.anchoredPosition.y + 30, 0.3f);

        // Use DOTween to animate the RectTransform upwards by 30 units and fade in the CanvasGroup.
        goalCanvasGroup.alpha = 0;
        goalCanvasGroup.DOFade(1, .3f).OnComplete(() =>
        {
            isAnimationChangeObjective = false;

        });
    }

    /// <summary>
    /// Animate the exit of all active goals with a slide down and fade out effect.
    /// </summary>
    /// <param name="goals">The array of goal GameObjects to animate.</param>
    void ExitAnimateGoal(GameObject[] goals)
    {
        // If the current goal is the maximum goal, do not animate the exit.
        if (currentGoal == maxGoal) return;

        isAnimationChangeObjective = true;

        // Loop through each goal GameObject in the array and animate its RectTransform and CanvasGroup components.
        for (int i = 0; i < goals.Length; i++)
        {
            if (goals[i].activeInHierarchy)
            {
                RectTransform goalRectTransform = goals[i].GetComponent<RectTransform>();
                CanvasGroup goalCanvasGroup = goals[i].GetComponent<CanvasGroup>();

                goalRectTransform.DOLocalMoveY(goalRectTransform.anchoredPosition.y - 30, 0.3f);
                goalCanvasGroup.DOFade(0, .3f);
            }
        }
    }

    #endregion;

    #region Scoring Objective

    /// <summary>
    /// Coroutine for updating the remaining score objective on the UI.
    /// </summary>
    /// <returns>An IEnumerator for coroutine execution.</returns>
    public IEnumerator RemainingScore()
    {
        int amount = GameManager.Instance.MaxScoreObjective - GUIManager.Instance.Score;

        while (amount < remainingScoreObjective)
        {
            remainingScoreObjective--;
            remainingScoreText.text = remainingScoreObjective.ToString();

            if (remainingScoreObjective <= 0)
            {
                RemainingScoreComplete();
                yield break;
            }
            yield return null;
        }


        yield return null;
    }

    /// <summary>
    /// Handles the completion of the remaining score objective.
    /// </summary>
    void RemainingScoreComplete()
    {
        audioSource.Play();
        remainingScoreObjective = 0;
        remainingScoreText.text = remainingScoreObjective.ToString();
        GameManager.Instance.ObjectiveComplete = true;
        GUIManager.Instance.CompleteTimeToMatchObjective();
    }

    #endregion

    #region Time Objective

    /// <summary>
    /// Updates the remaining match count on the UI and checks if the match objective is completed.
    /// </summary>
    public void RemainingMatchObjective()
    {
        if (remainingMatch <= 0) return;

        remainingMatch--;
        remainingMatchText.text = remainingMatch.ToString();

        if (remainingMatch == 0)
        {
            audioSource.Play();
            GameManager.Instance.ObjectiveComplete = true;
            GUIManager.Instance.CompleteTimeToMatchObjective();
        }
    }

    #endregion

    #region Collection Objective

    /// <summary>
    /// Coroutine to set the collection objective UI elements.
    /// </summary>
    /// <returns>An enumerator for the coroutine.</returns>
    IEnumerator SetCollectionObjectiveRutiner()
    {
        // Wait until the collection objective sprite is available
        yield return new WaitUntil(() => BoardManager.Instance.SpriteCollectionObjective != null);
        // Set the collection objective sprite and text
        fruitCollectionGoal.GetComponentInChildren<Image>().sprite = BoardManager.Instance.SpriteCollectionObjective;
        fruitCollectionGoal.GetComponentInChildren<TMP_Text>().text = amountFruitCollection.ToString();
    }

    /// <summary>
    /// Changes the collection goals for the game based on the list of matched fruits.
    /// </summary>
    /// <param name="listMatchesFruits">The list of matched fruits.</param>
    public void ChangeCollectionGoals(List<GameObject> listMatchesFruits)
    {
        if (goalComplete) return;

        // Find all fruits that match the current objective and update the goal amount
        List<GameObject> fistFruitObjective = listMatchesFruits.FindAll(
               fruit => fruit.GetComponentInChildren<SpriteRenderer>().sprite == fruitCollectionGoal.GetComponentInChildren<Image>().sprite);

        if (ThereAreFruits(fistFruitObjective))
        {
            int amountGoal = Convert.ToInt32(fruitCollectionGoal.GetComponentInChildren<TMP_Text>().text);

            amountGoal -= fistFruitObjective.Count;

            // Update the UI element if the goal has been reached or exceeded
            if (amountGoal <= 0)
            {
                ChangeStatusCheckFruitGoal(fruitCollectionGoal.GetComponentInChildren<TMP_Text>(), checkFruitCollectionGoal);
                audioSource.Play();
                goalComplete = true;
                GameManager.Instance.ObjectiveComplete = true;
            }
            else fruitCollectionGoal.GetComponentInChildren<TMP_Text>().text = amountGoal.ToString();
        }
    }

    #endregion
}