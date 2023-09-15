using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using System.ComponentModel;
using Unity.VisualScripting;
using System.Collections;
using Firebase.Firestore;
using System.Collections.Generic;

// Type of power up that this object represents
public enum TypePowerUp
{
    [Description("Remove fruits in a radius of 0.6, a real explosion of fun!")]
    Bomb,
    [Description("Eliminate rows and columns of fruit, unleash the storm of destruction on the board!")]
    Lightning,
    [Description("Revolutionize fruits within a radius of 1, prepare the chaos of combinations!")]
    Potion
}
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
    public bool IsChecked { get { return isChecked; } set { isChecked = value; } }
    public bool IsInTransitionDescription { get { return isInTransitionDescription; } }
    // Public property to check if cooling is active
    public bool IsCooldown { get { return isCooldown; } }

    [SerializeField] TypePowerUp typePowerUp;
    [SerializeField] StatePowerUp statePowerUp;

    [SerializeField] TMP_Text textAmount;
    [SerializeField] GameObject labelTimer;
    [SerializeField] GameObject imageInfinite;
    [SerializeField] GameObject imageCheck;
    [SerializeField] Image spritePowerUp;
    [SerializeField] TMP_Text descriptionText;
    [SerializeField] GameObject descriptionObject;
    [SerializeField] Image cooldown;

    // boolean that tells us if the power up was chosen only works in the StatePowerUp.LevelUI
    bool isChecked;
    bool isInTransitionDescription;
    // Private variable to track if cooling is active
    bool isCooldown;

    AudioSource audioSource;
    Animator animator;
    float timeDescription = 3;
    // Cooling duration in seconds
    readonly int timeCooldown = 20;
    // Current elapsed cooling time
    float currentTimeCooldown = 0;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        descriptionText.text = GetPowerUpDescription(typePowerUp);
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

        if (isChecked && GameManager.Instance.currentGameState == GameState.LevelMenu)
        {
            toggleChangeCheckPowerUp();
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
        ResetCoolDownUI();
        SavePowerDataBase();
    }

    /// <summary>
    /// Saves power-up data to the inventory database.
    /// </summary>
    void SavePowerDataBase()
    {
        // Create a sub-dictionary for power-up data with a default time of 0.
        Dictionary<string, object> subData = new Dictionary<string, object> { { "time", timeRemainingInSeconds } };
        // Create a dictionary to store the power-up type and its sub-data.
        Dictionary<string, object> data = new Dictionary<string, object> { { typePowerUp.ToString(), subData } };
        // Save the data to the inventory.
        Inventory.Instance.SaveDataBase(data);
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
    void toggleChangeCheckPowerUp()
    {
        imageCheck.SetActive(imageCheck.activeSelf == true ? false : true);
        isChecked = isChecked == true ? false : true;
    }

    /// <summary>
    /// Toggles the visibility of the checkmark image.
    /// </summary>
    public void ChangeCheckPowerUpUI(bool stateToChange)
    {
        imageCheck.SetActive(stateToChange);
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
            if (isInTransitionDescription) return;

            isInTransitionDescription = true;
            descriptionObject.SetActive(true);
            animator.enabled = true;
            animator.Play("DescriptionIn");
            StartCoroutine(TimeToDescriptionRutiner());
        }
        else if (statePowerUp == StatePowerUp.LevelUI)
        {
            toggleChangeCheckPowerUp();
        }
        else if (statePowerUp == StatePowerUp.Game)
        {
            if (BoardManager.Instance.IsShifting || (Inventory.Instance.InventoryItems[typePowerUp] <= 0 && !IsInfinite) || isCooldown) return;
            // Updates the sorting order of the canvas based on the overlay display state,
            // and triggers the overlay switch with the provided power-up sprite.
            OverlayDisplayPowerUp overlay = GameObject.FindObjectOfType<OverlayDisplayPowerUp>();
            this.GetComponent<Canvas>().sortingOrder = overlay.OverlayEnable ? 1 : 5;
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

    /// <summary>
    /// Retrieves the description of a PowerUp type based on its Description attribute.
    /// </summary>
    /// <param name="typePowerUp">The PowerUp type for which to retrieve the description.</param>
    /// <returns>The description of the PowerUp type.</returns>    
    string GetPowerUpDescription(TypePowerUp typePowerUp)
    {
        // Get the field representing the PowerUp type.
        var fieldInfo = typePowerUp.GetType().GetField(typePowerUp.ToString());
        // Get the Description attributes of the field.
        var attributes = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

        // Return the description of the PowerUp type.
        return attributes[0].Description;
    }

    /// <summary>
    /// Routine that waits for a specified time and then performs the description exit animation.
    /// </summary>
    IEnumerator TimeToDescriptionRutiner()
    {
        // Wait for the specified time before continuing.
        yield return new WaitForSeconds(timeDescription);
        // Enable the animator if it's not enabled.
        if (!animator.enabled) animator.enabled = true;
        // Play the exit animation.
        animator.Play("DescriptionOut");
        // Wait for an additional time before disabling the description.
        yield return new WaitForSeconds(.6f);
        // Disable the description.
        DisableDescription();
    }

    /// <summary>
    /// Disables the description and related animator.
    /// </summary>
    public void DisableDescription()
    {
        // Indicates that it's not in description transition.
        isInTransitionDescription = false;
        // Disable the description object.
        descriptionObject.SetActive(false);
        // Disable the animator.
        DisableAnimator();
    }

    /// <summary>
    /// Sets the game timer to an infinite state, adjusting the time remaining.
    /// </summary>
    /// <param name="currentTime">The current time.</param>
    /// <param name="time">The timestamp to set the game timer to.</param>
    /// <param name="timeToInfinite">The time in seconds for the infinite power-up.</param>
    public void SetTimerInfinite(DateTime currentTime, Timestamp time, int timeToInfinite)
    {
        this.currentTime = currentTime;

        Timestamp timestamp = time;
        DateTime utcDate = timestamp.ToDateTime();
        DateTime localDate = utcDate.ToLocalTime();

        previouslyAllottedTime = localDate;

        float timeDifference = (float)(currentTime.Subtract(previouslyAllottedTime)).TotalSeconds;

        MakeInfinitePowerUp(timeToInfinite, currentTime);

        timeRemainingInSeconds -= timeDifference;
    }

    /// <summary>
    /// Initiates the cooldown process.
    /// </summary>
    public void StartCooldown()
    {
        isCooldown = true;
        cooldown.gameObject.SetActive(true);
        cooldown.fillAmount = 1;
        currentTimeCooldown = 0;

        StartCoroutine(CooldownRutiner());
    }

    /// <summary>
    /// Routine to manage cooldown.
    /// </summary>
    /// <returns>An enumerator that allows time-based waiting.</returns>
    IEnumerator CooldownRutiner()
    {
        float factor;

        while (currentTimeCooldown < timeCooldown)
        {
            currentTimeCooldown += Time.deltaTime;

            factor = 1 - (currentTimeCooldown / timeCooldown);
            cooldown.fillAmount = factor;

            if (currentTimeCooldown > timeCooldown)
            {
                isCooldown = false;
                cooldown.gameObject.SetActive(false);
            }

            yield return null;
        }

        yield return null;
    }

    /// <summary>
    /// Resets the cooldown user interface.
    /// </summary>
    public void ResetCoolDownUI()
    {
        isCooldown = false;
        cooldown.gameObject.SetActive(false);
    }
}