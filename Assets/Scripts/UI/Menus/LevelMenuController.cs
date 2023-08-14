using System.Collections;
using UnityEngine;

public class LevelMenuController : MonoBehaviour
{
    [SerializeField] GameObject overlay;

    public void OnScreen(GameObject screen)
    {
        overlay.SetActive(true);
        screen.SetActive(true);
    }

    public void OffScreen(GameObject screen)
    {
        screen.GetComponent<Animator>().SetTrigger("Transition");
        StartCoroutine(OffScreenRutiner(screen));
    }

    IEnumerator OffScreenRutiner(GameObject screen)
    {
        yield return new WaitForSeconds(1);
        overlay.SetActive(false);
        screen.SetActive(false);
    }

    public void OnScreenAdvance(GameObject screen)
    {
        screen.SetActive(true);
    }
}