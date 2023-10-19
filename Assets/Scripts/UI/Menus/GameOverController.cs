using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

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
    [SerializeField] TMP_Text informationText;
    // Text object displaying the current level in the UI
    [SerializeField] GameObject particleSystemEnergy;
    [SerializeField] UpdateScoreUI updateScoreUI;
    [SerializeField] TimerGame timerGame;

    AudioSource audioSource;
    string moreMoves = "Add 3 moves!";
    string moreTime;
    bool alreadyShow;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        audioSource = GetComponent<AudioSource>();
        moreTime = $"Add {GameManager.Instance.TimeToMatch} seconds!";
    }

    void Start()
    {
        informationText.text = GUIManager.Instance.GamePlayMode == GamePlayMode.MovesLimited ? moreMoves : moreTime;
    }

    /// <summary>
    /// Handles actions when the game is over.
    /// </summary>
    public void OnGameOver()
    {
        GUIManager.Instance.AlreadyLoseGame = true;

        if (GameManager.Instance.GameMode == GameMode.TimeObjective)
        {
            StopCoroutine(GUIManager.Instance.TimeToMatchCoroutine());
            timerGame.StopTimer();
        }

        particleSystemEnergy.SetActive(true);
        levelText.text = string.Format($"Level {GameManager.Instance.CurrentLevel + 1}");
        audioSourceCamera.Stop();
        boxGameOver.SetActive(true);
        overlay.SetActive(true);
        audioSourceBoxGameOver.PlayDelayed(1f);
        StartCoroutine(updateScoreUI.UpdateScoreRutiner());

        // Reduce lives if not infinite lives
        if (!alreadyShow && !LifeController.Instance.IsInfinite)
        {
            StartCoroutine(ActiveHeartBroken());
            LifeController.Instance.ChangeLives(-1);
        }

        GameManager.Instance.LoseGame();
    }

    IEnumerator ActiveHeartBroken()
    {
        yield return new WaitForSecondsRealtime(1);
        particleSystemBrokenHeart.SetActive(true);
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
        AdsManager.Instance.ShowRewardedAd(AdsManager.Instance.RewardedIdContinueLevel);
        AdsManager.Instance.LoadRewardedAd(AdsManager.Instance.RewardedIdContinueLevel);
    }

    public void HideScreen()
    {
        alreadyShow = true;
        GUIManager.Instance.AlreadyLoseGame = false;
        particleSystemEnergy.SetActive(false);
        audioSourceCamera.Play();
        boxGameOver.GetComponent<Animator>().SetTrigger("Transition");
        StartCoroutine(OffBoxRutiner());
        overlay.SetActive(false);
        audioSourceBoxGameOver.Stop();
    }

    IEnumerator OffBoxRutiner()
    {
        yield return new WaitForSeconds(1);
        boxGameOver.SetActive(false);
    }
}