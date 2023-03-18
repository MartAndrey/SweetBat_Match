using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonValueOfCoin : MonoBehaviour
{
    [SerializeField] TypePowerUp[] typePowerUp;
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
        Debug.Log("Hi");
    }
}
