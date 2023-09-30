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

    public void HideMenu(GameObject screen)
    {
        boxMenu.GetComponentInParent<Animator>().SetTrigger("Transition");
        StartCoroutine(OffScreenAdvanceRutiner(screen));
    }

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

    public void GetCoinAds()
    {
        Debug.LogWarning("TODO: Show Ads And Give Coin");
    }

    public void ShowMessage()
    {
        if (showMessageTransition) return;

        showMessageTransition = true;
        message.SetActive(true);
        message.GetComponent<CanvasGroup>().DOFade(1, 1f);

        HideMessage();
    }

    void HideMessage()
    {
        StartCoroutine(HideMessageRutiner());
    }

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