using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenChangeTransition : MonoBehaviour
{
    public static ScreenChangeTransition Instance;

    [SerializeField] Canvas canvas;

    Animator animator;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        animator = GetComponent<Animator>();

        StartCoroutine(FadeIn());
    }

    public IEnumerator FadeIn()
    {
        yield return new WaitForSecondsRealtime(1);
        canvas.enabled = false;        
    }

    public IEnumerator FadeOut(string nameScene)
    {
        canvas.enabled = true;
        animator.SetTrigger("Transition");
        yield return new WaitForSecondsRealtime(1);
        SceneManager.LoadScene(nameScene);
    }
}