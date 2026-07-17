using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Util : MonoBehaviour
{
    public static string ConvertTime(int timeConvert)
    {
        string temp = "";
        int hour = timeConvert / 3600;
        int minute = timeConvert % 3600 / 60;
        int second = timeConvert % 3600 % 60;

        if (hour >= 24)//day
        {
            temp = hour / 24 + "d ";
            hour = hour % 24;
        }
        if (hour > 0)
            temp += Convert(hour) + "h ";
        if (minute > 0)
            temp += Convert(minute) + "m ";
        if (second > 0)
            temp += Convert(second) + "s";

        return temp;
    }
    public static string ConvertTimeClock(int timeConvert)
    {
        string temp = "";
        int hour = timeConvert / 3600;
        int minute = timeConvert % 3600 / 60;
        int second = timeConvert % 3600 % 60;

        if (hour > 0)
            temp += Convert(hour) + ":";
        temp += Convert(minute) + ":";
        temp += Convert(second) + "";
        return temp;
    }
    public static string ConvertTimeMinute(int time)
    {
        string temp = "";
        int hour = time / 3600;
        int minute = time % 3600 / 60;

        if (hour >= 24)//day
        {
            temp = hour / 24 + " day ";
            hour = hour % 24;
        }
        if (hour > 0)
            temp = hour + " hour ";
        if (minute > 0)
            temp += minute + " mins ";
        return temp;
    }
    public static string ConvertTime24H(int hour)
    {
        string tempHour = "";
        if (hour < 12)
            tempHour = hour + (isVietnamese? " giờ Sáng" : " Am");
        else
        if (hour == 12)
            tempHour = hour + (isVietnamese ? " giờ Trưa" : " Pm");
        else
            if (hour > 12)
                tempHour = (hour - 12) + (isVietnamese ? " giờ Chiều" : " Pm");
        return tempHour;
    }
    public static string GetTextDesTime(int time)
    {
        string temp = "";
        int hour = time / 3600;
        int minute = time % 3600 / 60;
        int second = time % 3600 % 60;

        if (hour >= 24)//day
        {
            temp = hour / 24 + " day ";
            hour = hour % 24;
        }

        if (hour > 0)
            temp = hour + " hour ";
        else
        if (minute > 0)
            temp += minute + " mins ";
        else
            if (second > 0)
            temp += second + " second ";
        return temp + "of production";
    }
    public static int GetTimeSeconds(int time)
    {
        int temp = time;


        return temp;
    }

    static string Convert(int i)
    {
        if (i < 10)
            return "0" + i;
        else
            return i.ToString();
    }

    public static int timeNow
    {
        get
        {
            DateTime datetime = DateTime.Now;
            int month = datetime.Month;
            int day = datetime.Day;
            int hour = datetime.Hour;
            int minute = datetime.Minute;
            int second = datetime.Second;
            int TimeNow = month * 30 * 24 * 60 * 60 + (day * 24 * 60 * 60) + (hour * 60 * 60) + (minute * 60) + second;
            return TimeNow;
        }
    }
    public static int timeOut_farm
    {
        set { PlayerPrefs.SetInt("timeout", value); }
        get { return PlayerPrefs.GetInt("timeout", 0); }
    }

    public static int timeOffline
    {
        get { return timeOut_farm > 0 ? timeNow - timeOut_farm : 0; }
    }
    public static string ConvertNumber(int number)
    {
        string tempNum = number % 1000 + "";
        if (number >= 1000)
        {
            if (tempNum.Equals("0"))
                tempNum = "000";
            else
                if (tempNum.Length == 1)
                tempNum = "00" + tempNum;
            else
                if (tempNum.Length == 2)
                tempNum = "0" + tempNum;
        }


        string temp = tempNum + "";
        while (number / 1000 > 0)
        {
            temp = number / 1000 + "," + temp;
            number = number % 1000;
        }
        return temp;
    }

    private static string[] format = new string[]
    {
            "K",
            "M",
            "B",
            "T",
            "aa",
            "ab",
            "ac",
            "ad",
            "ae",
            "af",
            "ag",
            "ah",
            "ai",
            "aj",
            "ak",
            "al",
            "am",
            "an",
            "ao",
            "ap",
            "aq",
            "ar",
            "as",
            "at",
            "au",
            "av",
            "aw",
            "ax",
            "ay",
            "az"
    };

    public static string Convert(double input)
    {
        if (input < 1000.0)
        {
            return Math.Round(input).ToString();
        }
        double num = 0.0;
        for (int i = 0; i < format.Length; i++)
        {
            num = input / Math.Pow(1000.0, (double)(i + 1));
            if (num < 1000.0)
            {
                return Math.Round(num, (num >= 100.0) ? 0 : 1).ToString() + format[i];
            }
        }
        return num.ToString();
    }

    public static string ConvertTime2(int second)
    {
        int num = second / 86400;
        int num2 = second % 86400 / 3600;
        int num3 = second % 3600 / 60;
        int num4 = second % 60;
        if (num > 0)
        {
            return string.Format("{0:D2}:{1:D2}:{2:D2}:{3:D2}", num, num2, num3, num4);
        }
        if (num2 > 0)
        {
            return string.Format("{0:D2}:{1:D2}:{2:D2}", num2, num3, num4);
        }
        //if (num3 > 0)
        //{
        //    return string.Format("{0:D2}:{1:D2}", num3, num4);
        //}
        //return num4.ToString() + "s";
        return string.Format("{0:D2}:{1:D2}", num3, num4);
    }
    public static bool IsMouseOverUI
    {
        get
        {
            if (Application.platform == RuntimePlatform.WindowsEditor
            || Application.platform == RuntimePlatform.OSXEditor)
            {
                return EventSystem.current.IsPointerOverGameObject();
            }
            else
            {
                return EventSystem.current.IsPointerOverGameObject(0);
                //return EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
            }
        }
    }
    public static GameObject objClick = null;
    public static bool isPlayMinigame = false;
    static bool tempVn = false;
    public static bool isVietnamese
    {
        get { return Application.systemLanguage == SystemLanguage.Vietnamese; }
        //get { return true; }

        //get { return tempVn; }
        //set { tempVn = value; }
    }

    public static int IndexOrderHarbor = 0;
}
