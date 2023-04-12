using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    // The prefab for the level objects
    [SerializeField] GameObject levelPrefab;
    // The starting levels
    [SerializeField] int initialLevel = 50;

    [SerializeField] RectTransform rectTransformLevels;
    // A game object that marks the player's progress on the level objects
    [SerializeField] GameObject profileMarker;

    // This line is marking the GameObject "returnLevel" as Serializable 
    [SerializeField] ReturnLevel returnLevel;

    // This line is marking the GameObject "focusLevel" as Serializable
    [SerializeField] GameObject focusLevel;

    // A list of the level objects in the scene
    List<GameObject> levelsList = new List<GameObject>();

    // Get the width of the level prefab
    float widthLevelPrefab;

    // This integer variable defines the maximum distance between the focusLevel and profileMarker before the player is returned to the returnLevel.
    int maxDistanceToReturn = 850;

    void Start()
    {
        widthLevelPrefab = levelPrefab.GetComponent<RectTransform>().sizeDelta.x;

        CreateLevel(initialLevel); // Create the initial levels
        UnlockLevel(levelsList[GameManager.Instance.Level]); // Unlock the first level
        UnlockLevel(levelsList[1]);
        UnlockLevel(levelsList[2]);
        UnlockLevel(levelsList[3]);
        UnlockLevel(levelsList[4]);
        UnlockLevel(levelsList[5]);
    }
    // [ContextMenu("SetLevels")] void SetLevels() => CreateLevel(10);
    // Creates the specified number of levels starting from the current level
    void CreateLevel(int amount)
    {
        int currentLevel = GameManager.Instance.Level;  // Get the current level from the GameManager

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

            // Increase the size of the levels container if there are more than 4 levels and the current level is not 4
            if (i > 4 && currentLevel < 4 || currentLevel > 4)
            {
                // Increase the width of the levels container by the width of the level prefab
                rectTransformLevels.sizeDelta = new Vector2(rectTransformLevels.sizeDelta.x + widthLevelPrefab, rectTransformLevels.sizeDelta.y);
            }
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
}