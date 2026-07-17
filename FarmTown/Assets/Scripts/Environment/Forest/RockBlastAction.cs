using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
//电子邮件puhalskijsemen@gmail.com
//源码网站 开vpn全局模式打开 http://web3incubators.com/
//电报https://t.me/gamecode999
//网页客服 http://web3incubators.com/kefu.html
public class RockBlastAction : MonoBehaviour
{

    [FormerlySerializedAs("hieuung")] [SerializeField] GameObject effectPrefab;
    [FormerlySerializedAs("exp")] [SerializeField] GameObject rewardPrefab;
    [FormerlySerializedAs("dungbom")] [SerializeField] GameObject toolUsePrefab;
    bool isBeingCleared;
    [FormerlySerializedAs("bom")] [SerializeField] Sprite bombIcon;
    [FormerlySerializedAs("ho")] [SerializeField] GameObject blastEffectPrefab;
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
            if (PlayerPrefs.GetInt("dangdichuyen") == 0)
            {
                if (isBeingCleared == false)
                {
                    if (!PlayerPrefs.HasKey("huongdan"))
                    {
                        MainCamera.Instance.DisableAll();
                        EnvironmentToolPanel.instance.Open(new Vector2(transform.position.x, transform.position.y - 1f), 2);
                        PlayerPrefs.SetString("choncayrung", gameObject.name);

                    }
                }
            }
        }
    }
    IEnumerator RunBlastSequence()
    {
        GameObject toolUse = Instantiate(toolUsePrefab, transform.position, Quaternion.identity);
        toolUse.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = bombIcon;
        yield return new WaitForSeconds(1f);
        Destroy(toolUse);
        GameObject effect = Instantiate(effectPrefab, transform.position, Quaternion.identity);
        GameObject blastEffect = Instantiate(blastEffectPrefab, transform.position, Quaternion.identity);
        AudioManager.Instance.PlayExplosionSound();
        Destroy(blastEffect, 5f);
        Destroy(effect, 2f);
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
                    if (PlayerPrefs.GetInt("slvatpham51") > 0)
                    {
                        PlayerPrefs.SetInt("slvatpham51", PlayerPrefs.GetInt("slvatpham51") - 1);
                        Clear();
                    }
                    else
                    {
                        gemPurchaseDialog.SetActive(true);
                        gemPurchaseDialog.transform.GetChild(1).GetChild(3).GetComponent<Button>().onClick.RemoveAllListeners();
                        gemPurchaseDialog.transform.GetChild(1).GetChild(4).GetComponent<Button>().onClick.RemoveAllListeners();
                        gemPurchaseDialog.transform.GetChild(1).GetChild(1).GetComponent<Image>().sprite = bombIcon;
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
        StartCoroutine(RunBlastSequence());
        InventoryController.instance.UpdateProductQuantity(51);
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
