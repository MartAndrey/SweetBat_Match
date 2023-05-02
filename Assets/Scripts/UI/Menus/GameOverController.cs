using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverController : MonoBehaviour
{
    [Header("UI Game Over")]
    [SerializeField] GameObject boxGameOver;
    [SerializeField] GameObject particleSystemBrokenHeart;
    [SerializeField] Animator boxAnimatorGameOver;
    [SerializeField] GameObject overlay;
    [SerializeField] AudioClip popComplete;
    // Sound effect for when the score increases
    [SerializeField] AudioClip scoreIncreasing;
    // Audio source used to play the score increasing sound effect
    [SerializeField] AudioSource audioSourceScore;
    [SerializeField] AudioSource audioSourceCamera;
    [SerializeField] AudioSource audioSourceBoxGameOver;
    // Text object displaying the current level in the UI
    [SerializeField] TMP_Text levelText;
    // Text object displaying the current level in the UI
    [SerializeField] TMP_Text scoreText;
    // Text object displaying the current level in the UI
    [SerializeField] GameObject particleSystemEnergy;

    AudioSource audioSource;
    // Text object displaying the current level in the UI
    int displayScore = 0;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void OnGameOver()
    {
        particleSystemEnergy.SetActive(true);
        levelText.text = string.Format($"Level {GameManager.Instance.Level}");
        audioSourceCamera.Stop();
        boxGameOver.SetActive(true);
        overlay.SetActive(true);
        particleSystemBrokenHeart.SetActive(true);
        audioSourceBoxGameOver.PlayDelayed(1f);
        StartCoroutine(UpdateScoreRutiner());
    }

    public void Replay()
    {
        audioSource.PlayOneShot(popComplete);
        StartCoroutine(ScreenChangeTransition.Instance.FadeOut(SceneManager.GetActiveScene().name));
    }

    /// <summary>
    /// Coroutine that updates the displayed score in the UI over time
    /// </summary>
    IEnumerator UpdateScoreRutiner()
    {
        // Wait for 1.1 seconds before starting the routine
        yield return new WaitForSeconds(1.1f);

        audioSourceScore.Play(); // Play the score increasing sound effect

        // Continuously update the displayed score in the UI until it matches the current score
        while (displayScore < GUIManager.Instance.Score)
        {
            // Increment the displayed score and update the score text in the UI
            displayScore++;
            scoreText.text = displayScore.ToString();
            // Wait for the next frame
            yield return null;
        }
        // Update the score text in the UI one last time to match the final score
        scoreText.text = GUIManager.Instance.Score.ToString();
        // Stop playing the score increasing sound effect
        audioSourceScore.Stop();

        yield return null;
    }

    public void Ads()
    {
        audioSource.PlayOneShot(popComplete);
        Debug.Log("Ads");
        // TODO: Ads
    }
}