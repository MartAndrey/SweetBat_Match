using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] AudioSource audioSourceLoginUI;

    /// <summary>
    /// Plays the screen change transition and fades out to the level menu.
    /// </summary>
    public void Play()
    {
        StartCoroutine(ScreenChangeTransition.Instance.FadeOut("LevelMenu"));
    }

    /// <summary>
    /// Logs in by activating the specified screen and fading it in.
    /// </summary>
    /// <param name="screen">The screen GameObject to be logged in.</param>
    public void Login(GameObject screen)
    {
        screen.SetActive(true);
        CanvasGroup canvasGroup = screen.GetComponent<CanvasGroup>();

        canvasGroup.alpha = 0;
        canvasGroup.DOFade(1, 0.5f);
    }

    /// <summary>
    /// Closes the UI login by playing the login UI audio, fading out the screen, and deactivating it when complete.
    /// </summary>
    /// <param name="screen">The screen GameObject to be closed.</param>
    public void CloseUILogin(GameObject screen)
    {
        audioSourceLoginUI.Play();

        screen.SetActive(true);
        CanvasGroup canvasGroup = screen.GetComponent<CanvasGroup>();

        canvasGroup.alpha = 1;
        canvasGroup.DOFade(0, 0.5f).OnComplete(() => screen.SetActive(false));
    }
}