using System.Collections;
using UnityEngine;

public class CoinShop : MonoBehaviour
{
    [SerializeField] GameObject boxMenu;
    [SerializeField] GameObject closeButton;
    [SerializeField] GameObject overlay;

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
}