using FarmTown.Save;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
public class Order : MonoBehaviour
{
    [FormerlySerializedAs("txtName"), SerializeField] private Text nameText;
    [FormerlySerializedAs("txtGold"), SerializeField] private Text goldText;
    [FormerlySerializedAs("txtExp"), SerializeField] private Text experienceText;
    [FormerlySerializedAs("txtTimeCho"), SerializeField] private Text waitTimeText;
    [FormerlySerializedAs("listVp"), SerializeField] private GameObject itemList;
    [FormerlySerializedAs("btnban"), SerializeField] private GameObject completeButton;
    [FormerlySerializedAs("iddh"), SerializeField] private int orderDefinitionId;
    int time;
    [FormerlySerializedAs("id"), SerializeField] private int slotId;
    [FormerlySerializedAs("iconTien"), SerializeField] private GameObject goldIcon;
    [FormerlySerializedAs("iconExp"), SerializeField] private GameObject experienceIcon;
    [FormerlySerializedAs("hoanThanh")] public bool isComplete;
    // Use this for initialization
    void Start()
    {
        orderDefinitionId = PlayerPrefs.GetInt("dh" + slotId);
        BindOrder();
        OrderPanel.instance.RefreshOrders();
        if (PlayerPrefs.HasKey("dangtaidh" + slotId))
        {
            int timenew = GameTime.TimeCurrent();
            int timethoat = Mathf.Abs(timenew - PlayerPrefs.GetInt("timethoatdh" + slotId));
            if (timethoat >= PlayerPrefs.GetInt("timedh" + slotId))
            {
                PlayerPrefs.DeleteKey("dangtaidh" + slotId);
                LoadNewOrder();
            }
            else
            {
                isComplete = false;
                time = PlayerPrefs.GetInt("timedh" + slotId) - timethoat;
                if (Application.systemLanguage == SystemLanguage.Vietnamese)
                {
                    nameText.text = "Xin chờ...";

                }
                else
                {
                    nameText.text = "Wait...";

                }
                completeButton.transform.GetChild(0).gameObject.SetActive(false);
                completeButton.transform.GetChild(1).gameObject.SetActive(true);
                waitTimeText.gameObject.SetActive(true);
                goldText.text = "0";
                experienceText.text = "0";
                for (int i = 0; i < itemList.transform.childCount; i++)
                {
                    itemList.transform.GetChild(i).gameObject.SetActive(false);
                }
                StartCoroutine(WaitForRefresh());
            }
        }
    }
    public void CompleteOrder()
    {
        if (PlayerPrefs.GetInt("huongdan") == 13)
        {
            TutorialController.instance.secondaryPointerHand.SetActive(false);
            PlayerPrefs.SetInt("huongdan", 14);
            MainCamera.Instance.MoveCamera(new Vector2(GameBootstrap.Instance.TutorialForestTreeTarget.position.x, GameBootstrap.Instance.TutorialForestTreeTarget.position.y + 2f), false);
            TutorialController.instance.pointerHand.SetActive(true);
            TutorialController.instance.MovePointerHand(GameBootstrap.Instance.TutorialForestTreeTarget.position);
            TutorialController.instance.ShowWorldMessage("Nhấn vào cây rừng", "Click on the forest tree");
        }
        if (PlayerPrefs.HasKey("dangtaidh" + slotId))
        {
            if (PlayerPrefs.GetInt("diamond") >= 2)
            {
                GameBootstrap.Instance.AddDiamonds(-2);
                time = 0;
            }
            else
            {
                NotificationController.Instance.ShowCanvasNotification("Bạn không đủ kim cương!", "You don't have enough diamonds!");
            }
        }
        else
        {
            if (OrderPanel.instance.check == false)
            {
                if (isComplete == true)
                {
                    for (int i = 0; i < OrderPanel.instance.orders[orderDefinitionId].requirements.Length; i++)
                    {
                        int idNL = OrderPanel.instance.orders[orderDefinitionId].requirements[i].itemId;
                        if (OrderPanel.instance.orders[orderDefinitionId].requirements[i].itemType == 0)
                        {
                            FarmingPlotPersistence.SetCropQuantity(idNL, FarmingPlotPersistence.GetCropQuantity(idNL) - OrderPanel.instance.orders[orderDefinitionId].requirements[i].quantity);
                            if (FarmingPlotPersistence.GetCropQuantity(idNL) < 0)
                                FarmingPlotPersistence.SetCropQuantity(idNL, 0);
                            InventoryController.instance.UpdateCropQuantity(idNL);
                        }
                        else
                        {
                            PlayerPrefs.SetInt("slvatpham" + idNL, PlayerPrefs.GetInt("slvatpham" + idNL) - OrderPanel.instance.orders[orderDefinitionId].requirements[i].quantity);

                            if (PlayerPrefs.GetInt("slvatpham" + idNL) < 0)
                                PlayerPrefs.SetInt("slvatpham" + idNL, 0);
                            InventoryController.instance.UpdateProductQuantity(idNL);
                        }
                    }

                    OrderPanel.instance.gold = OrderPanel.instance.orders[orderDefinitionId].goldReward;
                    OrderPanel.instance.exp = OrderPanel.instance.orders[orderDefinitionId].experienceReward;

                    OrderPanel.instance.StartDelivery();
                    OrderPanel.instance.Close();
                    LoadNewOrder();
                }
                else
                {
                    NotificationController.Instance.ShowCanvasNotification("Bạn chưa hoàn thành đơn hàng!", "You have not completed the order!");
                }
            }
            else
            {
                NotificationController.Instance.ShowCanvasNotification("Xe chưa về!", "The car is not back yet!");
            }
        }


    }

    public void CancelOrder()
    {
        if (!PlayerPrefs.HasKey("huongdan"))
        {
            if (!PlayerPrefs.HasKey("dangtaidh" + slotId))
            {
                isComplete = false;
                PlayerPrefs.SetInt("dangtaidh" + slotId, 1);
                ShowCancelledState();
                OrderPanel.instance.RefreshOrders();
            }
        }
    }
    public void ShowCancelledState()
    {
        time = 120;
        if (Application.systemLanguage == SystemLanguage.Vietnamese)
        {
            nameText.text = "Xin chờ...";

        }
        else
        {
            nameText.text = "Wait...";

        }
        completeButton.transform.GetChild(0).gameObject.SetActive(false);
        completeButton.transform.GetChild(1).gameObject.SetActive(true);
        waitTimeText.gameObject.SetActive(true);
        goldText.text = "0";
        experienceText.text = "0";
        for (int i = 0; i < itemList.transform.childCount; i++)
        {
            itemList.transform.GetChild(i).gameObject.SetActive(false);
        }
        StartCoroutine(WaitForRefresh());
    }
    IEnumerator WaitForRefresh()
    {
        yield return new WaitForSeconds(1f);
        time -= 1;
        PlayerPrefs.SetInt("timedh" + slotId, time);
        PlayerPrefs.SetInt("timethoatdh" + slotId, GameTime.TimeCurrent());
        if (Application.systemLanguage == SystemLanguage.Vietnamese)
        {
            waitTimeText.text = "Đơn hàng tiếp trong: " + time / 60 + "m " + time % 60 + "s.";
        }
        else
        {
            waitTimeText.text = "Next order in: " + time / 60 + "m " + time % 60 + "s.";
        }
        if (time > 0)
        {
            StartCoroutine(WaitForRefresh());
        }
        else
        {
            PlayerPrefs.DeleteKey("dangtaidh" + slotId);
            completeButton.transform.GetChild(0).gameObject.SetActive(true);
            completeButton.transform.GetChild(1).gameObject.SetActive(false);
            waitTimeText.gameObject.SetActive(false);
            //load don hang moi
            LoadNewOrder();
        }
    }
    public void LoadNewOrder()
    {
        int slDhTheoCap;
        if (PlayerPrefs.GetInt("level") < 42)
            slDhTheoCap = OrderPanel.instance.ordersPerLevel[PlayerPrefs.GetInt("level") - 1];
        else
            slDhTheoCap = 112;
        //mang sl cac dh co san
        int[] mang;
        mang = new int[PlayerPrefs.GetInt("sodonhang")];
        for (int i = 0; i < mang.Length; i++)
        {
            mang[i] = PlayerPrefs.GetInt("dh" + (i + 1).ToString());
        }
        // mang tong cac dh
        ArrayList mangsldh = new ArrayList();
        for (int i = 0; i < slDhTheoCap; i++)
        {
            mangsldh.Add(i);
        }
        //xoa gia tri bi trung
        for (int i = 0; i < mang.Length; i++)
        {
            for (int j = 0; j < mangsldh.Count; j++)
            {
                if (mang[i] == (int)mangsldh[j])
                    mangsldh.Remove(mangsldh[j]);
            }
        }
        PlayerPrefs.SetInt("dh" + slotId, (int)mangsldh[Random.Range(0, mangsldh.Count)]);
        orderDefinitionId = PlayerPrefs.GetInt("dh" + slotId);
        BindOrder();
        OrderPanel.instance.RefreshOrders();
    }
    public void BindOrder()
    {
        for (int i = 0; i < 4; i++)
        {
            itemList.transform.GetChild(i).gameObject.SetActive(false);
        }
        for (int i = 0; i < OrderPanel.instance.orders[orderDefinitionId].requirements.Length; i++)
        {
            itemList.transform.GetChild(i).gameObject.SetActive(true);
            int idNL = OrderPanel.instance.orders[orderDefinitionId].requirements[i].itemId;
            if (OrderPanel.instance.orders[orderDefinitionId].requirements[i].itemType == 0)
            {
                itemList.transform.GetChild(i).GetComponent<Image>().sprite = GameBootstrap.Instance.cropDatabase.crops[idNL].productIcon;
                itemList.transform.GetChild(i).GetChild(0).GetComponent<Text>().text = FarmingPlotPersistence.GetCropQuantity(idNL) + "/" + OrderPanel.instance.orders[orderDefinitionId].requirements[i].quantity;
                if (FarmingPlotPersistence.GetCropQuantity(idNL) >= OrderPanel.instance.orders[orderDefinitionId].requirements[i].quantity)
                    itemList.transform.GetChild(i).GetChild(1).gameObject.SetActive(true);
                else
                    itemList.transform.GetChild(i).GetChild(1).gameObject.SetActive(false);
            }
            else
            {
                itemList.transform.GetChild(i).GetComponent<Image>().sprite = InventoryController.instance.products[idNL].icon;
                itemList.transform.GetChild(i).GetChild(0).GetComponent<Text>().text = PlayerPrefs.GetInt("slvatpham" + idNL) + "/" + OrderPanel.instance.orders[orderDefinitionId].requirements[i].quantity;
                if (PlayerPrefs.GetInt("slvatpham" + idNL) >= OrderPanel.instance.orders[orderDefinitionId].requirements[i].quantity)
                    itemList.transform.GetChild(i).GetChild(1).gameObject.SetActive(true);
                else
                    itemList.transform.GetChild(i).GetChild(1).gameObject.SetActive(false);
            }
        }
        if (Application.systemLanguage == SystemLanguage.Vietnamese)
        {
            nameText.text = OrderPanel.instance.orders[orderDefinitionId].vietnameseName;

        }
        else
        {
            nameText.text = OrderPanel.instance.orders[orderDefinitionId].englishName;

        }
        completeButton.transform.GetChild(0).gameObject.SetActive(true);
        completeButton.transform.GetChild(1).gameObject.SetActive(false);
        goldText.text = OrderPanel.instance.orders[orderDefinitionId].goldReward.ToString();
        experienceText.text = OrderPanel.instance.orders[orderDefinitionId].experienceReward.ToString();
    }
    public void RefreshOrder()
    {
        if (!PlayerPrefs.HasKey("dangtaidh" + slotId))
        {
            int dem = 0;
            for (int i = 0; i < OrderPanel.instance.orders[orderDefinitionId].requirements.Length; i++)
            {

                int idNL = OrderPanel.instance.orders[orderDefinitionId].requirements[i].itemId;
                if (OrderPanel.instance.orders[orderDefinitionId].requirements[i].itemType == 0)
                {

                    itemList.transform.GetChild(i).GetChild(0).GetComponent<Text>().text = FarmingPlotPersistence.GetCropQuantity(idNL) + "/" + OrderPanel.instance.orders[orderDefinitionId].requirements[i].quantity;
                    if (FarmingPlotPersistence.GetCropQuantity(idNL) >= OrderPanel.instance.orders[orderDefinitionId].requirements[i].quantity)
                    {
                        dem += 1;
                        itemList.transform.GetChild(i).GetChild(1).gameObject.SetActive(true);
                    }
                    else
                        itemList.transform.GetChild(i).GetChild(1).gameObject.SetActive(false);
                }
                else
                {

                    itemList.transform.GetChild(i).GetChild(0).GetComponent<Text>().text = PlayerPrefs.GetInt("slvatpham" + idNL) + "/" + OrderPanel.instance.orders[orderDefinitionId].requirements[i].quantity;
                    if (PlayerPrefs.GetInt("slvatpham" + idNL) >= OrderPanel.instance.orders[orderDefinitionId].requirements[i].quantity)
                    {
                        dem += 1;
                        itemList.transform.GetChild(i).GetChild(1).gameObject.SetActive(true);
                    }
                    else
                        itemList.transform.GetChild(i).GetChild(1).gameObject.SetActive(false);
                }
            }
            if (dem == OrderPanel.instance.orders[orderDefinitionId].requirements.Length)
            {
                isComplete = true;
            }
            else
            {
                isComplete = false;
            }
        }
        else
        {
            isComplete = false;
        }
    }
}
