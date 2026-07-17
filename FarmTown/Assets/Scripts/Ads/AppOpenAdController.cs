using GoogleMobileAds.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppOpenAdController : MonoBehaviour
{
#if UNITY_ANDROID
    //id test
    //private const string AD_UNIT_ID = "ca-app-pub-3940256099942544/3419835294";
    private const string AD_UNIT_ID = "ca-app-pub-2360378184897191/7578294815";
#elif UNITY_IOS
    private const string AD_UNIT_ID = "ca-app-pub-3321735106491906/1464416834";
#else
    private const string AD_UNIT_ID = "unexpected_platform";
#endif

    private AppOpenAd ad;
    private bool isShowingAd;

    public static AppOpenAdController Instance { get; private set; }

    private bool IsAdAvailable
    {
        get
        {
            return ad != null && ad.CanShowAd();
        }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        LoadAd();

    }
    public void LoadAd()
    {
        AdRequest request = new AdRequest.Builder().Build();

        // Load an app open ad for portrait orientation
        AppOpenAd.Load(AD_UNIT_ID, ScreenOrientation.Portrait, request, ((appOpenAd, error) =>
        {
            if (error != null)
            {
                // Handle the error.
                //Debug.LogFormat("Failed to load the ad. (reason: {0})", error.LoadAdError.GetMessage());
                return;
            }

            DestroyAd();
            ad = appOpenAd;
            RegisterCallbacks(ad);
        }));
    }
    public void ShowAdIfAvailable()
    {
        Debug.Log("IsAdAvailable: " + IsAdAvailable);
        Debug.Log("isShowingAd: " + isShowingAd);
        if (!IsAdAvailable || isShowingAd)
        {
            return;
        }
        isShowingAd = true;
        ad.Show();
        Debug.Log("show app openn");
    }

    private void RegisterCallbacks(AppOpenAd loadedAd)
    {
        loadedAd.OnAdFullScreenContentOpened += HandleAdFullScreenContentOpened;
        loadedAd.OnAdFullScreenContentClosed += HandleAdFullScreenContentClosed;
        loadedAd.OnAdFullScreenContentFailed += HandleAdFullScreenContentFailed;
        loadedAd.OnAdImpressionRecorded += HandleAdImpressionRecorded;
        loadedAd.OnAdPaid += HandleAdPaid;
    }

    private void HandleAdFullScreenContentClosed()
    {
        Debug.Log("Closed app open ad");
        isShowingAd = false;
        DestroyAd();
        LoadAd();
    }

    private void HandleAdFullScreenContentFailed(AdError error)
    {
        Debug.LogFormat("Failed to present app open ad: {0}", error.GetMessage());
        isShowingAd = false;
        DestroyAd();
        LoadAd();
    }

    private void HandleAdFullScreenContentOpened()
    {
        Debug.Log("Displayed app open ad");
    }

    private void HandleAdImpressionRecorded()
    {
        Debug.Log("Recorded ad impression");
    }

    private void HandleAdPaid(AdValue value)
    {
        Debug.LogFormat("Received paid event. (currency: {0}, value: {1}",
                value.CurrencyCode, value.Value);
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
        DestroyAd();
    }

    private void DestroyAd()
    {
        if (ad == null)
            return;

        ad.Destroy();
        ad = null;
    }
   
}
