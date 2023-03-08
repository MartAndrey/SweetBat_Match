using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LifeShop : MonoBehaviour
{
    [Header("Life")]
    [SerializeField] TMP_Text livesText;

    [Header("Timer")]
    [SerializeField] TMP_Text timerLivesText;

    void Start()
    {
        SetLifeAndTimer();
    }

    void Update()
    {
        if ((livesText.text != LifeController.Instance.Lives.ToString()) || (timerLivesText.text != LifeController.Instance.WaitTime.text))
        {
            SetLifeAndTimer();
        }
    }

    void SetLifeAndTimer()
    {
        livesText.text = LifeController.Instance.Lives.ToString();
        timerLivesText.text = LifeController.Instance.WaitTime.text;
    }
}