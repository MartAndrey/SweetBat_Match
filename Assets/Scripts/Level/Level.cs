
using UnityEngine;
using UnityEngine.UI;

public class Level : MonoBehaviour
{
    // Declaration of the 'Stars' property, which returns the value of the 'stars' variable
    // public int Stars { get { return stars; } }

    [SerializeField] Image imageLock; // The Image component for the lock icon
    [SerializeField] Sprite levelUnlocked; // The sprite to use when the level is unlocked

    AudioSource audioSource; // Declaration of the 'audioSource' variable of the 'AudioSource' data type

    // Declaration of the 'stars' variable of the 'int' data type, 'goal' variable of the 'string' data type, and 'isActive' variable of the 'bool' data type
    int stars;
    string goal = "Default";
    bool isActive = false;

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

            levelUI.ActiveLevelUI();
            // Sets the level name, number of stars, and goal in the level UI
            levelUI.SetValueLevel(gameObject.name, stars, goal);
        }
    }
}