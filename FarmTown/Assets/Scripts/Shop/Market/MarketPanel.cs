using FarmTown.Save;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;
public class MarketPanel : MonoBehaviour
{

    public static MarketPanel instance;
    [FormerlySerializedAs("listVp"), SerializeField] GameObject itemList;
    [FormerlySerializedAs("txttime"), SerializeField] Text refreshTimeText;
    int itemSlotCount;
    [FormerlySerializedAs("time"), SerializeField] int remainingRefreshSeconds;
    private void Start()
    {

        instance = this;
        if (!PlayerPrefs.HasKey("loaivp0"))
        {
            remainingRefreshSeconds = 7200;
            GenerateNewItems();
        }
        else
        {
            int currentTimestamp = GameTime.TimeCurrent();
            int offlineElapsedSeconds = Mathf.Abs(currentTimestamp - PlayerPrefs.GetInt("timechothoat"));
            if (offlineElapsedSeconds >= PlayerPrefs.GetInt("timecho"))
            {
                GenerateNewItems();
            }
            else
            {
                itemSlotCount = PlayerPrefs.GetInt("slgiohang");
                remainingRefreshSeconds = PlayerPrefs.GetInt("timecho") - offlineElapsedSeconds;
                RestoreItems();
}
        }

        StartCoroutine(RefreshCountdown());
    }
    public void OpenDialog()
    {
        //MainCamera.Instance.lockCam();
        transform.localScale = Vector3.one;
        gameObject.GetComponent<Animator>().enabled = true;
        gameObject.GetComponent<Animator>().Play("MarketPanelOpen", -1, 0);
    }
    public void CloseDialog()
    {
        MainCamera.Instance.unLockCam();
        gameObject.GetComponent<Animator>().enabled = false;
        transform.localScale = Vector3.zero;
    }
    public void Refresh()
    {
        if (PlayerPrefs.GetInt("diamond") >= 5)
        {
            StopAllCoroutines();
            GameBootstrap.Instance.AddDiamonds(-5);
            remainingRefreshSeconds = 7200;
            GenerateNewItems();
            StartCoroutine(RefreshCountdown());
        }
        else
        {
            NotificationController.Instance.ShowCanvasNotification("Không đủ kim cương!", "Not enough diamonds");
        }
    }
    public void GenerateNewItems()
    {
        int unlockedCropCount = 0;
        int unlockedProductCount = 0;
        ArrayList unlockedCropIds = new ArrayList();
        ArrayList unlockedProductIds = new ArrayList();
        for (int i = 0; i < GameBootstrap.Instance.cropDatabase.crops.Length; i++)
        {
            if (GameBootstrap.Instance.cropDatabase.crops[i].unlockLevel <= PlayerPrefs.GetInt("level"))
            {
                unlockedCropCount += 1;
                unlockedCropIds.Add(i);
            }
        }
        for (int i = 0; i < InventoryController.instance.products.Length; i++)
        {
            if (InventoryController.instance.products[i].unlockLevel <= PlayerPrefs.GetInt("level"))
            {
                unlockedProductCount += 1;
                unlockedProductIds.Add(i);
            }
        }
        UpdateSlotCountForLevel();
        PlayerPrefs.SetInt("slgiohang", itemSlotCount);
        itemList.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        itemList.transform.GetChild(0).GetChild(2).gameObject.SetActive(true);
        PlayerPrefs.SetInt("loaivp0", 1);
        PlayerPrefs.SetInt("muavp0", Random.Range(49, 52));
        PlayerPrefs.SetInt("slvp0", Random.Range(1, 3));
        itemList.transform.GetChild(0).GetComponent<Button>().onClick.RemoveAllListeners();
        itemList.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = InventoryController.instance.products[PlayerPrefs.GetInt("muavp0")].icon;
        itemList.transform.GetChild(0).GetChild(2).GetComponent<Text>().text = PlayerPrefs.GetInt("slvp0").ToString();
        itemList.transform.GetChild(0).GetChild(4).GetComponent<Text>().text = (InventoryController.instance.products[PlayerPrefs.GetInt("muavp0")].purchasePrice * PlayerPrefs.GetInt("slvp0")).ToString();
        itemList.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() => BuyItem(PlayerPrefs.GetInt("muavp0"), false, 0));
        for (int i = 1; i < itemSlotCount; i++)
        {
            itemList.transform.GetChild(i).GetComponent<Button>().onClick.RemoveAllListeners();
            itemList.transform.GetChild(i).GetChild(0).gameObject.SetActive(true);
            itemList.transform.GetChild(i).GetChild(2).gameObject.SetActive(true);
            PlayerPrefs.SetInt("loaivp" + i, Random.Range(0, 2));
            if (PlayerPrefs.GetInt("loaivp" + i) == 0)
            {
                // kho nong san
                int cropId =(int)unlockedCropIds[Random.Range(0, unlockedCropCount)];
                int cropQuantity = Random.Range(2, 5);
                PlayerPrefs.SetInt("muavp" + i, cropId);
                PlayerPrefs.SetInt("slvp" + i, cropQuantity);
                itemList.transform.GetChild(i).GetChild(0).GetComponent<Image>().sprite = GameBootstrap.Instance.cropDatabase.crops[cropId].productIcon;
                itemList.transform.GetChild(i).GetChild(2).GetComponent<Text>().text = cropQuantity.ToString();
                itemList.transform.GetChild(i).GetChild(4).GetComponent<Text>().text = (GameBootstrap.Instance.cropDatabase.crops[cropId].salePrice * cropQuantity).ToString();
                int j = i;
                itemList.transform.GetChild(i).GetComponent<Button>().onClick.AddListener(() => BuyItem(cropId, true, j));
            }
            else
            {
                //kho vat pham
                int productId = (int)unlockedProductIds[Random.Range(0, unlockedProductCount)];
                int productQuantity = Random.Range(1, 3);
                PlayerPrefs.SetInt("muavp" + i, productId);
                PlayerPrefs.SetInt("slvp" + i, productQuantity);
                itemList.transform.GetChild(i).GetChild(0).GetComponent<Image>().sprite = InventoryController.instance.products[productId].icon;
                itemList.transform.GetChild(i).GetChild(2).GetComponent<Text>().text = productQuantity.ToString();
                itemList.transform.GetChild(i).GetChild(4).GetComponent<Text>().text = (InventoryController.instance.products[productId].purchasePrice * productQuantity).ToString();
                int j = i;
                itemList.transform.GetChild(i).GetComponent<Button>().onClick.AddListener(() => BuyItem(productId, false, j));
            }
        }
        unlockedCropIds.Clear();
        unlockedProductIds.Clear();
    }
    public void UpdateSlotCountForLevel()
    {
        if (PlayerPrefs.GetInt("level") < 5)
            itemSlotCount = 3;
        else if (PlayerPrefs.GetInt("level") < 7 && PlayerPrefs.GetInt("level") >= 5)
            itemSlotCount = 4;
        else if (PlayerPrefs.GetInt("level") < 9 && PlayerPrefs.GetInt("level") >= 7)
            itemSlotCount = 5;
        else if (PlayerPrefs.GetInt("level") < 11 && PlayerPrefs.GetInt("level") >= 9)
            itemSlotCount = 6;
        else if (PlayerPrefs.GetInt("level") < 13 && PlayerPrefs.GetInt("level") >= 11)
            itemSlotCount = 7;
        else if (PlayerPrefs.GetInt("level") < 15 && PlayerPrefs.GetInt("level") >= 13)
            itemSlotCount = 8;
        else if (PlayerPrefs.GetInt("level") < 17 && PlayerPrefs.GetInt("level") >= 15)
            itemSlotCount = 9;
        else if (PlayerPrefs.GetInt("level") >= 17)
            itemSlotCount = 10;
    }
    public void RestoreItems()
    {
        
        for (int i = 0; i < itemSlotCount; i++)
        {
            itemList.transform.GetChild(i).GetChild(0).gameObject.SetActive(true);
            itemList.transform.GetChild(i).GetChild(2).gameObject.SetActive(true);
            itemList.transform.GetChild(i).GetComponent<Button>().onClick.RemoveAllListeners();
            if (PlayerPrefs.GetInt("loaivp" + i) == 0)
            {
                //nong san
                int cropId = PlayerPrefs.GetInt("muavp" + i);
                int cropQuantity = PlayerPrefs.GetInt("slvp" + i);

                itemList.transform.GetChild(i).GetChild(0).GetComponent<Image>().sprite = GameBootstrap.Instance.cropDatabase.crops[cropId].productIcon;
                itemList.transform.GetChild(i).GetChild(2).GetComponent<Text>().text = cropQuantity.ToString();
                itemList.transform.GetChild(i).GetChild(4).GetComponent<Text>().text = (cropQuantity * GameBootstrap.Instance.cropDatabase.crops[cropId].salePrice).ToString();
                int j = i;             
                itemList.transform.GetChild(i).GetComponent<Button>().onClick.AddListener(() => BuyItem(cropId, true, j));
            }
            else
            {
                //vat pham
                int productId = PlayerPrefs.GetInt("muavp" + i);
                int productQuantity = PlayerPrefs.GetInt("slvp" + i);
                itemList.transform.GetChild(i).GetChild(0).GetComponent<Image>().sprite = InventoryController.instance.products[productId].icon;
                itemList.transform.GetChild(i).GetChild(2).GetComponent<Text>().text = productQuantity.ToString();
                itemList.transform.GetChild(i).GetChild(4).GetComponent<Text>().text = (productQuantity * InventoryController.instance.products[productId].purchasePrice).ToString();
                int j = i;
                itemList.transform.GetChild(i).GetComponent<Button>().onClick.AddListener(() => BuyItem(productId, false, j));
            }
        }
    }
    IEnumerator RefreshCountdown()
    {
        remainingRefreshSeconds -= 1;
        PlayerPrefs.SetInt("timecho", remainingRefreshSeconds);
        PlayerPrefs.SetInt("timechothoat", GameTime.TimeCurrent());
        yield return new WaitForSeconds(1f);
        refreshTimeText.text = (remainingRefreshSeconds / 3600).ToString() + "h " + (remainingRefreshSeconds % 3600 / 60).ToString() + "m " + (remainingRefreshSeconds % 3600 % 60).ToString() + "s";
        
        if (remainingRefreshSeconds < 1)
        {
            remainingRefreshSeconds = 7200;
            GenerateNewItems();
            StartCoroutine(RefreshCountdown());
        }
        else
        {
            StartCoroutine(RefreshCountdown());
        }
    }
    public void BuyItem(int itemId, bool isCrop, int slotIndex)
    {
        if (isCrop)
        {
            if (PlayerPrefs.GetInt("gold") >= GameBootstrap.Instance.cropDatabase.crops[itemId].salePrice * PlayerPrefs.GetInt("slvp" + slotIndex))
            {
                FarmingPlotPersistence.SetCropQuantity(itemId, FarmingPlotPersistence.GetCropQuantity(itemId) + PlayerPrefs.GetInt("slvp" + slotIndex));
                GameBootstrap.Instance.AddGold(-GameBootstrap.Instance.cropDatabase.crops[itemId].salePrice * PlayerPrefs.GetInt("slvp" + slotIndex));
                itemList.transform.GetChild(slotIndex).GetChild(0).gameObject.SetActive(false);
                itemList.transform.GetChild(slotIndex).GetChild(2).gameObject.SetActive(false);
                itemList.transform.GetChild(slotIndex).GetChild(4).GetComponent<Text>().text = "0";
                InventoryController.instance.UpdateCropQuantity(itemId);
            }
            else
            {
                NotificationController.Instance.ShowCanvasNotification("Không đủ vàng!", "Not enough gold!");
            }
        }
        else
        {
            if (PlayerPrefs.GetInt("gold") >= InventoryController.instance.products[itemId].purchasePrice * PlayerPrefs.GetInt("slvp" + slotIndex))
            {
                PlayerPrefs.SetInt("slvatpham" + itemId, PlayerPrefs.GetInt("slvatpham" + itemId) + PlayerPrefs.GetInt("slvp" + slotIndex));
                GameBootstrap.Instance.AddGold(-InventoryController.instance.products[itemId].purchasePrice * PlayerPrefs.GetInt("slvp" + slotIndex));
                itemList.transform.GetChild(slotIndex).GetChild(0).gameObject.SetActive(false);
                itemList.transform.GetChild(slotIndex).GetChild(2).gameObject.SetActive(false);
                itemList.transform.GetChild(slotIndex).GetChild(4).GetComponent<Text>().text = "0";
                InventoryController.instance.UpdateProductQuantity(itemId);
            }
            else
            {
                NotificationController.Instance.ShowCanvasNotification("Không đủ vàng!", "Not enough gold!");
            }
        }
        OrderPanel.instance.RefreshOrders();
        AudioManager.Instance.click();
    }
}
