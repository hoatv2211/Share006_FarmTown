using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class OrderPanel : MonoBehaviour
{
    public static OrderPanel instance;
    [FormerlySerializedAs("soDHTheoCap")] public int[] ordersPerLevel;
    [FormerlySerializedAs("nhaDonHang"), SerializeField] private GameObject orderBoard;
    public bool check;
    [FormerlySerializedAs("XeKhong"), SerializeField] private GameObject emptyTruck;
    [FormerlySerializedAs("XeChoHang"), SerializeField] private GameObject cargoTruck;
    [FormerlySerializedAs("XeChoTien"), SerializeField] private GameObject rewardTruck;
    private bool isDeliveryReady;
    public int gold, exp;
    [FormerlySerializedAs("HieuUngKhoi"), SerializeField] private GameObject smokeEffect;
    [FormerlySerializedAs("banhxe1"), SerializeField] private GameObject rearWheel1;
    [FormerlySerializedAs("banhxe2"), SerializeField] private GameObject rearWheel2;
    [FormerlySerializedAs("banhxe3"), SerializeField] private GameObject rearWheel3;
    [FormerlySerializedAs("banhxe4"), SerializeField] private GameObject rearWheel4;
    [FormerlySerializedAs("xenhay"), SerializeField] private GameObject bouncingTruck;
    [FormerlySerializedAs("hieuUngThuHoach"), SerializeField] private GameObject harvestEffectPrefab;
    void Awake()
    {
        if (!PlayerPrefs.HasKey("dh0"))
        {

            PlayerPrefs.SetInt("dh0", 0);
            PlayerPrefs.SetInt("dh1", 1);
            PlayerPrefs.SetInt("dh2", 2);
            PlayerPrefs.SetInt("dh3", 3);
            PlayerPrefs.SetInt("dh4", 20);
            PlayerPrefs.SetInt("dh5", 38);
            PlayerPrefs.SetInt("dh6", 49);
            PlayerPrefs.SetInt("dh7", 61);
        }
        instance = this;
        UpdateOrderAvailability();
    }

    public enum OrderItemType
    {
        Crop = 0,
        Product = 1
    }
    [System.Serializable]
    public struct OrderItemRequirement
    {
        [FormerlySerializedAs("loai")] public OrderItemType itemType;
        [FormerlySerializedAs("idItem")] public int itemId;
        [FormerlySerializedAs("soluong")] public int quantity;
    }
    [System.Serializable]
    public struct OrderDefinition
    {
        public int id;
        [FormerlySerializedAs("ten")] public string vietnameseName;
        [FormerlySerializedAs("name")] public string englishName;
        [FormerlySerializedAs("kinhnghiem")] public int experienceReward;
        [FormerlySerializedAs("vang")] public int goldReward;
        [FormerlySerializedAs("item")] public OrderItemRequirement[] requirements;
    }
    [FormerlySerializedAs("dh")] public OrderDefinition[] orders;
    [FormerlySerializedAs("orderCards")] public GameObject[] orderCards;
    [FormerlySerializedAs("iconTien")] public GameObject goldRewardPrefab;
    [FormerlySerializedAs("iconExp")] public GameObject experienceRewardPrefab;

    public void Open()
    {
        if (!PlayerPrefs.HasKey("huongdan"))
        {
            //MainCamera.Instance.lockCam();
            transform.localScale = Vector3.one;
            gameObject.GetComponent<Animator>().enabled = true;
        gameObject.GetComponent<Animator>().Play("MarketPanelOpen", -1, 0);
            MainCamera.Instance.DisableAll();
            RefreshOrders();
        }
        else
        {
            if (PlayerPrefs.GetInt("huongdan") == 12)
            {
                TutorialController.instance.ShowCanvasMessage("Nhấn để giao hàng", "Click for delivery");
                transform.localScale = Vector3.one;
                gameObject.GetComponent<Animator>().enabled = true;
        gameObject.GetComponent<Animator>().Play("MarketPanelOpen", -1, 0);
                TutorialController.instance.pointerHand.SetActive(false);
                TutorialController.instance.secondaryPointerHand.SetActive(true);
                PlayerPrefs.SetInt("huongdan", 13);
                MainCamera.Instance.DisableAll();
            }
        }

    }
    public void Close()
    {
        if (!PlayerPrefs.HasKey("huongdan"))
        {
            MainCamera.Instance.unLockCam();
            gameObject.GetComponent<Animator>().enabled = false;
            transform.localScale = Vector3.zero;
        }
        else
        {
            if (PlayerPrefs.GetInt("huongdan") == 14)
            {
                MainCamera.Instance.unLockCam();
                gameObject.GetComponent<Animator>().enabled = false;
                transform.localScale = Vector3.zero;
            }
        }
    }
    public void UpdateOrderAvailability()
    {
        if (PlayerPrefs.GetInt("level") < 2)
        {
            PlayerPrefs.SetInt("sodonhang", 1);
            orderCards[0].SetActive(true);
        }
        else if (PlayerPrefs.GetInt("level") >= 2 && PlayerPrefs.GetInt("level") < 6)
        {
            PlayerPrefs.SetInt("sodonhang", 4);
            for (int i = 0; i < 4; i++)
            {
                orderCards[i].SetActive(true);
            }
        }
        else if (PlayerPrefs.GetInt("level") >= 6 && PlayerPrefs.GetInt("level") < 9)
        {
            PlayerPrefs.SetInt("sodonhang", 5);
            for (int i = 0; i < 5; i++)
            {
                orderCards[i].SetActive(true);
            }
        }
        else if (PlayerPrefs.GetInt("level") >= 9 && PlayerPrefs.GetInt("level") < 12)
        {
            PlayerPrefs.SetInt("sodonhang", 6);
            for (int i = 0; i < 6; i++)
            {
                orderCards[i].SetActive(true);
            }
        }
        else if (PlayerPrefs.GetInt("level") >= 12 && PlayerPrefs.GetInt("level") < 16)
        {
            PlayerPrefs.SetInt("sodonhang", 7);
            for (int i = 0; i < 7; i++)
            {
                orderCards[i].SetActive(true);
            }
        }
        else if (PlayerPrefs.GetInt("level") >= 16)
        {
            PlayerPrefs.SetInt("sodonhang", 8);
            for (int i = 0; i < 8; i++)
            {
                orderCards[i].SetActive(true);
            }
        }

    }
    public void RefreshOrders()
    {
        int dem = 0;
        var ob = FindObjectsOfType<Order>();
        for (int i = 0; i < ob.Length; i++)
        {
            ob[i].GetComponent<Order>().RefreshOrder();
            if (ob[i].GetComponent<Order>().isComplete == true)
            {
                if (!PlayerPrefs.HasKey("dangtaidh" + i))
                    dem += 1;
            }
        }
        //Debug.Log(dem);
        if (dem == 0)
        {
            orderBoard.transform.GetChild(0).gameObject.SetActive(false);
        }
        else
        {
            orderBoard.transform.GetChild(0).gameObject.SetActive(true);
        }
    }
    public void StartDelivery()
    {
        check = true;
        StartCoroutine(DeliverySequence());
    }
    IEnumerator DeliverySequence()
    {
        AudioManager.Instance.CarStart();
        bouncingTruck.GetComponent<Animator>().enabled = true;
        rearWheel1.GetComponent<Animator>().enabled = true;
        rearWheel2.GetComponent<Animator>().enabled = true;
        rearWheel3.GetComponent<Animator>().enabled = true;
        rearWheel4.GetComponent<Animator>().enabled = true;
        smokeEffect.SetActive(true);
        emptyTruck.SetActive(false);
        cargoTruck.SetActive(true);
        yield return new WaitForSeconds(21f);
        rewardTruck.SetActive(true);
        cargoTruck.SetActive(false);
        yield return new WaitForSeconds(21f);
        isDeliveryReady = true;
        smokeEffect.SetActive(false);
        rearWheel1.GetComponent<Animator>().enabled = false;
        rearWheel2.GetComponent<Animator>().enabled = false;
        rearWheel3.GetComponent<Animator>().enabled = false;
        rearWheel4.GetComponent<Animator>().enabled = false;
        bouncingTruck.GetComponent<Animator>().enabled = false;
        if (!PlayerPrefs.HasKey("xe"))
        {
            PlayerPrefs.SetInt("xe", 1);
            TutorialController.instance.deliveryTruckPointerHand.SetActive(true);

        }
        AudioManager.Instance.CarEnd();
    }
    public void CollectDeliveryReward()
    {
        if (isDeliveryReady == true)
        {
            GameObject ob = Instantiate(harvestEffectPrefab, GameBootstrap.Instance.RewardTruckTarget.position, Quaternion.identity);
            Destroy(ob, 2f);
            isDeliveryReady = false;
            check = false;
            rewardTruck.SetActive(false);
            emptyTruck.SetActive(true);
            Vector2 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            GameObject tien = Instantiate(goldRewardPrefab, target, Quaternion.identity);
            tien.GetComponent<RewardFlyout>().id = 3;
            tien.GetComponent<RewardFlyout>().numberCoin = gold;
            GameBootstrap.Instance.AddGold(gold);
            GameObject exp1 = Instantiate(experienceRewardPrefab, target, Quaternion.identity);
            exp1.GetComponent<RewardFlyout>().id = 2;
            exp1.GetComponent<RewardFlyout>().numberCoin = exp;
            if (PlayerPrefs.GetInt("xe") == 1)
            {
                TutorialController.instance.deliveryTruckPointerHand.SetActive(false);
            }
            AudioManager.Instance.Harvest();
        }
        else
        {

        }
    }
}
