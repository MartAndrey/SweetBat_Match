using System.Collections.Generic;
using TMPro;
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

    void Start()
    {
        if (inventoryPowersUp[0] == null)
        {
            for (int i = 0; i < typePowerUp.Length; i++)
            {
                inventoryPowersUp[i] = Inventory.Instance.PowerUpsObject.Find(powerUp => powerUp.GetComponent<PowerUp>().TypePowerUp == typePowerUp[i]).GetComponent<PowerUp>();
            }
        }
    }

    /// <summary>
    /// Handles the buying of power-ups.
    /// </summary>
    public void Buy()
    {
        audioSource.PlayOneShot(popEnter);

        // Check if the player has enough coins to purchase the power ups
        if (CoinController.Instance.Coins < valueOfPPowerUp) return;

        if (!CheckIsPowerUpIsActive())
        {
            CoinShop coinShop = GetComponentInParent<CoinShop>();
            coinShop.ShowMessage();
            return;
        }

        Dictionary<string, object> data = new Dictionary<string, object>();

        // Add the specified amount of power ups to the inventory
        foreach (TypePowerUp item in typePowerUp)
        {
            Inventory.Instance.InventoryItems[item] += amountOfPPowerUp;

            Dictionary<string, object> subData = new Dictionary<string, object> { { "amount", Inventory.Instance.InventoryItems[item] } };

            // If the power ups are infinite for a certain time, set a timer
            if (infiniteTimePowerUp != 0)
            {
                foreach (PowerUp powerUp in inventoryPowersUp)
                {
                    if (powerUp.TypePowerUp == item)
                    {
                        StartCoroutine(InternetTime.Instance.GetInternetTime(currentTime =>
                        {
                            if (powerUp.TimeRemainingInSeconds != 0)
                            {
                                powerUp.CurrentTime = currentTime;
                                powerUp.SubtractTimeTimer();
                            }
                            powerUp.MakeInfinitePowerUp(infiniteTimePowerUp, currentTime);

                            int time = (int)powerUp.TimeRemainingInSeconds / 60;

                            subData.Add("time", time);
                            subData.Add("last date", currentTime);

                            data.Add(item.ToString(), subData);

                            Inventory.Instance.SaveDataBase(data);
                        }));
                        break;
                    }
                }
            }
            else
            {
                data.Add(item.ToString(), subData);

                Inventory.Instance.SaveDataBase(data);
            }

        }

        // Subtract the value of the power ups from the player's coins
        CoinController.Instance.ChangeCoins(-valueOfPPowerUp);
    }

    bool CheckIsPowerUpIsActive()
    {
        TMP_Text text = GameObject.FindGameObjectWithTag("text").GetComponent<TMP_Text>();
        foreach (TypePowerUp item in typePowerUp)
        {
            foreach (PowerUp powerUp in inventoryPowersUp)
            {
                if (powerUp.TypePowerUp == item)
                {
                    if (!powerUp.GetComponent<PowerUp>().IsActive)
                        return false;
                }
            }
        }

        return true;
    }
}
