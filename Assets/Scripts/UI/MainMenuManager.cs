using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] GameObject boxSettings;
    [SerializeField] Animator boxAnimator;
    [SerializeField] Image imageSetting;
    [SerializeField] Sprite settingPurple;
    [SerializeField] Sprite SettingWhite;

    public void OnSetting()
    {
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
}