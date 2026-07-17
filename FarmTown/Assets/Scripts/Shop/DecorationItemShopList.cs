using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DecorationItemShopList : MonoBehaviour
{
    public static DecorationItemShopList instance;
    public GameObject item;
    private void Start()
    {
        instance = this;
        item = transform.GetChild(0).GetChild(0).GetChild(0).gameObject;
        for (int i = 0; i < GameBootstrap.Instance.decorationItemDatabase.decorationItems.Length; i++)
        {
            GameObject ob = Instantiate(item, transform.GetChild(0).GetChild(0));
            if (GameBootstrap.Instance.decorationItemDatabase.decorationItems[i].unlockLevel > PlayerPrefs.GetInt("level"))
                ob.transform.GetChild(0).GetComponent<Image>().sprite = GameBootstrap.Instance.decorationItemDatabase.decorationItems[i].lockedIcon;
            else
                ob.transform.GetChild(0).GetComponent<Image>().sprite = GameBootstrap.Instance.decorationItemDatabase.decorationItems[i].shopIcon;
            ob.transform.GetChild(0).GetComponent<Image>().SetNativeSize();
            if (Application.systemLanguage == SystemLanguage.Vietnamese)
            {
                ob.transform.GetChild(1).GetComponent<Text>().text = GameBootstrap.Instance.decorationItemDatabase.decorationItems[i].vietnameseName;
                if (GameBootstrap.Instance.decorationItemDatabase.decorationItems[i].unlockLevel > PlayerPrefs.GetInt("level"))
                    ob.transform.GetChild(2).GetComponent<Text>().text = "Mở khóa: lv" + GameBootstrap.Instance.decorationItemDatabase.decorationItems[i].unlockLevel;
                else
                    ob.transform.GetChild(2).gameObject.SetActive(false);
            }
            else
            {
                ob.transform.GetChild(1).GetComponent<Text>().text = GameBootstrap.Instance.decorationItemDatabase.decorationItems[i].englishName;
                if (GameBootstrap.Instance.decorationItemDatabase.decorationItems[i].unlockLevel > PlayerPrefs.GetInt("level"))
                    ob.transform.GetChild(2).GetComponent<Text>().text = "Unlock: lv" + GameBootstrap.Instance.decorationItemDatabase.decorationItems[i].unlockLevel;
                else
                    ob.transform.GetChild(2).gameObject.SetActive(false);
            }
            ob.transform.GetChild(4).GetComponent<Text>().text = GameBootstrap.Instance.decorationItemDatabase.decorationItems[i].purchasePrice.ToString();
            ob.transform.GetChild(0).GetComponent<DecorationItemShopItem>().decorationId = i;
        }
        Destroy(item);
    }
    public void check(int i)
    {
        if (GameBootstrap.Instance.decorationItemDatabase.decorationItems[i].unlockLevel > PlayerPrefs.GetInt("level"))
            transform.GetChild(0).GetChild(0).GetChild(i).GetChild(0).GetComponent<Image>().sprite = GameBootstrap.Instance.decorationItemDatabase.decorationItems[i].lockedIcon;
        else
        {
            transform.GetChild(0).GetChild(0).GetChild(i).GetChild(0).GetComponent<Image>().sprite = GameBootstrap.Instance.decorationItemDatabase.decorationItems[i].shopIcon;
            transform.GetChild(0).GetChild(0).GetChild(i).GetChild(2).gameObject.SetActive(false);
        }
    }
}
