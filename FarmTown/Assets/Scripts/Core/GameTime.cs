using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameTime
{
    public static int TimeCurrent()
    {
        int secsInAMin = 60;
        int secsInAnHour = 60 * 60;
        int secsInADay = 24 * 3600;
        int secsMonth = 30 * 24 * 3600;
        int secsYear = 12 * 30 * 24 * 3600;
        System.DateTime timeNow = System.DateTime.Now;
        int sec = timeNow.Second;
        int min = timeNow.Minute * secsInAMin;
        int hour = timeNow.Hour * secsInAnHour;
        int day = timeNow.Day * secsInADay;
        int month = timeNow.Month * secsMonth;
        int year = timeNow.Year * secsYear;
        int realTime = sec + min + hour + day + month /*+ year*/;
        return realTime;
    }
}
