using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(UpdateScoreUI))]
public class CompleteGameController : MonoBehaviour
{
    [SerializeField] GameObject overlay;
    [SerializeField] GameObject boxCompleteGame;
    [SerializeField] GameObject particleSystemEnergy;
    [SerializeField] GameObject[] boxStars;
    [SerializeField] UpdateScoreUI updateScoreUI;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip popComplete;

    /// <summary>
    /// Displays UI elements and particle system when the game is completed
    /// </summary>
    public void OnCompleteGame()
    {
        // Show overlay and complete game box
        overlay.SetActive(true);
        boxCompleteGame.SetActive(true);
        // Activate energy particle system
        particleSystemEnergy.SetActive(true);

        int activeStars = ProgressBar.Instance.GetActiveStars();
        // Get the number of active stars from progress bar and animate them
        StartCoroutine(ActiveStars(activeStars));
        // Update the score UI
        StartCoroutine(updateScoreUI.UpdateScoreRutiner());
    }

    /// <summary>
    /// Coroutine to activate the stars in sequence with animation
    /// </summary>
    /// <param name="activeStars">Number of active stars</param>
    IEnumerator ActiveStars(int activeStars)
    {
        // Wait for 1.2 seconds before starting the animation
        yield return new WaitForSeconds(1.2f);
        // Activate each star in sequence with animation
        for (int i = 0; i < activeStars; i++)
        {
            yield return new WaitForSeconds(.07f);
            boxStars[i].SetActive(true);
            boxStars[i].GetComponentInParent<Animator>().enabled = true;
        }
    }

    /// <summary>
    /// Plays a sound effect and initiates screen transition to replay the game
    /// </summary>
    public void Replay()
    {
        audioSource.PlayOneShot(popComplete);
        StartCoroutine(ScreenChangeTransition.Instance.FadeOut(SceneManager.GetActiveScene().name));
    }

    /// <summary>
    /// Plays a sound effect to indicate next level
    /// </summary>
    public void NextLevel()
    {
        audioSource.PlayOneShot(popComplete);
    }
}