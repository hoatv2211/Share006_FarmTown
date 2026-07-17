using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
public class TreeChopAction : MonoBehaviour
{
//电子邮件puhalskijsemen@gmail.com
//源码网站 开vpn全局模式打开 http://web3incubators.com/
//电报https://t.me/gamecode999
//网页客服 http://web3incubators.com/kefu.html
    [FormerlySerializedAs("hieuung")] [SerializeField] GameObject effectPrefab;
    [FormerlySerializedAs("exp")] [SerializeField] GameObject rewardPrefab;
    [FormerlySerializedAs("dungcua")] [SerializeField] GameObject toolUsePrefab;
    bool isBeingCleared;
    [FormerlySerializedAs("riu")] [SerializeField] Sprite axeIcon;
    [FormerlySerializedAs("DialogDungKc")] [SerializeField] GameObject gemPurchaseDialog;
    private void Awake()
    {
        if (PlayerPrefs.HasKey("dahuy" + gameObject.name))
        {
            Destroy(gameObject);
        }
        Vector3 temp = transform.TransformDirection(transform.position);
        gameObject.GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt((13 - temp.y) * 100f);
    }

    private void OnMouseUpAsButton()
    {
        if (!EventSystem.current.IsPointerOverGameObject(0))
        {
            if (isBeingCleared == false)
            {
                if (!PlayerPrefs.HasKey("huongdan"))
                {
                    if (PlayerPrefs.GetInt("dangdichuyen") == 0)
                        MainCamera.Instance.DisableAll();
                    EnvironmentToolPanel.instance.Open(transform.position, 1);
                    PlayerPrefs.SetString("choncayrung", gameObject.name);
                }
            }
        }
    }

    IEnumerator RunChopSequence()
    {
        GameObject toolUse = Instantiate(toolUsePrefab, transform.position, Quaternion.identity);
        toolUse.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = axeIcon;
        yield return new WaitForSeconds(1f);
        Destroy(toolUse);
        GameObject effect = Instantiate(effectPrefab, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(1f);
        AudioManager.Instance.PlayChopSound();
        yield return new WaitForSeconds(1f);
        AudioManager.Instance.PlayChopSound();
        yield return new WaitForSeconds(1f);
        AudioManager.Instance.PlayChopSound();
        yield return new WaitForSeconds(1f);
        Destroy(effect);
        GameObject reward = Instantiate(rewardPrefab, transform.position, Quaternion.identity);
        reward.GetComponent<RewardFlyout>().id = 2;
        reward.GetComponent<RewardFlyout>().numberCoin = 3;
        Destroy(gameObject);
    }
    private void OnTriggerEnter2D(Collider2D target)
    {
        if (target.tag == "cua")
        {
            if (PlayerPrefs.GetString("choncayrung") == gameObject.name)
            {
                if (isBeingCleared == false)
                {
                    if (PlayerPrefs.GetInt("slvatpham50") > 0)
                    {
                        PlayerPrefs.SetInt("slvatpham50", PlayerPrefs.GetInt("slvatpham50") - 1);
                        Clear();
                    }
                    else
                    {
                        gemPurchaseDialog.SetActive(true);
                        gemPurchaseDialog.transform.GetChild(1).GetChild(3).GetComponent<Button>().onClick.RemoveAllListeners();
                        gemPurchaseDialog.transform.GetChild(1).GetChild(1).GetComponent<Image>().sprite = axeIcon;
                        gemPurchaseDialog.transform.GetChild(1).GetChild(1).GetComponent<Image>().SetNativeSize();
                        gemPurchaseDialog.transform.GetChild(1).GetChild(3).GetComponent<Button>().onClick.AddListener(ClearWithGems);
                        gemPurchaseDialog.transform.GetChild(1).GetChild(4).GetComponent<Button>().onClick.AddListener(CloseDialog);
                    }

                }
            }
        }

    }
    public void Clear()
    {
        PlayerPrefs.SetInt("dahuy" + gameObject.name, 1);
        isBeingCleared = true;
        StartCoroutine(RunChopSequence());
        InventoryController.instance.UpdateProductQuantity(50);
    }
    public void ClearWithGems()
    {
        if (PlayerPrefs.GetInt("diamond") >= 5)
        {
            gemPurchaseDialog.SetActive(false);
            GameBootstrap.Instance.AddDiamonds(-5);
            Clear();

        }
        else
        {
            NotificationController.Instance.ShowCanvasNotification("Bạn không đủ kim cương!", "You 're not enough diamonds!");
        }
        AudioManager.Instance.click();
    }
    public void CloseDialog()
    {
        gemPurchaseDialog.SetActive(false);
        AudioManager.Instance.click();
    }
}
