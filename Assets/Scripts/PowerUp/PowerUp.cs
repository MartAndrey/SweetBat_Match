using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

// Type of power up that this object represents
public enum TypePowerUp { Bomb, Lightning, Potion }

// Timer that counts down the time for the power up
public class PowerUp : Timer
{
    // Type of power up that this object represents
    public TypePowerUp TypePowerUp { get { return typePowerUp; } }
    // Text component that displays the amount of this power up the player has
    public TMP_Text TextAmount { get { return textAmount; } set { textAmount = value; } }

    [SerializeField] TypePowerUp typePowerUp;
    [SerializeField] TMP_Text textAmount;
    [SerializeField] GameObject labelTimer;
    [SerializeField] GameObject imageInfinite;

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
}