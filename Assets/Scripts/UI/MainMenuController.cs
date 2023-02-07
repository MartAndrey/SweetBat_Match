using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("Box containing the other settings")]
    [SerializeField] Animator boxAnimator;
    [SerializeField] GameObject boxSettings;

    [Header("Setting button")]
    [SerializeField] Image imageSetting;
    [SerializeField] Sprite settingPurple;
    [SerializeField] Sprite SettingWhite;

    bool isAnimationTransition = false;

    public void Play()
    {
        StartCoroutine(ScreenChangeTransition.Instance.FadeOut("LevelMenu"));
    }

    // This method is called when the configure button is clicked     
    public void OnSetting()
    {
        if (isAnimationTransition) return;

        StartCoroutine(CheckTransition());

        if (!boxSettings.activeSelf)
        {
            boxSettings.SetActive(true);
            imageSetting.sprite = SettingWhite;
        }
        else
        {
            boxAnimator.SetTrigger("Transition");
            StartCoroutine(OffSetting());
            imageSetting.sprite = settingPurple;
        }
    }

    IEnumerator OffSetting()
    {
        yield return new WaitForSeconds(1);
        boxSettings.SetActive(false);
    }

    IEnumerator CheckTransition()
    {
        isAnimationTransition = true;
        yield return new WaitForSecondsRealtime(1);
        isAnimationTransition = false;
    }
}