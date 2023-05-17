using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplicationFactor : MonoBehaviour
{
    public static MultiplicationFactor Instance;

    // Checks the current multiplication factor.
    public int CheckMultiplicationFactor { get { return multiplicationFactor; } set { multiplicationFactor = value; } }
    // Checks if the multiplication factor is active.
    public bool IsActiveMultiplication { get { return isActiveMultiplication; } set { isActiveMultiplication = value; } }

    [Tooltip("Multiplication factor container")]
    // Reference to the multiplication factor container GameObject.
    [SerializeField] GameObject boxMultiplicationFactor;
    [Tooltip("Multiplication Factor Container Animator")]
    // Reference to the multiplication factor container Animator.
    [SerializeField] Animator animator;
    // Current multiplication factor.
    [SerializeField] int multiplicationFactor;
    // Audio clips for different combos.
    [SerializeField] AudioClip[] combosAudio;

    // Flag indicating if the multiplication factor is active.
    bool isActiveMultiplication;
    // Reference to the AudioSource component.
    AudioSource audioSource;

    void OnEnable()
    {
        if (GameManager.Instance.GameMode == GameMode.ScoringObjective)
            GameManager.Instance.OnUniqueMatches.AddListener(IncreaseMultiplicationFactor);
    }

    void OnDisable()
    {
        if (GameManager.Instance.GameMode == GameMode.ScoringObjective)
            GameManager.Instance.OnUniqueMatches.RemoveListener(IncreaseMultiplicationFactor);
    }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Sets the multiplication factor randomly based on a range of values.
    /// </summary>
    public void SetMultiplicationFactorRandom()
    {
        float rn = Random.Range(0.0f, 1f);

        if (rn < 0.3f)
        {
            isActiveMultiplication = true;
            boxMultiplicationFactor.SetActive(true);
        }
    }

    /// <summary>
    /// Increases the multiplication factor and performs actions based on the new factor value.
    /// </summary>
    void IncreaseMultiplicationFactor()
    {
        if (!isActiveMultiplication) return;

        multiplicationFactor++;

        if (multiplicationFactor <= 0) return;

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

    /// <summary>
    /// Resets the multiplication factor to zero and updates related values.
    /// </summary>
    public void ResetMultiplicationFactor()
    {
        multiplicationFactor = 0;
        GUIManager.Instance.MultiplicationFactor = multiplicationFactor;
        BoardManager.Instance.SumScore = 0;
    }

    /// <summary>
    /// Coroutine to disable the multiplication factor after a delay.
    /// </summary>
    public IEnumerator DisableMultiplication()
    {
        yield return new WaitForSeconds(1);

        GUIManager.Instance.Score += BoardManager.Instance.SumScore * multiplicationFactor;
        boxMultiplicationFactor.SetActive(false);
        isActiveMultiplication = false;
        ResetMultiplicationFactor();
    }
}