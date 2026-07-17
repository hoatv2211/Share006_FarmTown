using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ProductionPanel : MonoBehaviour
{
    public static ProductionPanel instance;
    Animator anim;
    // id:0 nha may banh my, 5 san pham   0->5
    //id:1 may rang bap, 5 san pham       6->11
    //id: 2 nha may sua bo, 5 san pham    12->17
    //id: 3 lo pizza , 4 san pham         18->23
    //id: 4 gato, 5 san pham              24->29
    //id: 5 soup, 5 san pham              30->35
    //id: 6 bep nuong, 3 san pham         36->41
    [SerializeField] GameObject[] listIconSp;
    [SerializeField] Text nameNM;
    private void Start()
    {
        instance = this;
        anim = GetComponent<Animator>();

    }
    public void Open()
    {
        for (int i = 0; i < 5; i++)
        {
            listIconSp[i].transform.GetChild(0).gameObject.SetActive(false);
        }
        //MainCamera.Instance.lockCam();
        transform.localScale = Vector3.one;
        anim.enabled = true;
        anim.Play("ProductionPanelOpen", -1, 0);
    }
    public void Close()
    {
        if (transform.GetChild(1).localScale == Vector3.one)
        {
            if (!PlayerPrefs.HasKey("huongdan"))
            {
                MainCamera.Instance.unLockCam();
                transform.localScale = Vector3.zero;
                transform.GetChild(1).localScale = Vector3.zero;
                anim.enabled = false;
            }
            else
            {
                MainCamera.Instance.unLockCam();
                transform.localScale = Vector3.zero;
                transform.GetChild(1).localScale = Vector3.zero;
                anim.enabled = false;
            }
        }
    }
    public void ShowBuildingProducts(int buildingTypeId)
    {
        if (Application.systemLanguage == SystemLanguage.Vietnamese)
            nameNM.text = GameBootstrap.Instance.buildingDatabase.buildings[buildingTypeId].vietnameseName;
        else
            nameNM.text = GameBootstrap.Instance.buildingDatabase.buildings[buildingTypeId].englishName;
        switch (buildingTypeId)
        {
            case 0:
                int id0 = 0;
                for (int i = 0; i < 6; i++)
                {
                    listIconSp[i].transform.GetChild(0).gameObject.SetActive(true);
                    listIconSp[i].transform.GetChild(0).GetComponent<Image>().sprite = InventoryController.instance.products[id0].icon;
                    listIconSp[i].transform.GetChild(0).GetComponent<ProductItem>().productId = id0;
                    if (PlayerPrefs.GetInt("level") < InventoryController.instance.products[id0].unlockLevel)
                    {
                        listIconSp[i].transform.GetChild(0).gameObject.SetActive(false);
                        listIconSp[i].transform.GetChild(2).gameObject.SetActive(true);
                    }
                    else
                    {
                        listIconSp[i].transform.GetChild(0).gameObject.SetActive(true);
                        listIconSp[i].transform.GetChild(2).gameObject.SetActive(false);
                    }
                    id0 += 1;
                }
                break;
            case 1:
                int id1 = 6;
                for (int i = 0; i < 6; i++)
                {
                    listIconSp[i].transform.GetChild(0).gameObject.SetActive(true);
                    listIconSp[i].transform.GetChild(0).GetComponent<Image>().sprite = InventoryController.instance.products[id1].icon;
                    listIconSp[i].transform.GetChild(0).GetComponent<ProductItem>().productId = id1;
                    if (PlayerPrefs.GetInt("level") < InventoryController.instance.products[id1].unlockLevel)
                    {
                        listIconSp[i].transform.GetChild(0).gameObject.SetActive(false);
                        listIconSp[i].transform.GetChild(2).gameObject.SetActive(true);
                    }
                    else
                    {
                        listIconSp[i].transform.GetChild(0).gameObject.SetActive(true);
                        listIconSp[i].transform.GetChild(2).gameObject.SetActive(false);
                    }
                    id1 += 1;
                }
                break;
            case 2:
                int id2 = 12;
                for (int i = 0; i < 6; i++)
                {
                    listIconSp[i].transform.GetChild(0).gameObject.SetActive(true);
                    listIconSp[i].transform.GetChild(0).GetComponent<Image>().sprite = InventoryController.instance.products[id2].icon;
                    listIconSp[i].transform.GetChild(0).GetComponent<ProductItem>().productId = id2;
                    if (PlayerPrefs.GetInt("level") < InventoryController.instance.products[id2].unlockLevel)
                    {
                        listIconSp[i].transform.GetChild(0).gameObject.SetActive(false);
                        listIconSp[i].transform.GetChild(2).gameObject.SetActive(true);
                    }
                    else
                    {
                        listIconSp[i].transform.GetChild(0).gameObject.SetActive(true);
                        listIconSp[i].transform.GetChild(2).gameObject.SetActive(false);
                    }
                    id2 += 1;
                }
                break;
            case 3:
                int id3 = 18;
                for (int i = 0; i < 6; i++)
                {
                    listIconSp[i].transform.GetChild(0).gameObject.SetActive(true);
                    listIconSp[i].transform.GetChild(0).GetComponent<Image>().sprite = InventoryController.instance.products[id3].icon;
                    listIconSp[i].transform.GetChild(0).GetComponent<ProductItem>().productId = id3;
                    if (PlayerPrefs.GetInt("level") < InventoryController.instance.products[id3].unlockLevel)
                    {
                        listIconSp[i].transform.GetChild(0).gameObject.SetActive(false);
                        listIconSp[i].transform.GetChild(2).gameObject.SetActive(true);
                    }
                    else
                    {
                        listIconSp[i].transform.GetChild(0).gameObject.SetActive(true);
                        listIconSp[i].transform.GetChild(2).gameObject.SetActive(false);
                    }
                    id3 += 1;
                }
                break;
            case 4:
                int id4 = 24;
                for (int i = 0; i < 6; i++)
                {
                    listIconSp[i].transform.GetChild(0).gameObject.SetActive(true);
                    listIconSp[i].transform.GetChild(0).GetComponent<Image>().sprite = InventoryController.instance.products[id4].icon;
            listIconSp[i].transform.GetChild(0).GetComponent<ProductItem>().productId = id4;
                    if (PlayerPrefs.GetInt("level") < InventoryController.instance.products[id4].unlockLevel)
                    {
                        listIconSp[i].transform.GetChild(0).gameObject.SetActive(false);
                        listIconSp[i].transform.GetChild(2).gameObject.SetActive(true);
                    }
                    else
                    {
                        listIconSp[i].transform.GetChild(0).gameObject.SetActive(true);
                        listIconSp[i].transform.GetChild(2).gameObject.SetActive(false);
                    }
                    id4 += 1;
                }
                break;
            case 5:
                int id5 = 30;
                for (int i = 0; i < 6; i++)
                {
                    listIconSp[i].transform.GetChild(0).gameObject.SetActive(true);
                    listIconSp[i].transform.GetChild(0).GetComponent<Image>().sprite = InventoryController.instance.products[id5].icon;
            listIconSp[i].transform.GetChild(0).GetComponent<ProductItem>().productId = id5;
                    if (PlayerPrefs.GetInt("level") < InventoryController.instance.products[id5].unlockLevel)
                    {
                        listIconSp[i].transform.GetChild(0).gameObject.SetActive(false);
                        listIconSp[i].transform.GetChild(2).gameObject.SetActive(true);
                    }
                    else
                    {
                        listIconSp[i].transform.GetChild(0).gameObject.SetActive(true);
                        listIconSp[i].transform.GetChild(2).gameObject.SetActive(false);
                    }
                    id5 += 1;
                }
                break;
            case 6:
                int id6 = 36;
                for (int i = 0; i < 6; i++)
                {
                    listIconSp[i].transform.GetChild(0).gameObject.SetActive(true);
                    listIconSp[i].transform.GetChild(0).GetComponent<Image>().sprite = InventoryController.instance.products[id6].icon;
            listIconSp[i].transform.GetChild(0).GetComponent<ProductItem>().productId = id6;
                    if (PlayerPrefs.GetInt("level") < InventoryController.instance.products[id6].unlockLevel)
                    {
                        listIconSp[i].transform.GetChild(0).gameObject.SetActive(false);
                        listIconSp[i].transform.GetChild(2).gameObject.SetActive(true);
                    }
                    else
                    {
                        listIconSp[i].transform.GetChild(0).gameObject.SetActive(true);
                        listIconSp[i].transform.GetChild(2).gameObject.SetActive(false);
                    }
                    id6 += 1;
                }
                break;
        }
    }
}
