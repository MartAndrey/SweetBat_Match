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

    /// <summary>
    /// Handles the logic for buying additional lives.
    /// </summary>
    public void Buy()
    {
        // If Infinite Life is active, do nothing.
        if (LifeController.Instance.IsInfinite) return;

        // Check if the player has enough coins to purchase a life.
        if (CoinController.Instance.Coins < valueOfLife)
        {
            // Redirect to the Life Shop if coins are insufficient.
            LifeShop lifeShop = FindObjectOfType<LifeShop>();
            lifeShop.SendCoinShop();
            return;
        }

        // Check if adding the specified number of lives exceeds the maximum allowed.
        if (LifeController.Instance.Lives + amountOfLife > LifeController.Instance.MaxLives)
        {
            // Display a warning if the maximum lives limit is reached.
            if (!LifeShop.Instance.WarningText) LifeShop.Instance.WarningText = true;
            return;
        }

        // If Infinite Life is available and affordable, activate it.
        if (isInfiniteLife && valueOfLife <= CoinController.Instance.Coins)
        {
            LifeController.OnInfiniteLife?.Invoke();
            LifeController.Instance.SetTimer(infiniteLifeTimer);
            LifeController.Instance.SaveLivesDataBase(0, infiniteLifeTimer);
            CoinController.Instance.ChangeCoins(-valueOfLife);
            return;
        }

        // Increase the player's lives and deduct the corresponding coins.
        LifeController.Instance.ChangeLives(amountOfLife, true);
        CoinController.Instance.ChangeCoins(-valueOfLife);
    }
}