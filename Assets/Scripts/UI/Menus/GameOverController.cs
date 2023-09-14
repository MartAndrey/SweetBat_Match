using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

[RequireComponent(typeof(UpdateScoreUI))]
public class GameOverController : MonoBehaviour
{
    public static GameOverController Instance;

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
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Handles actions when the game is over.
    /// </summary>
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

        // Reduce lives if not infinite lives
        if (!LifeController.Instance.IsInfinite)
            LifeController.Instance.ChangeLives(-1);
    }

    /// <summary>
    /// Handles replaying the level if lives are available.
    /// </summary>
    public void Replay()
    {
        if (LifeController.Instance.HasLives || LifeController.Instance.IsInfinite)
        {
            audioSource.PlayOneShot(popComplete);
            StartCoroutine(ScreenChangeTransition.Instance.FadeOut(SceneManager.GetActiveScene().name));
            Inventory.Instance.ResetParentPowerUps(false);
        }
        else
        {
            StartCoroutine(ScreenChangeTransition.Instance.FadeOut("LevelMenu"));
            Inventory.Instance.ResetParentPowerUps(true);
        }
    }

    public void Ads()
    {
        audioSource.PlayOneShot(popComplete);
        Debug.Log("Ads");
        // TODO: Ads
    }
}