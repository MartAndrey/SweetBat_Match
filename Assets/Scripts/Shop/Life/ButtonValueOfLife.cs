using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonValueOfLife : MonoBehaviour
{
    [Tooltip("Price of life or how much such lives cost")]
    [SerializeField] int valueOfLife;
    [Tooltip("Amount of lives that will be given in exchange for coins")]
    [SerializeField] int amountOfLife;
    [Tooltip("Boolean that identifies if the number of lives will be infinite or not")]
    [SerializeField] bool isInfiniteLife;
    [Tooltip("Time in which the life will be infinite in case you select an infinite life")]
    [SerializeField] float infiniteLifeTimer;

    // Method in charge of making the purchase and validating all the data
    public void Buy()
    {
        if (LifeController.Instance.IsInfinite) return;

        if (CoinController.Instance.Coins < valueOfLife)
        {
            // TODO: Send to the coin shop
            Debug.Log("You do not have the necessary amount of coins");
            return;
        }

        if (LifeController.Instance.Lives + amountOfLife > LifeController.Instance.MaxLives)
        {
            if (!LifeShop.Instance.WarningText) LifeShop.Instance.WarningText = true;
            return;
        }

        if (isInfiniteLife && valueOfLife <= CoinController.Instance.Coins)
        {
            LifeController.OnInfiniteLife?.Invoke();
            LifeController.Instance.SetTimer(infiniteLifeTimer);
            CoinController.Instance.ChangeCoins(-valueOfLife);
            return;
        }

        LifeController.Instance.ChangeLives(amountOfLife, true);
        CoinController.Instance.ChangeCoins(-valueOfLife);
    }
}