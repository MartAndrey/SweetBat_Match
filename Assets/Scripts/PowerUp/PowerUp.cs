using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

// Type of power up that this object represents
public enum TypePowerUp { Bomb, Lightning, Potion }
public enum StatePowerUp { Profile, LevelUI, Game }

// Timer that counts down the time for the power up
public class PowerUp : Timer
{
    // Type of power up that this object represents
    public TypePowerUp TypePowerUp { get { return typePowerUp; } }
    // State or place where the Power Up is located
    public StatePowerUp StatePowerUp { get { return statePowerUp; } set { statePowerUp = value; } }
    // Text component that displays the amount of this power up the player has
    public TMP_Text TextAmount { get { return textAmount; } set { textAmount = value; } }

    [SerializeField] TypePowerUp typePowerUp;
    [SerializeField] StatePowerUp statePowerUp;

    [SerializeField] TMP_Text textAmount;
    [SerializeField] GameObject labelTimer;
    [SerializeField] GameObject imageInfinite;
    [SerializeField] GameObject imageCheck;
    [SerializeField] Image spritePowerUp;

    bool isChecked; // boolean that tells us if the power up was chosen only works in the StatePowerUp.LevelUI

    AudioSource audioSource;
    Animator animator;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
    }

    // Called when the script instance is being loaded
    void OnEnable()
    {
        // If the power up is infinite, start the timer
        if (IsInfinite)
        {
            StartCoroutine(WaitForGetCurrentTime());
        }
    }

    // Called when the script instance is being disabled
    void OnDisable()
    {
        InternetTime.Instance.UpdateInternetTime(this.gameObject);

        if (isChecked)
        {
            ChangeCheckPowerUp();
        }
    }

    void Update()
    {
        // If the power up is infinite, update the timer
        if (IsInfinite)
            UpdateTimer(OnCountdownFinished);
    }

    // This method is called when the counter or timer reaches zero.
    void OnCountdownFinished()
    {
        // Change the state of the power up
        ChangeStatePowerUp();
    }

    // This method is responsible for making ignition infinite for a certain time, it is also responsible for changing the UI in "Profile"
    public void MakeInfinitePowerUp(float time, DateTime currentTime)
    {
        timeRemainingInSeconds += time * 60;
        previouslyAllottedTime = currentTime;

        if (!IsInfinite)
            ChangeStatePowerUp();
    }

    // Changes the state of the power up and updates the UI
    void ChangeStatePowerUp()
    {
        IsInfinite = !IsInfinite;
        labelTimer.SetActive(!labelTimer.activeSelf);
        textAmount.enabled = !textAmount.enabled;
        imageInfinite.SetActive(!imageInfinite.activeSelf);
    }

    /// <summary>
    /// Change the state of the checkbox and the isChecked variable.
    /// </summary>
    /// <summary>
    /// Change the state of the checkbox and the isChecked variable.
    /// </summary>
    void ChangeCheckPowerUp()
    {
        imageCheck.SetActive(imageCheck.activeSelf == true ? false : true);
        isChecked = isChecked == true ? false : true;
    }

    /// <summary>
    /// This method is called when the power button is clicked.
    /// </summary>
    public void OnClickPowerUp()
    {
        animator.enabled = true;
        audioSource.Play();

        if (statePowerUp == StatePowerUp.Profile)
        {
            Debug.Log("Profile");
        }
        else if (statePowerUp == StatePowerUp.LevelUI)
        {
            ChangeCheckPowerUp();
        }
        else if (statePowerUp == StatePowerUp.Game)
        {
            if (BoardManager.Instance.IsShifting) return;
            // Updates the sorting order of the canvas based on the overlay display state,
            // and triggers the overlay switch with the provided power-up sprite.
            OverlayDisplayPowerUp overlay = GameObject.FindObjectOfType<OverlayDisplayPowerUp>();
            this.GetComponent<Canvas>().sortingOrder = overlay.OverlayEnable ? 0 : 5;
            overlay.SwitchState(spritePowerUp.sprite);
            GameManager.Instance.CurrentPowerUp = typePowerUp;
            GameManager.Instance.CurrentGameObjectPowerUp = this.gameObject;
        }
    }

    // <summary>
    /// Disables the animator of the object.
    /// </summary>
    public void DisableAnimator()
    {
        animator.enabled = false;
    }
}