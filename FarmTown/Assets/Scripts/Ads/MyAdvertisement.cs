using UnityEngine;
using System.Collections;

public class MyAdvertisement : MonoBehaviour
{
    public static void ShareFace()
    {
        AndroidJavaClass cls_UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject obj_Activity = cls_UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        obj_Activity.Call("shareFaceBook");
    }

    public static void RateGoogle()
    {
        AndroidJavaClass cls_UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject obj_Activity = cls_UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        obj_Activity.Call("rateInGooglePlay");
    }

    public static void ShowFullWhenExit()
    {
        AndroidJavaClass cls_UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject obj_Activity = cls_UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        obj_Activity.Call("showFullAdsWhenExit");
    }

    public static void ShowFullNormal()
    {
        AndroidJavaClass cls_UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject obj_Activity = cls_UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        obj_Activity.Call("showFullAdsNormal");
    }

    public static void ShowAdvertisement()
    {
        AndroidJavaClass cls_UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject obj_Activity = cls_UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        obj_Activity.Call("showQuangCao");
    }

    public static string GetNameEmail()
    {
        AndroidJavaClass cls_UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject obj_Activity = cls_UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        string pa = obj_Activity.Call<string>("getNameEmail");
        return pa;
    }

    public static bool CheckConnection()
    {
        AndroidJavaClass cls_UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject obj_Activity = cls_UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        bool pa = obj_Activity.Call<bool>("IsConnection");
        return pa;
    }
    public static bool isAd()
    {
        AndroidJavaClass cls_UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject obj_Activity = cls_UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        bool pa = obj_Activity.Call<bool>("isAd");
        return pa;
    }

    public static void HideBanner()
    {
        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
        jo.Call("Hidde");
    }
    public static void ShowBanner()
    {
        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
        jo.Call("Show");
    }
    ////======Quang cao thoat game hoan toan
    public static void ExitGame()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");    
            AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
            jo.Call("showDialogGame");

        }
    }

	public static void ShowRewardedVideo()
	{
		AndroidJavaClass cls_UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject obj_Activity = cls_UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
		obj_Activity.Call("showRewardedVideo");
	}

}
