using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonValueOfCoin : MonoBehaviour
{
    // The power up types that will be affected by the button
    [SerializeField] TypePowerUp[] typePowerUp;
    // The power ups that will be affected by the button
    [SerializeField] PowerUp[] inventoryPowersUp;
    // The value in coins of the power ups
    [SerializeField] int valueOfPPowerUp;
     // The amount of power ups that will be added to the inventory
    [SerializeField] int amountOfPPowerUp;
    // The sound effect that will play when the button is pressed
    [SerializeField] AudioClip popEnter;

     // The time that the power ups will be infinite, if applicable
    [SerializeField, Tooltip("Insert the value in minutes only if the powers up are infinite for a certain time")]
    float infiniteTimePowerUp;

    AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Buy()
    {
        audioSource.PlayOneShot(popEnter);

        // Check if the player has enough coins to purchase the power ups
        if (CoinController.Instance.Coins < valueOfPPowerUp) return;

        // Add the specified amount of power ups to the inventory
        foreach (TypePowerUp item in typePowerUp)
        {
            Inventory.Instance.InventoryItems[item] += amountOfPPowerUp;

            // If the power ups are infinite for a certain time, set a timer
            if (infiniteTimePowerUp != 0)
            {
                foreach (PowerUp powerUp in inventoryPowersUp)
                {
                    if (powerUp.TypePowerUp == item)
                    {
                        StartCoroutine(InternetTime.Instance.GetInternetTime(currentTime =>
                        {
                            powerUp.MakeInfinitePowerUp(infiniteTimePowerUp, currentTime);
                        }));
                        break;
                    }
                }
            }
        }

         // Subtract the value of the power ups from the player's coins
        CoinController.Instance.ChangeCoins(-valueOfPPowerUp);
    }
}