using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class CoinController : MonoBehaviour
{
    public static CoinController Instance;

    public int Coins { get { return coins; } }

    [SerializeField] int coins;
    [SerializeField] TMP_Text coinsText;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        GetCoins();
    }

    /// <summary>
    /// Retrieves the number of coins from the CollectiblesData dictionary and sets the initial coins.
    /// If the coins data is not available, it sets the coins to 0 and initializes the UI accordingly.
    /// </summary>
    void GetCoins()
    {
        Dictionary<string, object> data = GameManager.Instance.CollectiblesData;
        if (data != null && data.Count > 0)
        {
            if (data.ContainsKey("coins"))
            {
                int coins = Convert.ToInt32(data["coins"]);

                SetInitialCoins(coins);

                return;
            }
        }

        CloudFirestore.Instance.SetCollectible(new Dictionary<string, object> { { "coins", 0 } });
        SetInitialCoins(0);
    }

    /// <summary>
    /// Sets the initial number of coins and updates the coins text UI element.
    /// </summary>
    /// <param name="coins">The number of coins to set.</param>
    void SetInitialCoins(int coins)
    {
        this.coins = coins;
        coinsText.text = coins.ToString();
    }

    /// <summary>
    /// Increases or decreases the number of coins by the specified amount and updates the coins text UI element.
    /// </summary>
    /// <param name="amount">The amount by which to change the number of coins.</param>
    public void ChangeCoins(int amount)
    {
        coins += amount;
        coinsText.text = coins.ToString();
    }
}