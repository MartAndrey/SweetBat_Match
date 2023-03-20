using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonValueOfCoin : MonoBehaviour
{
    [SerializeField] TypePowerUp[] typePowerUp;
    [SerializeField] PowerUp[] inventoryPowersUp;
    [SerializeField] int valueOfPPowerUp;
    [SerializeField] int amountOfPPowerUp;
    [SerializeField] AudioClip popEnter;

    [SerializeField, Tooltip("Insert the value in minutes only if the powers up are infinite for a certain time")]
    int infiniteTimePowerUp;

    AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Buy()
    {
        audioSource.PlayOneShot(popEnter);

        if (CoinController.Instance.Coins < valueOfPPowerUp) return;

        foreach (TypePowerUp item in typePowerUp)
        {
            Inventory.Instance.InventoryItems[item] += amountOfPPowerUp;

            if(infiniteTimePowerUp != 0)
            {
                foreach (PowerUp powerUp in inventoryPowersUp)
                {
                    if(powerUp.TypePowerUp == item )
                    {
                        powerUp.MakeInfinitePowerUp(infiniteTimePowerUp);
                        break;
                    }
                }
            }                    
        }

        CoinController.Instance.ChangeCoins(-valueOfPPowerUp);
    }
}
