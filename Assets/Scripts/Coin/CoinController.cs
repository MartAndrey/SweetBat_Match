using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class CoinController : MonoBehaviour
{
    public TMP_Text text;
    public static CoinController Instance;

    public int Coins { get { return coins; } }

    [SerializeField] int coins;
    [SerializeField] TMP_Text coinsText;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
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
        text = GameObject.FindGameObjectWithTag("text").GetComponent<TMP_Text>();
        text.text = " Start Find ";
        Dictionary<string, object> data = GameManager.Instance.CollectiblesData;

        if (data != null && data.Count > 0)
        {
            if (data.ContainsKey("coins"))
            {
                int coins = Convert.ToInt32(data["coins"]);
                ChangeCoins(coins, false);
                return;
            }
        }

        if (!GameManager.Instance.UserIsAnonymous)
            Invoke("GetRewardCoins", 2f);

        Dictionary<string, object> coinsData = new Dictionary<string, object> { { "coins", 0 } };
        // GameManager.Instance.CollectiblesData.Add(coinsData);
        CloudFirestore.Instance.SetCollectible(coinsData);
        ChangeCoins(0);
    }

    void GetRewardCoins()
    {
        Reward reward = FindObjectOfType<Reward>(true);
        if (reward != null) reward.ActivateReward();
    }

    /// <summary>
    /// Increases or decreases the number of coins by the specified amount and updates the coins text UI element.
    /// </summary>
    /// <param name="amount">The amount by which to change the number of coins.</param>
    public void ChangeCoins(int amount, bool saveDataBase = true)
    {
        coins += amount;
        coinsText.text = coins.ToString();

        if (saveDataBase)
            SaveCoinsDataBase();
    }

    public void SaveCoinsDataBase()
    {
        Dictionary<string, object> coins = new Dictionary<string, object> { { "coins", this.coins } };
        CloudFirestore.Instance.SetCollectible(coins);
    }
}