using UnityEngine;
using GoogleMobileAds.Api;
using System.Collections.Generic;
using System.Collections;

public class AdsManager : MonoBehaviour
{
    public static AdsManager Instance;

    public string RewardedIdLife { get { return rewardedIdLife; } }
    public string RewardedIdCoins { get { return rewardedIdCoins; } }
    public string RewardedIdContinueLevel { get { return rewardedIdContinueLevel; } }

    [SerializeField] string appId;
    [SerializeField] string bannerId;
    [SerializeField] string interstitialId;
    [SerializeField] string rewardedIdLife;
    [SerializeField] string rewardedIdCoins;
    [SerializeField] string rewardedIdContinueLevel;

    BannerView bannerView;
    InterstitialAd interstitialAd;
    RewardedAd rewardedAdLife;
    RewardedAd rewardedAdCoins;
    RewardedAd rewardedAdContinueLevel;

    Dictionary<string, RewardedAd> rewardedAds;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        rewardedAds = new Dictionary<string, RewardedAd>()
        {
            // { rewardedIdLife, rewardedAdLife },
            // { rewardedIdCoins, rewardedAdCoins },
            { rewardedIdContinueLevel, rewardedAdContinueLevel },
        };

        MobileAds.Initialize(initStatus =>
        {
            LoadRewardedAd(rewardedIdLife);
            LoadRewardedAd(rewardedIdCoins);
            LoadInterstitialAd();

            RegisterReloadHandler(rewardedAdLife, rewardedIdLife);
            RegisterReloadHandler(rewardedAdCoins, rewardedIdCoins);
            RegisterReloadHandler(interstitialAd);
        });

        DontDestroyOnLoad(gameObject);
    }

    //========================Banner===========================//
    /// <summary>
    /// Creates a 320x50 banner view at top of the screen.
    /// </summary>
    public void CreateBannerView(AdPosition position)
    {
        // If we already have a banner, destroy the old one.
        if (bannerView != null)
            DestroyBannerView();

        // Create a 320x50 banner at top of the screen
        bannerView = new BannerView(bannerId, AdSize.Banner, position);
    }

    /// <summary>
    /// Creates the banner view and loads a banner ad.
    /// </summary>
    public void LoadAd(AdPosition position)
    {
        // create an instance of a banner view first.
        if (bannerView == null)
            CreateBannerView(position);

        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        bannerView.LoadAd(adRequest);
    }

    /// <summary>
    /// Destroys the banner view.
    /// </summary>
    public void DestroyBannerView()
    {
        if (bannerView != null)
        {
            bannerView.Destroy();
            bannerView = null;
        }
    }

    //========================Rewarded===========================//
    /// <summary>
    /// Loads the rewarded ad.
    /// </summary>
    public void LoadRewardedAd(string adId)
    {
        // Clean up the old ad before loading a new one.
        if (rewardedAds[adId] != null)
            DestroyRewardedAd(adId);

        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        RewardedAd.Load(adId, adRequest,
            (RewardedAd ad, LoadAdError error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    return;
                }

                rewardedAds[adId] = ad;
            });
    }

    public void DestroyRewardedAd(string adId)
    {
        if (rewardedAds[adId] != null)
        {
            rewardedAds[adId].Destroy();
            rewardedAds[adId] = null;
        }
    }

    public void ShowRewardedAd(string adId)
    {
        if (rewardedAds[adId] != null && rewardedAds[adId].CanShowAd())
        {
            rewardedAds[adId].Show((GoogleMobileAds.Api.Reward reward) =>
            {
                if (adId == RewardedIdLife) LifeController.Instance.ChangeLives((int)reward.Amount);
                else if (adId == RewardedIdCoins) CoinController.Instance.ChangeCoins((int)reward.Amount);
                GUIManager.Instance.ContinueGameReward();
            });
        }
    }

    private void RegisterReloadHandler(RewardedAd ad, string adId)
    {
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            // Reload the ad so that we can show another as soon as possible.
            LoadRewardedAd(adId);
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            // Reload the ad so that we can show another as soon as possible.
            LoadRewardedAd(adId);
        };
    }

    //========================Intersticial===========================//
    public void ShowAndLoadInterstitialAd()
    {
        ShowInterstitialAd();
        StartCoroutine(LoadInterstitialAdRutiner());
    }

    IEnumerator LoadInterstitialAdRutiner()
    {
        yield return new WaitForSeconds(.1f);
        LoadInterstitialAd();
    }

    /// <summary>
    /// Loads the interstitial ad.
    /// </summary>
    public void LoadInterstitialAd()
    {
        // Clean up the old ad before loading a new one.
        if (interstitialAd != null)
            DestroyInterstitialAd();

        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        InterstitialAd.Load(interstitialId, adRequest,
            (InterstitialAd ad, LoadAdError error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    return;
                }

                interstitialAd = ad;
            });
    }

    /// <summary>
    /// Shows the interstitial ad.
    /// </summary>
    public void ShowInterstitialAd()
    {
        if (interstitialAd != null && interstitialAd.CanShowAd())
        {
            interstitialAd.Show();
        }
        else
        {
        }
    }

    public void DestroyInterstitialAd()
    {
        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
            interstitialAd = null;
        }
    }

    private void RegisterReloadHandler(InterstitialAd interstitialAd)
    {
        // Raised when the ad closed full screen content.
        interstitialAd.OnAdFullScreenContentClosed += () =>
        {
            // Reload the ad so that we can show another as soon as possible.
            LoadInterstitialAd();
        };
        // Raised when the ad failed to open full screen content.
        interstitialAd.OnAdFullScreenContentFailed += (AdError error) =>
        {
            // Reload the ad so that we can show another as soon as possible.
            LoadInterstitialAd();
        };
    }
}
