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

    AudioSource audioSource;

    // bool isAnimationTransition = false;

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
    public void OffPause()
    {
        particleSystemEnergy.SetActive(false);
        boxAnimator.SetTrigger("Transition");
        StartCoroutine(OffPauseRutiner());
    }

    IEnumerator OffPauseRutiner()
    {
        yield return new WaitForSecondsRealtime(1);
        Time.timeScale = 1;
        boxPause.SetActive(false);
        overlay.SetActive(false);
    }

    public void Replay()
    {
        audioSource.PlayOneShot(popComplete);
        StartCoroutine(ScreenChangeTransition.Instance.FadeOut(SceneManager.GetActiveScene().name));
        Time.timeScale = 1;
    }
    
    // IEnumerator CheckTransition()
    // {
    //     isAnimationTransition = true;
    //     yield return new WaitForSecondsRealtime(1);
    //     isAnimationTransition = false;
    // }
}