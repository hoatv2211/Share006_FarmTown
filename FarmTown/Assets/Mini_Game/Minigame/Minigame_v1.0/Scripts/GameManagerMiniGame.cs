using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using DG.Tweening;

public class GameManagerMiniGame : MonoBehaviour
{
    public static GameManagerMiniGame instance;
    public static bool IsWin => instance.isWin;
    public static bool IsLost => instance.isLost;
    public static Camera Camera=>instance.camera;
    public static int coinMiniGame => instance.coin;
    public static int diamondMiniGame => instance.diamond;
    [SerializeField] GameVersion gameVersion;
    [SerializeField] int coin = 200;
    [SerializeField] int diamond = 5;
    [Header("======================")]
    [SerializeField] int LevelTest = 0;
    [Header("======================")]
    [SerializeField] Camera camera;
    [SerializeField] Canvas canvas;
    [SerializeField] Text txtCoin;
    [SerializeField] Text txtDiamond;
    [SerializeField] private Transform parentLevel;
    [SerializeField] private GameObject[] effectWin;
    [SerializeField] Animator animBtnNext;
    [Header("Minigame v1: Tower")]
    [SerializeField] private GameObject[] objGame1;
    [SerializeField] private GameObject[] levels1;
    [SerializeField] private GameObject win1;
    [SerializeField] private GameObject lost1;
    [SerializeField] Text txtCoinWin1;
    [SerializeField] Text txtDiamondWin1;
    [SerializeField] Text txtHome;
    [SerializeField] AudioSource music1;
    [SerializeField] ParticleSystem fxFloorHuse;
    [SerializeField] Transform[] tfPosFx;

    [Header("Minigame v2: Rescue")]
    [SerializeField] private GameObject[] levels2;
    [SerializeField] private GameObject win2;
    [SerializeField] private GameObject lost2;
    [SerializeField] Text txtCoinWin2;
    [SerializeField] Text txtDiamondWin2;
    [SerializeField] AudioSource music2;
    int keyCount;
    [Header("Minigame v3: Farm Rescue")]
    [SerializeField] private GameObject[] levels3;
    [SerializeField] GameObject objLevelG3;
    [SerializeField] Text txtLevelCurrent;
    [SerializeField] Text txtLevelNext;
    [SerializeField] Slider slider;
    [SerializeField] AudioSource music3;
    [SerializeField] MG3_Player player;
    [SerializeField] float speedPlayer = 5;
    MG3_LevelController levelCurrent;
    MG3_Player tempPlayer;
    public static MG3_Player Player => instance.tempPlayer;
    [Header("===================")]
    public GameObject objTutorial;
    public int totalEnemy;
    bool isWin, isLost;
    GameObject[] levels;
    GameObject lost;
    GameObject win;
    Text txtCoinWin;
    Text txtDiamondWin;
    AudioSource music;
    int tempLevel = 0;
    GameObject objLevel;

    private void Awake()
    {
        instance = this;
        parentLevel.RecycleChild();
    }
    private void OnEnable()
    {
        this.RegisterListener((int)EventID.OnFxFloorHouse, OnFxFloorHouseHandle);
        this.RegisterListener((int)EventID.OnUpdateProgressG3, OnUpdateProgressG3Handle);
        this.RegisterListener((int)EventID.OnGameWin, OnGameWinHandle);
        this.RegisterListener((int)EventID.OnGameLost, OnGameLostHandle);
        this.RegisterListener((int)EventID.OnMG3Replay, OnMG3ReplayHandle);
    }

    private void OnDisable()
    {
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnGameLost, OnGameLostHandle);
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnGameWin, OnGameWinHandle);
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnFxFloorHouse, OnFxFloorHouseHandle);
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnUpdateProgressG3, OnUpdateProgressG3Handle);
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnMG3Replay, OnMG3ReplayHandle);
    }

    private void OnMG3ReplayHandle(object obj)
    {
        win.SetActive(false);
        lost.SetActive(false);
        InstanceObjLevel();
    }

    private void OnGameWinHandle(object obj)
    {
        Win();
    }
    private void OnUpdateProgressG3Handle(object obj)
    {
        var msg = (float)obj;
        slider.value = msg;
    }

    private void OnFxFloorHouseHandle(object obj)
    {
        var msg = (int)obj;
        fxFloorHuse.transform.localPosition = tfPosFx[msg - 1].transform.position;
        fxFloorHuse.Play();
    }

    private void OnGameLostHandle(object obj)
    {
        isLost = true;
        //MobileFullVideo.instance.ShowFullNormal(() =>
        //{
        //}, () =>
        //{
        //});

        lost.SetActive(true);
        DOTween.KillAll();
    }
    private void Start()
    {
        lost1.SetActive(false);
        win1.SetActive(false);
        lost2.SetActive(false);
        win2.SetActive(false);
        objLevelG3.SetActive(false);
        AcitveObjGame1(false);
        CameraFollow.instance.RestPosition();
        switch (gameVersion)
        {
            case GameVersion.V1_Tower:
                PlayerPrefSave.version_minigame = "v1";
                break;
            case GameVersion.V2_Recue:
                PlayerPrefSave.version_minigame = "v2";
                break;
            case GameVersion.V3_FarmRecue:
                PlayerPrefSave.version_minigame = "v3";
                break;
        }
        Debug.Log("=> Start minigame " + PlayerPrefSave.version_minigame);
        switch (PlayerPrefSave.version_minigame)
        {
            case "v1":
                levels = levels1;
                lost = lost1;
                win = win1;
                txtCoinWin = txtCoinWin1;
                txtDiamondWin = txtDiamondWin1;
                music = music1;
                AcitveObjGame1(true);
                break;
            case "v2":
                levels = levels2;
                lost = lost2;
                win = win2;
                txtCoinWin = txtCoinWin2;
                txtDiamondWin = txtDiamondWin2;
                music = music2;
                break;
            case "v3":
                levels = levels3;
                lost = lost1;
                win = win1;
                txtCoinWin = txtCoinWin1;
                txtDiamondWin = txtDiamondWin1;
                music = music3;
                objLevelG3.SetActive(true);
                txtLevelCurrent.text = (PlayerPrefSave.LevelMiniGame + 1) + "";
                txtLevelNext.text = (PlayerPrefSave.LevelMiniGame + 2) + "";
                slider.value = 0;
                break;
        }
        tempLevel = PlayerPrefSave.LevelMiniGame;
        lost.SetActive(false);
        win.SetActive(false);

        if (tempLevel >= levels.Length - 1)
        {
            tempLevel = UnityEngine.Random.Range(0, levels.Length);
        }
        InstanceObjLevel();
    }
    void InstanceObjLevel()
    {
        if (objLevel != null)
            objLevel.Recycle();

        objLevel = levels[tempLevel].Spawn(parentLevel);
        objLevel.transform.localPosition = Vector3.zero;
        if (PlayerPrefSave.LevelMiniGame == 0)
        {
            objTutorial = objLevel.transform.GetChild(objLevel.transform.childCount - 1).gameObject;
        }

        if (PlayerPrefSave.version_minigame == "v1")
            StartCoroutine(DelayFloor());

        if(LevelTest != 0)
        {
            tempLevel = LevelTest - 1;
        }

        if (PlayerPrefSave.version_minigame == "v3")
        {
            ShowObjTutorial(false);

            if (tempPlayer != null)
                tempPlayer.Recycle();

            tempPlayer = player.Spawn(parentLevel);
            levelCurrent = objLevel.GetComponent<MG3_LevelController>();
            tempPlayer.Init(levelCurrent.skinPlayers, levelCurrent.listMove, speedPlayer, levelCurrent.posStart.position);

            txtLevelCurrent.text = (PlayerPrefSave.LevelMiniGame + 1) + "";
            txtLevelNext.text = (PlayerPrefSave.LevelMiniGame + 2) + "";
        }

        txtCoin.text = PlayerPrefSave.Coin.ToString();
        txtDiamond.text = PlayerPrefSave.Diamond.ToString();

    }
    IEnumerator DelayFloor()
    {
        yield return new WaitForSeconds(0.1f);
        isWin = false;
        isLost = false;
        GameObject[] floors = GameObject.FindGameObjectsWithTag("Floor");
        totalEnemy = 0;
        for (int i = 0; i < floors.Length; i++)
        {
            totalEnemy += floors[i].GetComponent<Floor>().enemies.Count;
        }
    }
    void AcitveObjGame1(bool isActive)
    {
        for (int i = 0; i < objGame1.Length; i++)
        {
            objGame1[i].SetActive(isActive);
        }
    }
    public void Win()
    {
        isWin = true;
        txtCoinWin.text =coin+"";
        txtDiamondWin.text = diamond+"";
        PlayerPrefSave.LevelMiniGame++;

        GameObject chuong = GameObject.FindGameObjectWithTag("chuong");
        if (chuong != null)
        {
            chuong.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            chuong.GetComponentInParent<Animator>().enabled = false;
        }

        for (int i = 0; i < effectWin.Length; i++)
        {
            effectWin[i].transform.position = new Vector3(camera.transform.position.x,
                effectWin[i].transform.position.y, 0);
            effectWin[i].SetActive(true);
        }

        StartCoroutine(DelayShowWin());
        MobileFullVideo.instance.ShowFullNormal();
    }

    IEnumerator DelayShowWin()
    {
        yield return new WaitForSeconds(2);
        for (int i = 0; i < effectWin.Length; i++)
        {
            effectWin[i].SetActive(false);
        }
        var currentCoin = PlayerPrefSave.Coin;
        PlayerPrefSave.Coin += 200;
        var currentDiamond = PlayerPrefSave.Diamond;
        PlayerPrefSave.Diamond += 5;

        txtCoin.text = PlayerPrefSave.Coin+"";
        txtDiamond.text = PlayerPrefSave.Diamond+"";
        win.SetActive(true);

    }


    public void Replay()
    {
        //InstanceObjLevel();
        this.PostEvent((int)EventID.OnMG3Replay);
        if (PlayerPrefSave.version_minigame == "v1")
            StartCoroutine(DelayFloor());
        lost.SetActive(false);
    }

    public void Next()
    {
        Util.isPlayMinigame = true;
        animBtnNext.SetTrigger("click");
        tempLevel = PlayerPrefSave.LevelMiniGame;
        lost.SetActive(false);
        win.SetActive(false);

        if (tempLevel >= levels.Length - 1)
        {
            tempLevel = UnityEngine.Random.Range(0, levels.Length);
        }
        SceneLoader.Instance?.Load(1);
    }
    void NextVersionMinigame()
    {
        if (PlayerPrefSave.version_minigame == "v1")
        {
            if (PlayerPrefSave.IsEnableMiniGame("v2"))
                PlayerPrefSave.version_minigame = "v2";
            else
                if (PlayerPrefSave.IsEnableMiniGame("v3"))
                    PlayerPrefSave.version_minigame = "v3";
        }
        else
        {
            if (PlayerPrefSave.version_minigame == "v2")
            {
                if (PlayerPrefSave.IsEnableMiniGame("v3"))
                    PlayerPrefSave.version_minigame = "v3";
                else
                    if (PlayerPrefSave.IsEnableMiniGame("v1"))
                    PlayerPrefSave.version_minigame = "v1";
            }
            else
            {
                if (PlayerPrefSave.version_minigame == "v3")
                {
                    if (PlayerPrefSave.IsEnableMiniGame("v1"))
                        PlayerPrefSave.version_minigame = "v1";
                    else
                        if (PlayerPrefSave.IsEnableMiniGame("v2"))
                        PlayerPrefSave.version_minigame = "v2";
                }
            }
        }

    }
    public void PressKey()
    {
        keyCount++;
        if (keyCount >= MNG2_Player.instance.keyToMove)
        {
            MNG2_Player.instance.GoGo();
        }
    }
    public void ShowObjTutorial(bool isDisable)
    {
        if (objTutorial != null)
        {
            objTutorial.SetActive(isDisable);
        }
    }
    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyUp(KeyCode.R))
        {
            this.PostEvent((int)EventID.OnMG3Replay);
        }
        if (Input.GetKeyUp(KeyCode.N))
        {
            NextVersionMinigame();
            this.PostEvent((int)EventID.OnMG3Replay);
        }
#endif
    }
}
public enum GameVersion
{
    V1_Tower, V2_Recue, V3_FarmRecue
}