using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplicationFactor : MonoBehaviour
{
    public static MultiplicationFactor Instance;

    // Returns the multiplication value
    public int GetMultiplicationFactor { get { return multiplicationFactor; } }

    [Tooltip("Multiplication factor container")]
    [SerializeField] GameObject boxMultiplicationFactor;
    [Tooltip("Multiplication Factor Container Animator")]
    [SerializeField] Animator animator;
    [SerializeField] int multiplicationFactor;
    [SerializeField] AudioClip[] combosAudio;

    AudioSource audioSource;

    // Time in which the multiplication factor is active
    float timeToDisable;

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

        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (timeToDisable > 0 && boxMultiplicationFactor.activeSelf)
        {
            timeToDisable -= Time.deltaTime;

            if (timeToDisable < 0)
            {
                if (multiplicationFactor > 1)
                {
                    GUIManager.Instance.Score += BoardManager.Instance.SumScore * (multiplicationFactor - 1);
                }

                boxMultiplicationFactor.SetActive(false);
                ResetMultiplicationFactor();
            }
        }
    }

    public void SetMultiplicationFactor()
    {
        timeToDisable = 2;
        multiplicationFactor++;

        if (multiplicationFactor <= 0) return;

        boxMultiplicationFactor.SetActive(true);

        GUIManager.Instance.MultiplicationFactor = multiplicationFactor;

        switch (multiplicationFactor)
        {
            case 1:
                audioSource.PlayOneShot(combosAudio[0]);
                break;

            case 2:
                audioSource.PlayOneShot(combosAudio[1]);
                animator.SetTrigger("MultiplicationFactor");
                break;

            default:
                audioSource.PlayOneShot(combosAudio[2]);
                animator.SetTrigger("MultiplicationFactor");
                break;
        }
    }

    public void ResetMultiplicationFactor()
    {
        multiplicationFactor = 0;
        BoardManager.Instance.SumScore = 0;
    }
}