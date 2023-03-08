using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMenuController : MonoBehaviour
{
    [SerializeField] GameObject overlay;
    [SerializeField] GameObject plusLives;

    public void OnPlusLives()
    {
        overlay.SetActive(true);
        plusLives.SetActive(true);
    }

    public void OffPlusLives()
    {
        plusLives.GetComponent<Animator>().SetTrigger("Transition");
        StartCoroutine(OffPlusLivesRutiner());
    }

    IEnumerator OffPlusLivesRutiner()
    {
        yield return new WaitForSeconds(1);
        overlay.SetActive(false);
        plusLives.SetActive(false);
        
    }
}
