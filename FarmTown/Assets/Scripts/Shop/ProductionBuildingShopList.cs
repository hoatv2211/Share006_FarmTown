using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProductionBuildingShopList : MonoBehaviour
{
    public static ProductionBuildingShopList instance;
    private GameObject item;
    void Start()
    {
        instance = this;
        item = transform.GetChild(0).GetChild(0).GetChild(0).gameObject;
        for (int i = 0; i < GameBootstrap.Instance.buildingDatabase.buildings.Length; i++)
        {
            GameObject ob = Instantiate(item, transform.GetChild(0).GetChild(0));
            if (GameBootstrap.Instance.buildingDatabase.buildings[i].unlockLevel > PlayerPrefs.GetInt("level"))
                ob.transform.GetChild(0).GetComponent<Image>().sprite = GameBootstrap.Instance.buildingDatabase.buildings[i].lockedShopIcon;
            else
                ob.transform.GetChild(0).GetComponent<Image>().sprite = GameBootstrap.Instance.buildingDatabase.buildings[i].shopIcon;
            ob.transform.GetChild(0).GetComponent<Image>().SetNativeSize();
            if (Application.systemLanguage == SystemLanguage.Vietnamese)
            {
                ob.transform.GetChild(1).GetComponent<Text>().text = GameBootstrap.Instance.buildingDatabase.buildings[i].vietnameseName;
                if (PlayerPrefs.GetInt("level") < GameBootstrap.Instance.buildingDatabase.buildings[i].secondaryUnlockLevel)
                {
                    if (PlayerPrefs.GetInt("slnhamay" + i) == 0)
                        if (PlayerPrefs.GetInt("level") < GameBootstrap.Instance.buildingDatabase.buildings[i].unlockLevel)
                            ob.transform.GetChild(2).GetComponent<Text>().text = "Mở khóa: lv" + GameBootstrap.Instance.buildingDatabase.buildings[i].unlockLevel;
                        else
                            ob.transform.GetChild(2).gameObject.SetActive(false);
                    else
                    if (PlayerPrefs.GetInt("slnhamay" + i) == 1)
                        ob.transform.GetChild(2).GetComponent<Text>().text = "Mở khóa: lv" + GameBootstrap.Instance.buildingDatabase.buildings[i].secondaryUnlockLevel;
                }
                else
                    ob.transform.GetChild(2).gameObject.SetActive(false);
            }
            else
            {
                ob.transform.GetChild(1).GetComponent<Text>().text = GameBootstrap.Instance.buildingDatabase.buildings[i].englishName;
                if (PlayerPrefs.GetInt("level") < GameBootstrap.Instance.buildingDatabase.buildings[i].secondaryUnlockLevel)
                {
                    if (PlayerPrefs.GetInt("slnhamay" + i) == 0)
                        if (PlayerPrefs.GetInt("level") < GameBootstrap.Instance.buildingDatabase.buildings[i].unlockLevel)
                            ob.transform.GetChild(2).GetComponent<Text>().text = "Unlock: lv" + GameBootstrap.Instance.buildingDatabase.buildings[i].unlockLevel;
                        else
                            ob.transform.GetChild(2).gameObject.SetActive(false);
                    else
                    if (PlayerPrefs.GetInt("slnhamay" + i) == 1)
                        ob.transform.GetChild(2).GetComponent<Text>().text = "Unlock: lv" + GameBootstrap.Instance.buildingDatabase.buildings[i].secondaryUnlockLevel;
                }
                else
                    ob.transform.GetChild(2).gameObject.SetActive(false);
            }
            ob.transform.GetChild(3).GetComponent<Text>().text = PlayerPrefs.GetInt("slnhamay" + i) + "/" + PlayerPrefs.GetInt("slnhamaymax" + i);
            ob.transform.GetChild(4).GetComponent<Text>().text = PlayerPrefs.GetInt("gianhamay" + i).ToString();
            ob.name = GameBootstrap.Instance.buildingDatabase.buildings[i].vietnameseName;
            ob.transform.GetChild(0).GetComponent<ProductionBuildingShopItem>().id = i;
        }
        Destroy(item);
    }

    public void RefreshQuantity(int i)
    {
        transform.GetChild(0).GetChild(0).GetChild(i).GetChild(3).GetComponent<Text>().text = PlayerPrefs.GetInt("slnhamay" + i) + "/" + PlayerPrefs.GetInt("slnhamaymax" + i);
        if (Application.systemLanguage == SystemLanguage.Vietnamese)
        {
            
            if (PlayerPrefs.GetInt("level") < GameBootstrap.Instance.buildingDatabase.buildings[i].secondaryUnlockLevel)
            {
                transform.GetChild(0).GetChild(0).GetChild(i).GetChild(2).gameObject.SetActive(true);
                if (PlayerPrefs.GetInt("slnhamay" + i) == 0)
                    if (PlayerPrefs.GetInt("level") < GameBootstrap.Instance.buildingDatabase.buildings[i].unlockLevel)
                        transform.GetChild(0).GetChild(0).GetChild(i).GetChild(2).GetComponent<Text>().text = "Mở khóa: lv" + GameBootstrap.Instance.buildingDatabase.buildings[i].unlockLevel;
                    else
                        transform.GetChild(0).GetChild(0).GetChild(i).GetChild(2).gameObject.SetActive(false);
                else
                if (PlayerPrefs.GetInt("slnhamay" + i) == 1)
                    transform.GetChild(0).GetChild(0).GetChild(i).GetChild(2).GetComponent<Text>().text = "Mở khóa: lv" + GameBootstrap.Instance.buildingDatabase.buildings[i].secondaryUnlockLevel;
            }
            else
                transform.GetChild(0).GetChild(0).GetChild(i).GetChild(2).gameObject.SetActive(false);
        }
        else
        {
            
            if (PlayerPrefs.GetInt("level") < GameBootstrap.Instance.buildingDatabase.buildings[i].secondaryUnlockLevel)
            {
                transform.GetChild(0).GetChild(0).GetChild(i).GetChild(2).gameObject.SetActive(true);
                if (PlayerPrefs.GetInt("slnhamay" + i) == 0)
                    if (PlayerPrefs.GetInt("level") < GameBootstrap.Instance.buildingDatabase.buildings[i].unlockLevel)
                        transform.GetChild(0).GetChild(0).GetChild(i).GetChild(2).GetComponent<Text>().text = "Unlock: lv" + GameBootstrap.Instance.buildingDatabase.buildings[i].unlockLevel;
                    else
                        transform.GetChild(0).GetChild(0).GetChild(i).GetChild(2).gameObject.SetActive(false);
                else
                if (PlayerPrefs.GetInt("slnhamay" + i) == 1)
                    transform.GetChild(0).GetChild(0).GetChild(i).GetChild(2).GetComponent<Text>().text = "Unlock: lv" + GameBootstrap.Instance.buildingDatabase.buildings[i].secondaryUnlockLevel;
            }
            else
                transform.GetChild(0).GetChild(0).GetChild(i).GetChild(2).gameObject.SetActive(false);
        }
    }
    public void checkIcon(int i)
    {
        if (GameBootstrap.Instance.buildingDatabase.buildings[i].unlockLevel > PlayerPrefs.GetInt("level"))
            transform.GetChild(0).GetChild(0).GetChild(i).GetChild(0).GetComponent<Image>().sprite = GameBootstrap.Instance.buildingDatabase.buildings[i].lockedShopIcon;
        else
        {
            transform.GetChild(0).GetChild(0).GetChild(i).GetChild(2).gameObject.SetActive(false);
            transform.GetChild(0).GetChild(0).GetChild(i).GetChild(0).GetComponent<Image>().sprite = GameBootstrap.Instance.buildingDatabase.buildings[i].shopIcon;
        }
    }
    public void RefreshPrice(int i)
    {
        transform.GetChild(0).GetChild(0).GetChild(i).GetChild(4).GetComponent<Text>().text = PlayerPrefs.GetInt("gianhamay" + i).ToString();
    }
}
