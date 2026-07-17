using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MG3_UIManager : MonoBehaviour
{
    [SerializeField] Text txtCoinMain;
    [SerializeField] Text txtDiamondMain;
    [SerializeField] GameObject uiWin;
    [SerializeField] Text txtCoinWin;
    [SerializeField] Text txtDiamondWin;
    [SerializeField] GameObject uiLost;
    [SerializeField] Text txtLevelCurrent;
    [SerializeField] Text txtLevelNext;
    [SerializeField] Slider slider;
    private void OnEnable()
    {
        this.RegisterListener((int)EventID.OnShowMinigameInFarm, OnShowMinigameInFarmHandle);
        this.RegisterListener((int)EventID.OnUpdateProgressG3, OnUpdateProgressG3Handle);
        this.RegisterListener((int)EventID.OnGameWin, OnGameWinHandle);
        this.RegisterListener((int)EventID.OnGameLost, OnGameLostHandle);
        this.RegisterListener((int)EventID.OnMG3Replay, OnMG3ReplayHandle);
    }

    private void OnDisable()
    {
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnShowMinigameInFarm, OnShowMinigameInFarmHandle);
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnUpdateProgressG3, OnUpdateProgressG3Handle);
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnGameWin, OnGameWinHandle);
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnGameLost, OnGameLostHandle);
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnMG3Replay, OnMG3ReplayHandle);
    }

    private void OnMG3ReplayHandle(object obj)
    {
        uiWin.SetActive(false);
        uiLost.SetActive(false);
    }

    private void OnShowMinigameInFarmHandle(object obj)
    {
        if (!(bool)obj)
        {
            return;
        }
        txtCoinMain.text = PlayerPrefSave.Coin + "";
        txtDiamondMain.text = PlayerPrefSave.Diamond + "";
        txtLevelCurrent.text = (MG3_Manager.LevelMiniGame3 +1)+ "";
        txtLevelNext.text = (MG3_Manager.LevelMiniGame3 + 2) + "";
    }

    private void OnGameLostHandle(object obj)
    {
        uiLost.SetActive(true);
    }

    private void OnGameWinHandle(object obj)
    {
        uiWin.SetActive(true);
        txtCoinWin.text = (GameManagerMiniGame.coinMiniGame+1) + "";
        txtDiamondWin.text = (GameManagerMiniGame.diamondMiniGame + 2) + "";
        MG3_Manager.LevelMiniGame3++;
    }
    private void OnUpdateProgressG3Handle(object obj)
    {
        var msg = (float)obj;
        slider.value = msg;
    }

    public void Btn_Home_Click()
    {

        Debug.Log("=> Btn_Home_Click");
        uiLost.SetActive(false);
        uiWin.SetActive(false);
        PlayerPrefSave.Coin += GameManagerMiniGame.coinMiniGame;
        PlayerPrefSave.Diamond += GameManagerMiniGame.diamondMiniGame;
        this.PostEvent((int)EventID.OnMG3Home);
        this.PostEvent((int)EventID.OnShowMinigameInFarm, true);
    }
    public void Btn_Replay_Click()
    {
        this.PostEvent((int)EventID.OnMG3Replay);
        uiLost.SetActive(false);
        uiWin.SetActive(false);
    }
}
