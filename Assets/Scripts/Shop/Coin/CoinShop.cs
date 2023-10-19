using System.Collections;
using UnityEngine;
using DG.Tweening;

public class CoinShop : MonoBehaviour
{
    [SerializeField] GameObject boxMenu;
    [SerializeField] GameObject closeButton;
    [SerializeField] GameObject overlay;
    [SerializeField] GameObject message;

    bool showMessageTransition;

    /// <summary>
    /// Hides a specific menu and performs a transition.
    /// </summary>
    /// <param name="screen">The screen to be shown after hiding the menu.</param>
    public void HideMenu(GameObject screen)
    {
        boxMenu.GetComponentInParent<Animator>().SetTrigger("Transition");
        StartCoroutine(OffScreenAdvanceRutiner(screen));
    }

    /// <summary>
    /// Routine to hide the menu and show the desired screen.
    /// </summary>
    /// <param name="screen">The screen to be shown after hiding the menu.</param>
    IEnumerator OffScreenAdvanceRutiner(GameObject screen)
    {
        yield return new WaitForSeconds(1.1f);
        boxMenu.SetActive(false);
        closeButton.SetActive(false);
        yield return new WaitForSeconds(0.3f);
        screen.SetActive(false);
        boxMenu.SetActive(true);
        closeButton.SetActive(true);
    }

    /// <summary>
    /// Handles obtaining coins through ads.
    /// </summary>
    public void GetCoinAds()
    {
        AdsManager.Instance.ShowRewardedAd(AdsManager.Instance.RewardedIdCoins);
        AdsManager.Instance.LoadRewardedAd(AdsManager.Instance.RewardedIdCoins);
    }

    /// <summary>
    /// Displays a message.
    /// </summary>
    public void ShowMessage()
    {
        if (showMessageTransition) return;

        showMessageTransition = true;
        message.SetActive(true);
        message.GetComponent<CanvasGroup>().DOFade(1, 1f);

        HideMessage();
    }

    /// <summary>
    /// Hides a message after a certain amount of time.
    /// </summary>
    void HideMessage()
    {
        StartCoroutine(HideMessageRutiner());
    }

    /// <summary>
    /// Routine to hide the message after a certain amount of time.
    /// </summary>
    IEnumerator HideMessageRutiner()
    {
        yield return new WaitForSecondsRealtime(4);

        message.GetComponent<CanvasGroup>().DOFade(0, 1f).OnComplete(() =>
        {
            message.SetActive(false);
            showMessageTransition = false;
        });
    }
}