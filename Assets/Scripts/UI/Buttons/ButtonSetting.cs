using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSetting : MonoBehaviour
{
    [Header("Setting button")]
    [SerializeField] Image imageSetting; // Reference to the image component of the setting button
    [SerializeField] Sprite settingPurple;  // Sprite to use when the setting button is inactive
    [SerializeField] Sprite SettingWhite; // Sprite to use when the setting button is active

    [Header("Box containing the other settings")]
    [SerializeField] Animator boxAnimator; // Reference to the animator component of the settings box
    [SerializeField] GameObject boxSettings; // Reference to the game object of the settings box
    [SerializeField] Image imageMusic;  // Reference to the image component of the music toggle button
    [SerializeField] Image imageSFX; // Reference to the image component of the SFX toggle button

    bool isAnimationTransition = false; // Flag to prevent multiple transitions at once

    void OnEnable()
    {
        // Invoke the AuxUpdateAudioUI method after a short delay to ensure that the audio UI references are properly set
        Invoke("AuxUpdateAudioUI", 0.1f);
    }

    // This method is called when the configure button is clicked     
    public void OnSetting()
    {
        if (isAnimationTransition) return; // If a transition is already in progress, exit the method

        StartCoroutine(CheckTransitionRutiner()); // Start a coroutine to prevent multiple transitions at once

        if (!boxSettings.activeSelf)  // If the settings box is inactive
        {
            boxSettings.SetActive(true); // Activate the settings box
            imageSetting.sprite = SettingWhite; // Change the sprite of the setting button to the active sprite
        }
        else
        {
            boxAnimator.SetTrigger("Transition"); // Trigger the transition animation of the settings box
            StartCoroutine(OffSettingRutiner()); // Start a coroutine to deactivate the settings box after a short delay
            imageSetting.sprite = settingPurple; // Change the sprite of the setting button to the inactive sprite
        }
    }

    // Coroutine to deactivate the settings box after a short delay
    IEnumerator OffSettingRutiner()
    {
        yield return new WaitForSeconds(0.5f); // Wait for 1 second
        boxSettings.SetActive(false); // Deactivate the settings box
    }

    // Coroutine to prevent multiple transitions at once
    IEnumerator CheckTransitionRutiner()
    {
        isAnimationTransition = true; // Set the flag to true to indicate that a transition is in progress
        yield return new WaitForSecondsRealtime(0.5f); // Wait for 1 second using real time (ignoring time scale)
        isAnimationTransition = false; // Set the flag to false to indicate that the transition is complete
    }

    // Method to update the references of the music and SFX toggle buttons in the AudioManager
    void AuxUpdateAudioUI()
    {
        AudioManager.Instance.UpdateAudioUI(imageMusic, imageSFX);
    }
}