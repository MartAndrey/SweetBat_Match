using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
    [Header("UI Game Over")]
    [SerializeField] GameObject boxGameOver;
    [SerializeField] GameObject particleSystemBrokenHeart;
    [SerializeField] Animator boxAnimatorGameOver;
    [SerializeField] GameObject overlay;
    [SerializeField] AudioClip popComplete;
    [SerializeField] AudioSource audioSourceCamera;
    [SerializeField] AudioSource audioSourceBoxGameOver;

    AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void OnGameOver()
    {
        audioSourceCamera.Stop();
        boxGameOver.SetActive(true);
        overlay.SetActive(true);
        particleSystemBrokenHeart.SetActive(true);
        audioSourceBoxGameOver.PlayDelayed(1f);
    }

    public void OffGameOver()
    {

    }

    public void Replay()
    {
        audioSource.PlayOneShot(popComplete);
        StartCoroutine(ScreenChangeTransition.Instance.FadeOut(SceneManager.GetActiveScene().name));
    }

    public void Ads()
    {
        Debug.Log("Ads");
        // TODO: Ads
    }

    // IEnumerator OnGameOverRutiner()
    // {
    //     yield return new WaitForSeconds(1);
    //     particleSystemBrokenHeart.SetActive(true);
    // }
}