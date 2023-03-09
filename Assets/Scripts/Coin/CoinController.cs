using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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
        ChangeCoins(0);
    }

    // Responsible method increase or decrease the number of coins
    public void ChangeCoins(int amount)
    {
        coins += amount;
        coinsText.text = coins.ToString();
    }
}