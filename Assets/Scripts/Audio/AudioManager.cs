using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance; // Singleton instance

    [Header("Sound Music")]
    [SerializeField] AudioMixer musicMixer; // Audio mixer for music
    [SerializeField] Image imageMusic;  // Image component for music icon
    [SerializeField] Sprite imageMusicOn;  // Sprite for music on
    [SerializeField] Sprite imageMusicOff; // Sprite for music off

    [Header("Sound SFX")]
    [SerializeField] AudioMixer sfxMixer; // Audio mixer for sound effects
    [SerializeField] Image imageSFX; // Image component for sound effects icon
    [SerializeField] Sprite imageSFXOn; // Sprite for sound effects on
    [SerializeField] Sprite imageSFXOff; // Sprite for sound effects off

    // Current values ​​of each audio mixer
    float currentValueSFX = 0;
    float currentValueMusic = 0;

    // Minimum and maximum value of sound
    float minValue = -80.00f;
    float maxValue = 0.00f;

    void Awake()
    {
        if (Instance == null) Instance = this; // Singleton implementation
        else Destroy(gameObject);
    }

    void Start()
    {
        DontDestroyOnLoad(gameObject); // Don't destroy game object on load
    }

    public void ControlSFX()
    {
        currentValueSFX = currentValueSFX <= minValue ? maxValue : minValue; // Toggles the value of sound effect volume
        imageSFX.sprite = imageSFX.sprite == imageSFXOn ? imageSFXOff : imageSFXOn; // Toggles the sprite of sound effect icon

        sfxMixer.SetFloat("SFXMixer", currentValueSFX); // Sets the volume of sound effects
    }

    public void ControlMusic()
    {
        currentValueMusic = currentValueMusic <= minValue ? maxValue : minValue; // Toggles the value of music volume
        imageMusic.sprite = imageMusic.sprite == imageMusicOn ? imageMusicOff : imageMusicOn; // Toggles the sprite of music icon

        musicMixer.SetFloat("MusicMixer", currentValueMusic); // Sets the volume of music
    }

    /// <summary>
    /// Updates the music and SFX toggle buttons' UI images based on the current audio state.
    /// </summary>
    /// <param name="imageMusic">The image component for the music toggle button.</param>
    /// <param name="imageSFX">The image component for the SFX toggle button.</param>
    public void UpdateAudioUI(Image imageMusic, Image imageSFX)
    {
        UpdateAudioMusicUI(imageMusic);
        UpdateAudioSFXUI(imageSFX);
    }

    /// <summary>
    /// Updates the music toggle button's UI image based on the current audio state.
    /// </summary>
    /// <param name="imageMusic">The image component for the music toggle button.</param>
    public void UpdateAudioMusicUI(Image imageMusic)
    {
        this.imageMusic = imageMusic; // Updates the image component for music icon
        imageMusic.sprite = currentValueMusic == minValue ? imageMusicOff : imageMusicOn;
    }

    /// <summary>
    /// Updates the SFX toggle button's UI image based on the current audio state.
    /// </summary>
    /// <param name="imageSFX">The image component for the SFX toggle button.</param>
    public void UpdateAudioSFXUI(Image imageSFX)
    {
        this.imageSFX = imageSFX; // Updates the image component for sound effect icon
        imageSFX.sprite = currentValueSFX == minValue ? imageSFXOff : imageSFXOn;
    }

    /// <summary>
    /// Determines if the music is currently muted.
    /// </summary>
    /// <returns><c>true</c> if the music is currently muted; otherwise, <c>false</c>.</returns>
    public bool IsMuteControlMusic() => currentValueMusic == minValue;

    /// <summary>
    /// Determines if the SFX are currently muted.
    /// </summary>
    /// <returns><c>true</c> if the SFX are currently muted; otherwise, <c>false</c>.</returns>
    public bool IsMuteControlSFX() => currentValueSFX == minValue;
}