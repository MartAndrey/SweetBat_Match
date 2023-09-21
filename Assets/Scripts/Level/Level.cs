using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class Level : MonoBehaviour
{
    // Declaration of the 'Stars' property, which returns the value of the 'stars' variable
    public int Stars { get { return stars; } set { stars = value; } }
    public int Score { get { return score; } set { score = value; } }
    public string GoalInformation
    {
        get { return goalInformation; }
        set
        {
            string goal = value;
            GameMode gameMode = (GameMode)Enum.Parse(typeof(GameMode), goal);
            this.gameMode = gameMode;
            var fieldInfo = gameMode.GetType().GetField(gameMode.ToString());
            var attributes = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];
            goalInformation = attributes[0].Description;
        }
    }

    public bool IsActive { get { return isActive; } set { isActive = value; } }

    [SerializeField] Image imageLock; // The Image component for the lock icon
    [SerializeField] Sprite levelUnlocked; // The sprite to use when the level is unlocked
    [SerializeField] GameObject[] starsObject;

    AudioSource audioSource; // Declaration of the 'audioSource' variable of the 'AudioSource' data type

    // Declaration of the 'stars' variable of the 'int' data type, 'goal' variable of the 'string' data type, and 'isActive' variable of the 'bool' data type
    int stars;
    int score;
    string goalInformation = "";
    bool isActive = false;
    GameMode gameMode;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Unlocks the level by enabling the stars, hiding the lock icon, and updating the level image
    public void UnlockLevel()
    {
        imageLock.enabled = false; // Hide the lock icon
        GetComponent<Image>().sprite = levelUnlocked; // Update the level image to show that it is unlocked
        GetComponent<UnityEngine.UI.Button>().interactable = true; // Enables the 'Button' component attached to the game object, allowing the level to be interacted with
        isActive = true; // Sets the 'isActive' variable to true
    }

    // Method is called when the level is clicked
    public void OpenLevel()
    {
        audioSource.Play(); // Plays an audio clip using the 'audioSource' variable

        if (isActive)
        {
            // Finds the 'LevelUI' component in the scene and calls the 'ActiveLevelUI' method
            LevelUI levelUI = FindObjectOfType<LevelUI>();

            levelUI.ResetInformationLevel();

            levelUI.ActiveLevelUI();
            // Sets the level name, number of stars, and goal in the level UI
            levelUI.SetValueLevel(gameObject.name, stars, goalInformation, gameMode);
        }
    }

    /// <summary>
    /// Checks if the number of stars is greater than 0 and unlocks the level and enables stars.
    /// </summary>
    public void CheckLevelToUnlock()
    {
        if (stars > 0)
        {
            UnlockLevel();
            EnableStars();
        }
    }

    /// <summary>
    /// Enables stars based on the number of stars.
    /// </summary>
    public void EnableStars()
    {
        for (int i = 0; i < stars; i++)
        {
            starsObject[i].SetActive(true);
        }
    }
}