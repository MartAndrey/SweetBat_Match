using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using System;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    // The prefab for the level objects
    [SerializeField] GameObject levelPrefab;
    // The starting levels
    [SerializeField] int initialLevel = 50;

    // A game object that marks the player's progress on the level objects
    [SerializeField] GameObject profileMarker;

    // This line is marking the GameObject "returnLevel" as Serializable 
    [SerializeField] ReturnLevel returnLevel;

    // This line is marking the GameObject "focusLevel" as Serializable
    [SerializeField] GameObject focusLevel;

    // A list of the level objects in the scene
    List<GameObject> levelsList = new List<GameObject>();

    RectTransform rectTransformLevels;

    // Get the width of the level prefab
    float widthLevelPrefab;

    // This integer variable defines the maximum distance between the focusLevel and profileMarker before the player is returned to the returnLevel.
    readonly int maxDistanceToReturn = 850;

    // Duration to move the Rect Transform from the list of levels to indicate the next level
    readonly float nextLevelTime = 1;
    readonly int nextPositionLevels = -250;

    public int LimitLevels { get; set; }
    public int OffsetNewLevels { get { return offsetNewLevels; } }
    int offsetNewLevels = 4;
    bool levelsAlreadyCreated = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // Get the width of the level prefab
        LimitLevels = -offsetNewLevels;
        widthLevelPrefab = levelPrefab.GetComponent<RectTransform>().sizeDelta.x;
        rectTransformLevels = GetComponent<RectTransform>();
        GetUserLevels();
    }

    /// <summary>
    /// Retrieves user levels and initializes them if they exist; otherwise, creates initial levels.
    /// </summary>
    void GetUserLevels()
    {
        List<Dictionary<string, object>> data = GameManager.Instance.LevelsData;

        // If levels exist, create them; otherwise, create initial levels
        if (data != null && data.Count > 0)
        {
            LimitLevels = data.Count - offsetNewLevels;
            CreateAndSetLevels(data);
            return;
        }

        CreateNewLevels(initialLevel);
    }

    /// <summary>
    /// Creates the specified number of levels starting from the current level.
    /// </summary>
    /// <param name="amount">The number of levels to create.</param>
    void CreateNewLevels(int amount)
    {
        // Get the current level from the GameManager
        int currentLevel = LimitLevels + offsetNewLevels;

        // Create a list to hold level data
        List<Dictionary<string, object>> level = new List<Dictionary<string, object>>();

        // Create the specified number of levels
        for (int i = 1; i <= amount; i++)
        {
            // Instantiate a copy of the level prefab
            GameObject newLevel = Instantiate(levelPrefab, transform);

            // Set the level number and name
            newLevel.GetComponentInChildren<TextMeshProUGUI>().text = (currentLevel + i).ToString();
            newLevel.name = "Level " + (currentLevel + i);

            // Add the level to the levels list
            levelsList.Add(newLevel);

            // Set level data based on setDataBase flag
            // Create level data for new levels
            Dictionary<string, object> levelData = new Dictionary<string, object>()
            {
                {"Stars", 0},
                {"Score", 0},
                {"Game Mode", UnityEngine.Random.Range(0, Enum.GetValues(typeof(GameMode)).Length)},
                {"order", currentLevel + i},
            };

            level.Add(levelData);

            SetInformationAndIncreaseSizeLevelsContainer(i, newLevel, levelData);
        }

        GameManager.Instance.LevelsData.AddRange(level);

        // Update user levels in the database
        CloudFirestore.Instance.SetUserLevels(level);

        UnlockLevelAndUpdateAvatar();

        LimitLevels = GameManager.Instance.LevelsData.Count - offsetNewLevels;
        levelsAlreadyCreated = true;
    }

    /// <summary>
    /// Creates and sets levels based on the provided list of level data.
    /// </summary>
    /// <param name="listLevels">The list of level data.</param>
    void CreateAndSetLevels(List<Dictionary<string, object>> listLevels)
    {
        for (int i = 0; i < listLevels.Count; i++)
        {
            // Instantiate a copy of the level prefab
            GameObject newLevel = Instantiate(levelPrefab, transform);

            // Add the level to the levels list
            levelsList.Add(newLevel);

            Dictionary<string, object> levelData = listLevels[i];

            SetInformationAndIncreaseSizeLevelsContainer(i, newLevel, levelData, true);
            newLevel.GetComponentInChildren<TextMeshProUGUI>().text = levelData["order"].ToString();

        }

        UnlockLevelAndUpdateAvatar();
        levelsAlreadyCreated = true;
    }

    /// <summary>
    /// Unlocks the next level and updates the avatar.
    /// </summary>
    void UnlockLevelAndUpdateAvatar()
    {
        StartCoroutine(UnlockLevelAndUpdateAvatarRoutine());
    }

    /// <summary>
    /// Coroutine that unlocks the next level and updates the avatar.
    /// </summary>
    IEnumerator UnlockLevelAndUpdateAvatarRoutine()
    {
        yield return new WaitForEndOfFrame();
        UnlockLevel(levelsList[GameManager.Instance.Level]); // Unlock current level
        UpdatePositionProfileMarker();
        SetScrollRect();
    }

    /// <summary>
    /// Adjusts the scroll rect position based on the level focus.
    /// </summary>
    void SetScrollRect()
    {
        float distance = focusLevel.transform.localPosition.x - profileMarker.transform.localPosition.x;

        ScrollRect scrollRect = gameObject.GetComponentInParent<ScrollRect>();
        scrollRect.content.anchoredPosition = new Vector2(scrollRect.content.anchoredPosition.x + distance, 0);
    }

    /// <summary>
    /// Sets the information and increases the size of the levels container.
    /// </summary>
    /// <param name="i">The index of the level.</param>
    /// <param name="currentLevel">The current level.</param>
    /// <param name="newLevel">The new level object.</param>
    /// <param name="levelData">The level data.</param>
    /// <param name="getDataBase">Flag indicating whether to get data from the database.</param>
    void SetInformationAndIncreaseSizeLevelsContainer(int i, GameObject newLevel, Dictionary<string, object> levelData, bool getDataBase = false)
    {
        Level levelUser = newLevel.GetComponent<Level>();
        levelUser.Stars = Convert.ToInt32(levelData["Stars"]);
        levelUser.Score = Convert.ToInt32(levelData["Score"]);
        levelUser.GoalInformation = levelData["Game Mode"].ToString();
        levelUser.name = $"Level {levelData["order"]}";

        if (getDataBase) levelUser.CheckLevelToUnlock();

        // Increase the size of the levels container if there are more than 4 levels and the current level is not 4
        if (i > 4 || levelsAlreadyCreated)
        {
            // Increase the width of the levels container by the width of the level prefab
            rectTransformLevels.sizeDelta = new Vector2(rectTransformLevels.sizeDelta.x + widthLevelPrefab, rectTransformLevels.sizeDelta.y);
        }
    }

    // Called when the user scrolls through the level objects
    public void OnScrollMotion()
    {
        // Update the position of the profile marker
        UpdatePositionProfileMarker();
        ReturnToCurrentLevel();
    }

    // Updates the position of the profile marker based on the current level
    void UpdatePositionProfileMarker()
    {
        profileMarker.transform.position = new Vector2(levelsList[GameManager.Instance.Level].transform.position.x, profileMarker.transform.position.y);
    }

    // Unlocks a level by calling its UnlockLevel() method
    void UnlockLevel(GameObject level)
    {
        level.GetComponent<Level>().UnlockLevel();
    }

    // This function returns the player to the returnLevel if they move too far away from the focus Level.
    void ReturnToCurrentLevel()
    {
        // Calculate the distance between the focusLevel and profileMarker.
        float distance = focusLevel.transform.position.x - profileMarker.transform.position.x;

        // If the distance is greater than the maxDistanceToReturn, rotate the returnLevel to the right and set it to active.
        if (distance > maxDistanceToReturn)
        {
            returnLevel.StatusChild(true);
            returnLevel.Rotation(Vector2.right);
        }
        // If the distance is less than negative maxDistanceToReturn, rotate the returnLevel to the left and set it to active.
        else if (distance < -maxDistanceToReturn)
        {
            returnLevel.StatusChild(true);
            returnLevel.Rotation(Vector2.left);
        }
        // If the distance is within the acceptable range, set the returnLevel to inactive.
        else
        {
            returnLevel.StatusChild(false);
        }
    }

    /// <summary>
    /// Unlocks the next level, moves the profile marker to the next level, and sets focus on it.
    /// </summary>
    public void NextLevel()
    {
        GameObject nextLevel = levelsList[GameManager.Instance.Level + 1];

        GameManager.Instance.Level++;

        // Unlocks the next level
        nextLevel.GetComponent<Level>().UnlockLevel();

        // Animates the movement of the profile marker to the next level
        profileMarker.transform.DOMoveX(nextLevel.transform.position.x, nextLevelTime);
        profileMarker.transform.DOMoveY(profileMarker.transform.position.y + 40, nextLevelTime / 2).OnComplete(() =>
        {
            profileMarker.transform.DOMoveY(profileMarker.transform.position.y - 40, nextLevelTime / 2).OnComplete(() =>
            {
                // Sets focus on the next level and animates the movement of the levels list
                focusLevel.GetComponent<FocusLevel>().SetLevelFocus(nextLevel.GetComponent<Collider2D>());
                rectTransformLevels.DOAnchorPosX(rectTransformLevels.anchoredPosition.x + nextPositionLevels, nextLevelTime).OnComplete(() =>
                {
                    if (GameManager.Instance.Level + 1 == LimitLevels)
                        CreateNewLevels(initialLevel);
                });
            });
        });

    }
}