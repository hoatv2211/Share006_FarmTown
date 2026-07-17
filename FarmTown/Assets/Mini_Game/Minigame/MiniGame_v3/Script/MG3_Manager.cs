using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MG3_Manager : MonoBehaviour
{
    public static MG3_Manager instance;
    public static MG3_Player Player => instance.tempPlayer;
    [SerializeField] MG3_Player player;
    [SerializeField] Transform posStart;
    [SerializeField] float speedPlayer = 3;
    [SerializeField] int lvTest = 3;
    [SerializeField] MG3_LevelController[] levels;
    MG3_LevelController levelCurrent;
    MG3_Player tempPlayer;
    int tempLevel = 0;
    private void Awake()
    {
        instance = this;
    }
 
    private void OnEnable()
    {
        this.RegisterListener((int)EventID.OnShowMinigameInFarm, OnShowMinigameInFarmHandle);
        this.RegisterListener((int)EventID.OnMG3Replay, OnMG3ReplayHandle);
    }
    private void OnDisable()
    {
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnShowMinigameInFarm, OnShowMinigameInFarmHandle);
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnMG3Replay, OnMG3ReplayHandle);
    }

    private void OnMG3ReplayHandle(object obj)
    {
        if (lvTest != 0)
            tempLevel = lvTest - 1;
        LoadLevel(tempLevel);
    }

    private void OnShowMinigameInFarmHandle(object obj)
    {
        if (!(bool)obj)
        {
            return;
        }

        tempLevel = LevelMiniGame3;
        if (LevelMiniGame3 > levels.Length-1)
        {
            tempLevel = UnityEngine.Random.Range(1, levels.Length);
        }
        if (lvTest != 0)
            tempLevel = lvTest - 1;
        LoadLevel(tempLevel);
    }
    void LoadLevel(int lv)
    {
        Debug.Log("=> LoadLevel lv= " + tempLevel);
        if (levelCurrent != null)
            levelCurrent.Recycle();
        if (tempPlayer != null)
            tempPlayer.Recycle();

        levelCurrent = levels[tempLevel].Spawn(transform);
        tempPlayer = player.Spawn(transform);
        tempPlayer.Init(levelCurrent.skinPlayers, levelCurrent.listMove, speedPlayer, posStart.position);
    }
    public static int LevelMiniGame3
    {
        get { return PlayerPrefs.GetInt("LevelMiniGame3", 0); }
        set { PlayerPrefs.SetInt("LevelMiniGame3", value); }
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyUp(KeyCode.R))
        {
            this.PostEvent((int)EventID.OnMG3Replay);
        }
#endif
    }
}

