using UnityEngine;

public class PlayerPrefSave
{
    public static bool IsFirtOpen
    {
        get { return PlayerPrefs.GetInt("IsFirtOpen") == 0; }
        set { PlayerPrefs.SetInt("IsFirtOpen", value == false ? 1 : 0); }
    }

    public static int Coin
    {
        get { return PlayerPrefs.GetInt("gold",1000); }
        set { PlayerPrefs.SetInt("gold", value); }
    }
    public static int Diamond
    {
        get { return PlayerPrefs.GetInt("diamond", 20); }
        set { PlayerPrefs.SetInt("diamond", value); }
    }
    public static int Level
    {
        get { return PlayerPrefs.GetInt("LevelGame", 1); }
        set { PlayerPrefs.SetInt("LevelGame", value); }
    }

    public static int LevelMiniGame
    {
        get { return PlayerPrefs.GetInt("LevelMiniGame" + version_minigame, 0); }
        set { PlayerPrefs.SetInt("LevelMiniGame" + version_minigame, value); }
    }
    public static string version_minigame
    {
        get { return PlayerPrefs.GetString("version_minigame", "v1"); }
        set { PlayerPrefs.SetString("version_minigame", value); }
    }
    public static bool IsEnableMiniGame(string version)
    {
        return PlayerPrefs.GetInt("IsEnableMiniGame" + version) == 0;
    }
    public static void SetEnableMiniGame(string version, bool isEnable)
    {
        PlayerPrefs.SetInt("IsEnableMiniGame" + version, isEnable ? 0 : 1);
    }

}
