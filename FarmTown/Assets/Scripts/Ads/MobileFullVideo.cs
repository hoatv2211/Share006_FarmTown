using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;

public class MobileFullVideo : MonoBehaviour
{
    //full Video Ad
    private static InterstitialAd interstitial;
    public static MobileFullVideo instance;
    string adUnitIdAndroid = "ca-app-pub-2360378184897191/5537013270";
    string adUnitIdIos = "";
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;

        DontDestroyOnLoad(gameObject);

        MobileAds.SetiOSAppPauseOnBackground(true);

        MobileAds.Initialize(initStatus => { });

        RequestInterstitial();
    }

    #region
    public void ShowFullNormal()
    {
        try
        {
            if (interstitial.CanShowAd())
            {
#if !UNITY_EDITOR
                interstitial.Show();
#endif
            }
            else
            {

                RequestInterstitial();
            }
        }
        catch
        {

        }
    }

    public void RequestInterstitial()
    {
        try
        {
#if UNITY_ANDROID
            string adUnitId = adUnitIdAndroid;
#elif UNITY_IPHONE
        string adUnitId = adUnitIdIos;
#endif
            // Clean up the old ad before loading a new one.
            if (interstitial != null)
            {
                interstitial.Destroy();
                interstitial = null;
            }

            Debug.Log("Loading the interstitial ad.");

            // create our request used to load the ad.
            var adRequest = new AdRequest();
            adRequest.Keywords.Add("unity-admob-sample");

            // send the request to load the ad.
            InterstitialAd.Load(adUnitIdAndroid, adRequest,
                (InterstitialAd ad, LoadAdError error) =>
                {
                //if error is not null, the load request failed.
                if (error != null || ad == null)
                    {
                        Debug.LogError("interstitial ad failed to load an ad " +
                                       "with error : " + error);
                        return;
                    }

                    Debug.Log("Interstitial ad loaded with response : "
                              + ad.GetResponseInfo());

                    interstitial = ad;
                });

        }
        catch
        {

        }
    }

    public static void HandleOnAdLoaded(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdLoaded event received");
    }

    public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        MonoBehaviour.print("HandleFailedToReceiveAd event received with message: "
                            + args);
    }

    public void HandleOnAdOpened(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdOpened event received");
    }

    public void HandleOnAdClosed(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdClosed event received");
        RequestInterstitial();
    }

    public void HandleOnAdLeavingApplication(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdLeavingApplication event received");
    }

#endregion
}
