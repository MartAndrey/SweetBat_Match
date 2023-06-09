using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public class Test : MonoBehaviour
{
    [SerializeField] GameObject loadingLogOut;
    [SerializeField] Image checkLogOut;
    [SerializeField] TMP_Text textButtonLogOut;
    void Start()
    {
        StartCoroutine(Rutiner());
    }
    [ContextMenu("gdfgd")] public void fsdfs() { StartCoroutine(Rutiner()); }
    IEnumerator Rutiner()
    {
        yield return new WaitForSeconds(2);
        checkLogOut.DOFade(1, 0.5f);
        checkLogOut.transform.DOScale(Vector3.one * 1.3f, 0.25f).OnComplete(() =>
       {
           checkLogOut.transform.DOScale(Vector3.one, 0.1f);
       });

        yield return new WaitForSeconds(2);


    }
}
