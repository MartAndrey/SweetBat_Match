using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum TypePowerUp { Bomb, Lightning, Potion }

public class PowerUp : Timer
{
    public TypePowerUp TypePowerUp { get { return typePowerUp; } }
    public TMP_Text TextAmount { get { return textAmount; } set { textAmount = value; } }
    public bool IsInfinite { get; set; }

    [SerializeField] TypePowerUp typePowerUp;
    [SerializeField] TMP_Text textAmount;
    [SerializeField] GameObject labelTimer;
    [SerializeField] GameObject imageInfinite;

    void Update()
    {
        if (restartTimer)
            UpdateTimer(OnCountdownFinished);
    }

    // This method is called when the counter or timer reaches zero.
    void OnCountdownFinished()
    {
        restartTimer = false;
        labelTimer.SetActive(false);
        IsInfinite = false;

        ChangeAmountPowerUp();
    }

    // This method is responsible for making ignition infinite for a certain time, it is also responsible for changing the UI in "Profile"
    public void MakeInfinitePowerUp(int time)
    {
        IsInfinite = true;
        restartTimer = true;
        timeRemainingInSeconds = time * 60;
        labelTimer.SetActive(true);

        ChangeAmountPowerUp();
    }
    
    void ChangeAmountPowerUp()
    {
        textAmount.enabled = textAmount.enabled == true ? textAmount.enabled = false : textAmount.enabled = true;
        imageInfinite.SetActive(imageInfinite.activeSelf ? false : true);
    }
}