using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SocialController : MonoBehaviour
{
    [SerializeField] GameObject boxMenu;

    public void HideMenu(GameObject screen)
    {
        boxMenu.GetComponentInParent<Animator>().SetTrigger("Transition");
        StartCoroutine(OffScreenAdvanceRutiner(screen));
    }

    IEnumerator OffScreenAdvanceRutiner(GameObject screen)
    {
        yield return new WaitForSeconds(1.1f);
        boxMenu.SetActive(false);
        yield return new WaitForSeconds(0.3f);
        screen.SetActive(false);
        boxMenu.SetActive(true);
    }
}
