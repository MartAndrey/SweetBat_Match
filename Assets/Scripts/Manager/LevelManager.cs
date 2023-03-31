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
    [SerializeField] int initialLevel = 100;
    // The ScrollRect component that controls the scrolling of the level objects
    // [SerializeField] ScrollRect scrollRect;
    // A game object that marks the player's progress on the level objects
    [SerializeField] GameObject profileMarker;

    // A list of the level objects in the scene
    List<GameObject> levelsList = new List<GameObject>();

    void Start()
    {
        // Create the initial levels
        CreateLevel(initialLevel);
        // Unlock the first level
        UnlockLevel(levelsList[GameManager.Instance.Level - 1]);
    }

    // Creates the specified number of levels starting from the current level
    void CreateLevel(int amount)
    {
        int currentLevel = GameManager.Instance.Level;  // Get the current level from the GameManager

        // Create the specified number of levels
        for (int i = 0; i < amount; i++)
        {
            // Instantiate a copy of the level prefab
            GameObject newLevel = Instantiate(levelPrefab, transform);

            // Set the level number and name
            newLevel.GetComponentInChildren<TextMeshProUGUI>().text = (currentLevel + i).ToString();
            newLevel.name = "Level " + (currentLevel + i);

            // Add the level to the levels list
            levelsList.Add(newLevel);
        }
    }

    // Called when the user scrolls through the level objects
    public void OnScrollMotion()
    {
        // Update the position of the profile marker
        UpdatePositionProfileMarker();
    }

    // Updates the position of the profile marker based on the current level
    void UpdatePositionProfileMarker()
    {
        profileMarker.transform.position = new Vector2(levelsList[GameManager.Instance.Level - 1].transform.position.x, profileMarker.transform.position.y);
    }

    // Unlocks a level by calling its UnlockLevel() method
    void UnlockLevel(GameObject level)
    {
        level.GetComponent<Level>().UnlockLevel();
    }

    // Restrict the scrolling of the level objects
    void RestrictMovement()
    {
        // Vector2 contentPosition = GetComponent<RectTransform>().anchoredPosition
        // Vector2 firstLevel = levelsList.First().GetComponent<RectTransform>().anchoredPosition;
        // Vector2 lastLevel = levelsList.Last().GetComponent<RectTransform>().anchoredPosition;
    }
}