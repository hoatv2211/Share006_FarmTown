using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;

public class MobileRewardAd : MonoBehaviour
{
    public static MobileRewardAd instance = null;
    string IdRewardedVideoAndroid = "ca-app-pub-2360378184897191/4504623898";
    public RewardedAd rewardBasedVideoAd;
    // Use this for initialization
    private void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
       
        RequestRewardedVideo();
    }
    public void RequestRewardedVideo()
    {
#if UNITY_ANDROID
        string adUnitId = IdRewardedVideoAndroid;
#elif UNITY_IPHONE
        string adUnitId = IdRewardedVideoIOs;
#else
        string adUnitId = "unexpected_platform";
#endif
        //rewardBasedVideoAd = new RewardedAd(adUnitId);
        //rewardBasedVideoAd.OnAdLoaded += OnAdLoaded;
        //rewardBasedVideoAd.OnAdFailedToLoad += OnAdFailedToLoad;
        //rewardBasedVideoAd.OnAdLoaded += OnAdLoaded;
        //rewardBasedVideoAd.OnAdClosed += OnAdClose;
        //rewardBasedVideoAd.OnUserEarnedReward += OnAdRewarded;
        //AdRequest request = new AdRequest.Builder().Build();
        //rewardBasedVideoAd.LoadAd(request);
        // Clean up the old ad before loading a new one.
        if (rewardBasedVideoAd != null)
        {
            rewardBasedVideoAd.Destroy();
            rewardBasedVideoAd = null;
        }

        Debug.Log("Loading the rewarded ad.");

        // create our request used to load the ad.
        AdRequest adRequest = new AdRequest();
        //adRequest.Keywords.Add("unity-admob-sample");

        // send the request to load the ad.
        RewardedAd.Load(IdRewardedVideoAndroid, adRequest,
            (RewardedAd ad, LoadAdError error) =>
            {
                // if error is not null, the load request failed.
                //if (error != null || ad == null)
                //  {
                //      Debug.LogError("Rewarded ad failed to load an ad " +
                //                     "with error : " + error);
                //      return;
                //  }

                //  Debug.Log("Rewarded ad loaded with response : "
                //            + ad.GetResponseInfo());

                rewardBasedVideoAd = ad;
            });
    }

    private void OnAdLoaded(object sender, EventArgs args)
    {

    }

    private void OnAdFailedToLoad(object sender, EventArgs args)
    {

    }

    public void ShowRewardedAd()
    {
        const string rewardMsg =
       "Rewarded ad rewarded the user. Type: {0}, amount: {1}.";

        if (rewardBasedVideoAd != null && rewardBasedVideoAd.CanShowAd())
        {
            rewardBasedVideoAd.Show((Reward reward) =>
            {
                DialogRewardGift.instance.ShowDialog();
                // TODO: Reward the user.
                Debug.Log(string.Format(rewardMsg, reward.Type, reward.Amount));
            });
        }

    }


    private void OnAdClose(object sender, EventArgs args)
    {
        //rewardBasedVideoAd.OnAdLoaded -= OnAdLoaded;
        //rewardBasedVideoAd.OnAdFailedToLoad -= OnAdFailedToLoad;
        //rewardBasedVideoAd.OnAdLoaded -= OnAdLoaded;
        //rewardBasedVideoAd.OnAdClosed -= OnAdClose;
        //rewardBasedVideoAd.OnUserEarnedReward += OnAdRewarded;
        RequestRewardedVideo();
    }

    private void OnAdRewarded(object sender, EventArgs args)
    {       
        DialogRewardGift.instance.ShowDialog();
       
    }
}
