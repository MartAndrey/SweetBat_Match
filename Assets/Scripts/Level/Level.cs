
using UnityEngine;
using UnityEngine.UI;

public class Level : MonoBehaviour
{
    [SerializeField] GameObject stars; // The game object that contains the stars for the level
    [SerializeField] Image imageLock; // The Image component for the lock icon
    [SerializeField] Sprite levelUnlocked; // The sprite to use when the level is unlocked

    // Unlocks the level by enabling the stars, hiding the lock icon, and updating the level image
    public void UnlockLevel()
    {
        imageLock.enabled = false; // Hide the lock icon
        stars.SetActive(true); // Show the stars
        GetComponent<Image>().sprite = levelUnlocked; // Update the level image to show that it is unlocked
    }
}