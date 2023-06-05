using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] AudioSource audioSourceLoginUI;

    public void Play()
    {
        StartCoroutine(ScreenChangeTransition.Instance.FadeOut("LevelMenu"));
    }

    public void Login(GameObject screen)
    {
        screen.SetActive(true);
        CanvasGroup canvasGroup = screen.GetComponent<CanvasGroup>();

        canvasGroup.alpha = 0;
        canvasGroup.DOFade(1, 0.8f);
    }

    public void CloseUILogin(GameObject screen)
    {
        audioSourceLoginUI.Play();

        screen.SetActive(true);
        CanvasGroup canvasGroup = screen.GetComponent<CanvasGroup>();

        canvasGroup.alpha = 1;
        canvasGroup.DOFade(0, 0.8f).OnComplete(() => screen.SetActive(false));
    }
}