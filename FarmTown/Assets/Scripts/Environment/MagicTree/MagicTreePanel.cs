using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
public class MagicTreePanel : MonoBehaviour
{

    public static MagicTreePanel instance;
    [FormerlySerializedAs("txtname")] public Text nameText;
    [FormerlySerializedAs("txtsoKC")] public Text rewardAmountText;
    [FormerlySerializedAs("txtTime")] public Text remainingTimeText;
    [FormerlySerializedAs("txtvangnc")] public Text goldUpgradeCostText;
    [FormerlySerializedAs("txtkcnc")] public Text gemUpgradeCostText;
    [FormerlySerializedAs("iconkc")] [SerializeField] Sprite gemIcon;
    [FormerlySerializedAs("iconvang")] [SerializeField] Sprite goldIcon;
    int gemUpgradeCost, goldUpgradeCost;
    [FormerlySerializedAs("HieuUngNangCap")] [SerializeField] GameObject upgradeEffectPrefab;
    void Awake()
    {
        instance = this;
    }
    public void Open(Vector3 pos)
    {
        transform.GetChild(1).localScale = Vector3.zero;
        transform.position = pos;
        transform.GetChild(0).GetComponent<Animator>().enabled = true;
        transform.GetChild(0).GetComponent<Animator>().Play("dialogLiem", -1, 0);
    }
    public void Close()
    {
        PlayerPrefs.DeleteKey("choncaydacbiet");
        transform.GetChild(0).GetComponent<Animator>().enabled = false;
        transform.GetChild(0).localScale = Vector3.zero;
        transform.GetChild(1).localScale = Vector3.zero;
    }
    
    public void Upgrade()
    {
        MagicCoinTree selectedTree = MagicCoinTree.FindRuntimeTree(PlayerPrefs.GetString("choncaydacbiet"));
        if (PlayerPrefs.GetInt(PlayerPrefs.GetString("choncaydacbiet")) < 3)
        {
            transform.GetChild(0).GetComponent<Animator>().enabled = false;
            transform.GetChild(0).localScale = Vector3.zero;
            transform.GetChild(1).GetComponent<Animator>().Play("dialognangcapmagic", -1, 0);
            transform.GetChild(1).localScale = Vector3.one;
        }
        else
        {
            NotificationController.Instance.ShowWorldNotification("Cây đã đạt cấp tối đa", "The tree has reached the maximum level", selectedTree.transform.position);
        }
        AudioManager.Instance.click();
    }
    public void ConfirmUpgrade()
    {
        MagicCoinTree selectedTree = MagicCoinTree.FindRuntimeTree(PlayerPrefs.GetString("choncaydacbiet"));
        if (PlayerPrefs.GetInt("gold") >= goldUpgradeCost)
        {
            if (PlayerPrefs.GetInt("diamond") >= gemUpgradeCost)
            {
                GameBootstrap.Instance.AddDiamonds(-gemUpgradeCost);
                GameBootstrap.Instance.AddGold(-goldUpgradeCost);
                PlayerPrefs.SetInt(PlayerPrefs.GetString("choncaydacbiet"), PlayerPrefs.GetInt(PlayerPrefs.GetString("choncaydacbiet")) + 1);
                selectedTree.ApplyLevelStats();
                GameObject upgradeEffect = Instantiate(upgradeEffectPrefab, selectedTree.transform.position, Quaternion.identity);
                Destroy(upgradeEffect, 2f);
            }
            else
            {
                NotificationController.Instance.ShowWorldNotification("Không đủ kim cương!!!", "Not enough diamond!!!", selectedTree.transform.position);
            }
        }
        else
        {
            NotificationController.Instance.ShowWorldNotification("Không đủ vàng!!!", "Not enough gold!!!", selectedTree.transform.position);
        }
        Close();
        AudioManager.Instance.click();
    }
    public void SetData(bool producesGems, string treeName, int rewardAmount, int remainingSeconds, int requiredGems, int requiredGold)
    {
        gemUpgradeCost = requiredGems;
        goldUpgradeCost = requiredGold;
        if (producesGems == true)
            transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Image>().sprite = gemIcon;
        else
            transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Image>().sprite = goldIcon;
        nameText.text = treeName;
        rewardAmountText.text = rewardAmount.ToString();
        remainingTimeText.text = (remainingSeconds / 60) + "m " + (remainingSeconds % 60) + "s";
        goldUpgradeCostText.text = requiredGold.ToString();
        gemUpgradeCostText.text = requiredGems.ToString();
    }
}
