using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LifeShop : MonoBehaviour
{
    [Header("Life")]
    [SerializeField] TMP_Text livesText;
    [SerializeField] GameObject imageInfiniteLife;

    [Header("Timer")]
    [SerializeField] TMP_Text timerLivesText;

    [Header("Coins")]
    [SerializeField] TMP_Text coinsText;

    void OnEnable()
    {
        LifeController.OnInfiniteLife += ChangeImageInfiniteLife;
        imageInfiniteLife.SetActive(LifeController.Instance.InfiniteLife ? true : false);
        livesText.enabled = LifeController.Instance.InfiniteLife ? false : true;
    }

    void OnDisable()
    {
        LifeController.OnInfiniteLife -= ChangeImageInfiniteLife;
    }


    void Start()
    {
        SetLifeTimerCoins();
    }

    void Update()
    {
        if ((livesText.text != LifeController.Instance.Lives.ToString())
        || (timerLivesText.text != LifeController.Instance.WaitTime.text)
        || coinsText.text != CoinController.Instance.Coins.ToString())
        {
            SetLifeTimerCoins();
        }
    }

    void SetLifeTimerCoins()
    {
        livesText.text = LifeController.Instance.Lives.ToString();
        timerLivesText.text = LifeController.Instance.WaitTime.text;
        coinsText.text = CoinController.Instance.Coins.ToString();
    }

    // Method in charge of changing the UI when it changes to infinity or not
    void ChangeImageInfiniteLife()
    {
        livesText.enabled = !livesText.isActiveAndEnabled;
        imageInfiniteLife.SetActive(!imageInfiniteLife.activeSelf);
    }
}