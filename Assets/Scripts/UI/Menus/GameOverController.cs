using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

[RequireComponent(typeof(UpdateScoreUI))]
public class GameOverController : MonoBehaviour
{
    [Header("UI Game Over")]
    [SerializeField] GameObject boxGameOver;
    [SerializeField] GameObject particleSystemBrokenHeart;
    [SerializeField] Animator boxAnimatorGameOver;
    [SerializeField] GameObject overlay;
    [SerializeField] AudioClip popComplete;
    // Sound effect for when the score increases
    [SerializeField] AudioClip scoreIncreasing;
    [SerializeField] AudioSource audioSourceCamera;
    [SerializeField] AudioSource audioSourceBoxGameOver;
    // Text object displaying the current level in the UI
    [SerializeField] TMP_Text levelText;
    // Text object displaying the current level in the UI
    [SerializeField] GameObject particleSystemEnergy;
    [SerializeField] UpdateScoreUI updateScoreUI;

    AudioSource audioSource;
    // Text object displaying the current level in the UI

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void OnGameOver()
    {
        particleSystemEnergy.SetActive(true);
        levelText.text = string.Format($"Level {GameManager.Instance.CurrentLevel + 1}");
        audioSourceCamera.Stop();
        boxGameOver.SetActive(true);
        overlay.SetActive(true);
        particleSystemBrokenHeart.SetActive(true);
        audioSourceBoxGameOver.PlayDelayed(1f);
        StartCoroutine(updateScoreUI.UpdateScoreRutiner());
    }

    public void Replay()
    {
        audioSource.PlayOneShot(popComplete);
        StartCoroutine(ScreenChangeTransition.Instance.FadeOut(SceneManager.GetActiveScene().name));
    }

    public void Ads()
    {
        audioSource.PlayOneShot(popComplete);
        Debug.Log("Ads");
        // TODO: Ads
    }
}