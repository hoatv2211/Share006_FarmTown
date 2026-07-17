using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
//电子邮件puhalskijsemen@gmail.com
//源码网站 开vpn全局模式打开 http://web3incubators.com/
//电报https://t.me/gamecode999
//网页客服 http://web3incubators.com/kefu.html
public class MagicCoinTree : MonoBehaviour
{
    private static readonly Dictionary<string, MagicCoinTree> RuntimeTrees = new Dictionary<string, MagicCoinTree>();
    [FormerlySerializedAs("gocC2")] [SerializeField] Sprite level2TrunkSprite;
    [FormerlySerializedAs("gocC3")] [SerializeField] Sprite level3TrunkSprite;
    [FormerlySerializedAs("tenVni")] [SerializeField] string vietnameseName;
    [FormerlySerializedAs("tenEng")] [SerializeField] string englishName;
    [FormerlySerializedAs("cayKC")] [SerializeField] bool producesGems;
    [FormerlySerializedAs("time")] [SerializeField] int remainingSeconds;
    [FormerlySerializedAs("timegoc")] [SerializeField] int productionDurationSeconds;
    private int rewardAmount, goldUpgradeCost, gemUpgradeCost;
    [FormerlySerializedAs("hieuung")] [SerializeField] GameObject rewardFlyoutPrefab;
    [FormerlySerializedAs("HieuUngThuHoach")] [SerializeField] GameObject harvestEffectPrefab;
    private void Awake()
    {
        RuntimeTrees[gameObject.name] = this;
    }

    private void Start()
    {
        Vector2 target = transform.position;
        transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt((13 - target.y) * 100f);
        transform.GetChild(1).GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt((13 - target.y) * 100f) + 1;
        if (!PlayerPrefs.HasKey(gameObject.name))
        {
            PlayerPrefs.SetInt(gameObject.name, 1);
            ApplyLevelStats();
            remainingSeconds = productionDurationSeconds;
            StartCoroutine(RunProductionTimer());
        }
        else
        {
            ApplyLevelStats();
            int currentTimestamp = GameTime.TimeCurrent();
            int elapsedSeconds = Mathf.Abs(currentTimestamp - PlayerPrefs.GetInt("timethoat" + gameObject.name));
            if (elapsedSeconds >= PlayerPrefs.GetInt("time" + gameObject.name))
            {
                remainingSeconds = 0;

            }
            else
            {
                remainingSeconds = PlayerPrefs.GetInt("time" + gameObject.name) - elapsedSeconds;
            }
            StartCoroutine(RunProductionTimer());
        }


    }

    private void OnMouseUpAsButton()
    {
        if (!EventSystem.current.IsPointerOverGameObject(0))
        {
            if (!PlayerPrefs.HasKey("huongdan"))
            {
                MainCamera.Instance.DisableAll();
                Interact();
            }
        }
    }
    public void Interact()
    {
        if (PlayerPrefs.GetInt("dangdichuyen") == 0)
        {
            if (remainingSeconds > 0)
            {
                PlayerPrefs.SetString("choncaydacbiet", gameObject.name);
                MagicTreePanel.instance.Open(transform.position);
                if (Application.systemLanguage == SystemLanguage.Vietnamese)
                    MagicTreePanel.instance.SetData(producesGems, vietnameseName, rewardAmount, remainingSeconds, gemUpgradeCost, goldUpgradeCost);
                else
                    MagicTreePanel.instance.SetData(producesGems, englishName, rewardAmount, remainingSeconds, gemUpgradeCost, goldUpgradeCost);
            }
            else
            {
                Harvest();
            }
        }
    }
    IEnumerator RunProductionTimer()
    {
        if (PlayerPrefs.GetString("choncaydacbiet") == gameObject.name)
        {

            MagicTreePanel.instance.remainingTimeText.text = (remainingSeconds / 60) + "m " + (remainingSeconds % 60) + "s";
        }
        PlayerPrefs.SetInt("timethoat" + gameObject.name, GameTime.TimeCurrent());
        PlayerPrefs.SetInt("time" + gameObject.name, remainingSeconds);
        yield return new WaitForSeconds(1f);
        remainingSeconds -= 1;
        if (remainingSeconds > 0)
        {
            StartCoroutine(RunProductionTimer());
        }
        else
        {
            transform.GetChild(2).gameObject.SetActive(true);
        }
    }
    public void Harvest()
    {
        GameObject rewardFlyout = Instantiate(rewardFlyoutPrefab, transform.GetChild(2).position, Quaternion.identity);
        rewardFlyout.GetComponent<RewardFlyout>().numberCoin = rewardAmount;
        transform.GetChild(2).gameObject.SetActive(false);
        GameObject harvestEffect = Instantiate(harvestEffectPrefab, transform.GetChild(2).position, Quaternion.identity);
        Destroy(harvestEffect, 2f);
        if (producesGems == true)
        {
            rewardFlyout.GetComponent<RewardFlyout>().id = 4;
            GameBootstrap.Instance.AddDiamonds(rewardAmount);
        }
        else
        {
            rewardFlyout.GetComponent<RewardFlyout>().id = 3;
            GameBootstrap.Instance.AddGold(rewardAmount);
        }
        remainingSeconds = productionDurationSeconds;
        StartCoroutine(RunProductionTimer());
        AudioManager.Instance.Harvest();
    }
    public void ApplyLevelStats()
    {
        if (PlayerPrefs.GetInt(gameObject.name) == 1)
        {
            if (producesGems == true)
            {
                rewardAmount = 3;
                productionDurationSeconds = 1200;
                goldUpgradeCost = 200;
                gemUpgradeCost = 5;
            }
            else
            {
                rewardAmount = 100;
                productionDurationSeconds = 1200;
                goldUpgradeCost = 200;
                gemUpgradeCost = 5;
            }
        }
        if (PlayerPrefs.GetInt(gameObject.name) == 2)
        {
            if (producesGems == true)
            {
                rewardAmount = 6;
                productionDurationSeconds = 1000;
                goldUpgradeCost = 500;
                gemUpgradeCost = 10;
            }
            else
            {
                rewardAmount = 200;
                productionDurationSeconds = 1000;
                goldUpgradeCost = 500;
                gemUpgradeCost = 10;
            }
            transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = level2TrunkSprite;
        }
        if (PlayerPrefs.GetInt(gameObject.name) == 3)
        {
            if (producesGems == true)
            {
                rewardAmount = 10;
                productionDurationSeconds = 600;
                goldUpgradeCost = 700;
                gemUpgradeCost = 12;
            }
            else
            {
                rewardAmount = 300;
                productionDurationSeconds = 600;
                goldUpgradeCost = 700;
                gemUpgradeCost = 12;
            }
            transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = level3TrunkSprite;
        }
        remainingSeconds = productionDurationSeconds;
    }

    public static MagicCoinTree FindRuntimeTree(string legacyName)
    {
        return RuntimeTrees.TryGetValue(legacyName, out var tree) ? tree : null;
    }

    private void OnDestroy()
    {
        if (RuntimeTrees.TryGetValue(gameObject.name, out var tree) && tree == this)
            RuntimeTrees.Remove(gameObject.name);
    }
}
