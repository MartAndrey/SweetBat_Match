using System.Collections;
using UnityEngine;

public class PauseMenuController : MonoBehaviour
{
    [Header("Button Pause")]
    [SerializeField] GameObject boxPause;
    [SerializeField] GameObject overlay;
    [SerializeField] GameObject particleSystemEnergy;
    [SerializeField] GameObject particleSystemBrokenHeart;
    [SerializeField] Animator boxAnimator;
    [SerializeField] AudioClip popComplete;

    [Header("Button Confirmation Quit")]
    [SerializeField] GameObject confirmationQuit;
    [SerializeField] Animator confirmationQuitAnimator;

    AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void OnPause()
    {
        Time.timeScale = 0;
        overlay.SetActive(true);
        boxPause.SetActive(true);
        particleSystemEnergy.SetActive(true);
    }

    // It is called when I click the close button
    public void OffPause(bool changeTimeScale)
    {
        particleSystemEnergy.SetActive(false);
        boxAnimator.SetTrigger("Transition");
        StartCoroutine(OffPauseRutiner(changeTimeScale));
    }

    IEnumerator OffPauseRutiner(bool changeTimeScale)
    {
        yield return new WaitForSecondsRealtime(1);

        if (changeTimeScale)
        {
            Time.timeScale = 1;
            overlay.SetActive(false);
        }
        boxPause.SetActive(false);
    }

    /// <summary>
    /// Handles replaying the level after a game over.
    /// </summary>
    public void Replay()
    {
        GameManager.Instance.LoseGame();
        if (!LifeController.Instance.IsInfinite)
        {
            particleSystemBrokenHeart.SetActive(true);
            LifeController.Instance.ChangeLives(-1);
        }
        GameOverController.Instance.Replay();
        Time.timeScale = 1;
    }

    public void Quit(bool changeTimeScale)
    {
        audioSource.PlayOneShot(popComplete);
        OffPause(changeTimeScale);
        StartCoroutine(ConfirmationQuitOnRutiner());
    }

    public void ConfirmationQuitOff()
    {
        audioSource.PlayOneShot(popComplete);
        particleSystemEnergy.SetActive(false);
        confirmationQuitAnimator.SetTrigger("Transition");
        StartCoroutine(ConfirmationQuitOffRutiner());
    }

    /// <summary>
    /// Quits the current level and returns to the level menu.
    /// </summary>
    public void QuitLevel()
    {
        audioSource.PlayOneShot(popComplete);
        StartCoroutine(ScreenChangeTransition.Instance.FadeOut("LevelMenu"));
        if (!LifeController.Instance.IsInfinite)
        {
            particleSystemBrokenHeart.SetActive(true);
            LifeController.Instance.ChangeLives(-1);
        }
        Inventory.Instance.ResetParentPowerUps(true);
        Time.timeScale = 1;
    }

    IEnumerator ConfirmationQuitOnRutiner()
    {
        yield return new WaitForSecondsRealtime(1);

        confirmationQuit.SetActive(true);
        particleSystemEnergy.SetActive(true);
    }

    IEnumerator ConfirmationQuitOffRutiner()
    {
        yield return new WaitForSecondsRealtime(1);
        overlay.SetActive(false);
        confirmationQuit.SetActive(false);
        Time.timeScale = 1;
    }
}