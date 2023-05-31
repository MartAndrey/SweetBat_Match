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

        screen.transform.localScale = new Vector2(0, 0);
        screen.transform.DOScale(new Vector2(1, 1), 0.8f);

        screen.transform.localRotation = Quaternion.Euler(0, 0, 180);
        screen.transform.DOLocalRotate(new Vector3(0, 0, 0), 0.8f);

        canvasGroup.alpha = 0;
        canvasGroup.DOFade(1, 0.8f);
    }

    public void CloseUILogin(GameObject screen)
    {
        audioSourceLoginUI.Play();

        screen.SetActive(true);
        CanvasGroup canvasGroup = screen.GetComponent<CanvasGroup>();

        screen.transform.localScale = new Vector2(1, 1);
        screen.transform.DOScale(new Vector2(0, 0), 0.8f);

        screen.transform.localRotation = Quaternion.Euler(0, 0, 0);
        screen.transform.DOLocalRotate(new Vector3(0, 0, 180), 0.8f);

        canvasGroup.alpha = 1;
        canvasGroup.DOFade(0, 0.8f).OnComplete(() => screen.SetActive(false));
    }
}