using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioMixer sfxMixer;
    [SerializeField] AudioMixer musicMixer;

    [SerializeField] Image imageMusic;
    [SerializeField] Sprite imageMusicOn;
    [SerializeField] Sprite imageMusicOff;


    [SerializeField] Image imageSFX;
    [SerializeField] Sprite imageSFXOn;
    [SerializeField] Sprite imageSFXOff;

    float currentValueSFX = 1;
    float currentValueSMusic = 1;

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