using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundConfig : MonoBehaviour
{
    AudioManager audioManager; // reference to AudioManager

    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>(); // get AudioManager reference
    }

    // Called when the "Sound Music" button is clicked
    public void OnClickSoundMusic()
    {
        audioManager.ControlMusic(); // call AudioManager's ControlMusic method to toggle music
    }

    // Called when the "Sound SFX" button is clicked
    public void OnClickSoundSFX()
    {
        audioManager.ControlSFX(); // call AudioManager's ControlSFX method to toggle SFX
    }
}