using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ToggleAudio : MonoBehaviour
{
    // Enum for different toggle types
    enum TypeToggle { Music, SFX }
    // Serialized variable for toggle type
    [SerializeField] TypeToggle typeToggle;
    // Serialized variable for audio image
    [SerializeField] Image imageAudio;
    // Serialized variable for button's rect transform
    [SerializeField] RectTransform rectTransformButton;
    // Serialized variable for mute status
    [SerializeField] bool isMute;
    // Variable for audio on position
    float audionOn = 192;
    // Variable for audio off position
    float audionOff = 0;
    // Serialized variable for button's rect transform

    [SerializeField] RectTransform buttonToggle;

    AudioManager audioManager;

    void Awake()
    {
        DOTween.defaultTimeScaleIndependent = true; // Enable independent time scale for DOTween
        audioManager = FindObjectOfType<AudioManager>(); // get AudioManager reference
    }

    void OnEnable()
    {
        // Invoke the AuxUpdateAudioUI method after a short delay to ensure that the audio UI references are properly set
        StartCoroutine(AuxUpdateAudioUIRutiner());
    }

    /// <summary>
    /// Called when the "Sound Music" button is clicked.
    /// </summary>
    public void OnClickSoundMusic()
    {
        isMute = !isMute; // Toggle mute status
        // Animate button's position based on mute status
        if (isMute) rectTransformButton.DOAnchorPos(new Vector2(audionOff, 0), 1f);
        else rectTransformButton.DOAnchorPos(new Vector2(audionOn, 0), 1f);
        audioManager.ControlMusic(); // Call AudioManager's ControlMusic method to toggle music

    }

    /// <summary>
    /// Called when the "Sound SFX" button is clicked.
    /// </summary>
    public void OnClickSoundSFX()
    {
        isMute = !isMute; // Toggle mute status
        // Animate button's position based on mute status
        if (isMute) rectTransformButton.DOAnchorPos(new Vector2(audionOff, 0), 1f);
        else rectTransformButton.DOAnchorPos(new Vector2(audionOn, 0), 1f);
        audioManager.ControlSFX(); // Call AudioManager's ControlSFX method to toggle sound effects
    }

    /// <summary>
    /// Coroutine that waits for 0.1 seconds before calling AuxUpdateAudioUI method
    /// </summary>
    /// <returns>Returns an IEnumerator</returns>
    IEnumerator AuxUpdateAudioUIRutiner()
    {
        // Wait for a short delay to ensure audio UI references are properly set
        yield return new WaitForSecondsRealtime(.1f);
        // Call AuxUpdateAudioUI method
        AuxUpdateAudioUI();
    }

    /// <summary>
    /// Method that updates the audio UI based on the current toggle type.
    /// </summary>
    void AuxUpdateAudioUI()
    {
        if (typeToggle == TypeToggle.Music)
        {
            audioManager.UpdateAudioMusicUI(imageAudio);
            isMute = audioManager.IsMuteControlMusic();
        }
        else if (typeToggle == TypeToggle.SFX)
        {
            audioManager.UpdateAudioSFXUI(imageAudio);
            isMute = audioManager.IsMuteControlSFX();
        }

        if (isMute) rectTransformButton.anchoredPosition = new Vector2(audionOff, 0);
        else rectTransformButton.anchoredPosition = new Vector2(audionOn, 0);
    }
}