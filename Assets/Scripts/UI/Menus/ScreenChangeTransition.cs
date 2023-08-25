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

    /// <summary>
    /// Fades in the canvas.
    /// </summary>
    public IEnumerator FadeIn()
    {
        yield return new WaitForSecondsRealtime(1);
        canvas.enabled = false;
    }

    /// <summary>
    /// Fades out the canvas, triggers a transition animation, and loads the specified scene.
    /// </summary>
    /// <param name="nameScene">The name of the scene to load.</param>
    /// <param name="waitForUser">Whether to wait for user interaction before loading.</param>
    public IEnumerator FadeOut(string nameScene, bool waitForUser = false)
    {
        canvas.enabled = true;
        animator.SetTrigger("Transition");
        yield return new WaitForSecondsRealtime(1);

        if (waitForUser)
        {
            while (!GameManager.Instance.UserAlready)
            {
                yield return null;
            }
        }

        SceneManager.LoadScene(nameScene);
    }
}