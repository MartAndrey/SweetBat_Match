using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    [Header("Button Pause")]
    [SerializeField] GameObject boxPause;
    [SerializeField] GameObject overlay;
    [SerializeField] GameObject particleSystemEnergy;
    [SerializeField] Animator boxAnimator;
    [SerializeField] AudioClip popComplete;
    
    [Header("Button Confirmation Quit")]
    [SerializeField] GameObject confirmationQuit;
    [SerializeField] Animator confirmationQuitAnimator;

    AudioSource audioSource;

    // bool isAnimationTransition = false;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        // particleSystemEnergy.GetComponent<ParticleSystem>().Stop();
    }

    public void OnPause()
    {
        Time.timeScale = 0;
        overlay.SetActive(true);
        boxPause.SetActive(true);
        // particleSystemEnergy.GetComponent<ParticleSystem>().Play();
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

    public void Replay()
    {
        audioSource.PlayOneShot(popComplete);
        StartCoroutine(ScreenChangeTransition.Instance.FadeOut(SceneManager.GetActiveScene().name));
        Time.timeScale = 1;
    }

    public void Quit(bool changeTimeScale)
    {
        audioSource.PlayOneShot(popComplete);
        OffPause(changeTimeScale);
        StartCoroutine(ConfirmationQuitOnRutiner());
    }

    IEnumerator ConfirmationQuitOnRutiner()
    {
        yield return new WaitForSecondsRealtime(1);

        confirmationQuit.SetActive(true);
        particleSystemEnergy.SetActive(true);
    }

    IEnumerator ConfirmationQuitOffRutiner()
    {
        yield return null;

    }

    // IEnumerator CheckTransition()
    // {
    //     isAnimationTransition = true;
    //     yield return new WaitForSecondsRealtime(1);
    //     isAnimationTransition = false;
    // }
}