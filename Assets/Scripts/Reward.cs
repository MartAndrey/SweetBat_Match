using UnityEngine;
using TMPro;
using DG.Tweening;

public class Reward : MonoBehaviour
{
    [SerializeField] TMP_Text textHeadMessage;
    [SerializeField] AudioClip audioPop;
    [SerializeField] AudioClip audioGetCoins;
    [SerializeField] GameObject coinObject;
    [SerializeField] GameObject rewardCoinObject;

    float time = .6f;
    int rewardCoins = 100;
    string nameUser;
    AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        nameUser = (string)GameManager.Instance.UserData["name"];

        textHeadMessage.text = string.Format($"Hello, {nameUser.Split(' ')[0]}!");
    }

    public void ActivateReward()
    {
        this.gameObject.SetActive(true);
    }

    public void ClaimReward()
    {
        rewardCoinObject.SetActive(true);
        audioSource.PlayOneShot(audioPop);
        rewardCoinObject.transform.DOMove(coinObject.transform.position, time).OnComplete(() =>
        {
            audioSource.PlayOneShot(audioGetCoins);

            rewardCoinObject.GetComponent<CanvasGroup>().DOFade(0, time).OnComplete(() =>
            {
                CoinController.Instance.ChangeCoins(rewardCoins);
                LevelMenuController levelMenuController = FindObjectOfType<LevelMenuController>();
                if (levelMenuController != null) levelMenuController.OffScreen(gameObject);
            });
        });
    }
}
