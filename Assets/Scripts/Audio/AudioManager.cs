using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [Header("Sound Music")]
    [SerializeField] AudioMixer musicMixer;
    [SerializeField] Image imageMusic;
    [SerializeField] Sprite imageMusicOn;
    [SerializeField] Sprite imageMusicOff;

    [Header("Sound SFX")]
    [SerializeField] AudioMixer sfxMixer;
    [SerializeField] Image imageSFX;
    [SerializeField] Sprite imageSFXOn;
    [SerializeField] Sprite imageSFXOff;

    // Current values ​​of each audio mixer
    float currentValueSFX = 0;
    float currentValueSMusic = 0;

    // Minimum and maximum value of sound
    float minValue = -80.00f;
    float maxValue = 0.00f;

    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void ControlSFX()
    {
        currentValueSFX = currentValueSFX <= minValue ? maxValue : minValue;
        imageSFX.sprite = imageSFX.sprite == imageSFXOn ? imageSFXOff : imageSFXOn;

        sfxMixer.SetFloat("SFXMixer",currentValueSFX);
    }

    public void ControlMusic()
    {
        currentValueSMusic = currentValueSMusic <= minValue ? maxValue : minValue;
        imageMusic.sprite = imageMusic.sprite == imageMusicOn ? imageMusicOff : imageMusicOn;

        musicMixer.SetFloat("MusicMixer", currentValueSMusic);
    }
}