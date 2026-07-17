using System.Collections;
using System.Collections.Generic;
using FarmTown.Save;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;
public class AnimalPenShopList : MonoBehaviour
{
    public static AnimalPenShopList instance;
    private GameObject itemTemplate;
    [FormerlySerializedAs("iconOdat"), SerializeField] Sprite cropPlotIcon;
    [FormerlySerializedAs("iconodatlock"), SerializeField] Sprite lockedCropPlotIcon;
    private void Start()
    {
        instance = this;
        itemTemplate = transform.GetChild(0).GetChild(0).GetChild(0).gameObject;
        //o dat
        GameObject obj = Instantiate(itemTemplate, transform.GetChild(0).GetChild(0));
        if (PlayerPrefs.GetInt("soodat") == PlayerPrefs.GetInt("soodatmax"))
            obj.transform.GetChild(0).GetComponent<Image>().sprite = lockedCropPlotIcon;
        else
            obj.transform.GetChild(0).GetComponent<Image>().sprite = cropPlotIcon;
        obj.transform.GetChild(0).GetComponent<Image>().SetNativeSize();
        if (Application.systemLanguage == SystemLanguage.Vietnamese)
        {
            obj.transform.GetChild(1).GetComponent<Text>().text = "Ô đất";
            if (PlayerPrefs.GetInt("soodat") == PlayerPrefs.GetInt("soodatmax"))
                if (PlayerPrefs.GetInt("soodatmax") < 33)
                    obj.transform.GetChild(2).GetComponent<Text>().text = "Mở khóa: lv" + PlayerPrefs.GetInt("capmoodat");
                else
                    obj.transform.GetChild(2).gameObject.SetActive(false);
            else
                obj.transform.GetChild(2).gameObject.SetActive(false);
        }
        else
        {
            obj.transform.GetChild(1).GetComponent<Text>().text = "Slot Land";
            if (PlayerPrefs.GetInt("soodat") == PlayerPrefs.GetInt("soodatmax"))
                if (PlayerPrefs.GetInt("soodatmax") < 33)
                    obj.transform.GetChild(2).GetComponent<Text>().text = "Unlock: lv" + PlayerPrefs.GetInt("capmoodat");
                else
                    obj.transform.GetChild(2).gameObject.SetActive(false);
            else
                obj.transform.GetChild(2).gameObject.SetActive(false);
        }
        obj.transform.GetChild(3).GetComponent<Text>().text = PlayerPrefs.GetInt("soodat") + "/" + PlayerPrefs.GetInt("soodatmax");
        obj.transform.GetChild(4).GetComponent<Text>().text = PlayerPrefs.GetInt("giamuaodat").ToString();
        obj.name = "ô đất";
        obj.transform.GetChild(0).GetComponent<AnimalPenShopItem>().itemTypeId = 7;
        // Animal pens.
        for (int i = 0; i < GameBootstrap.Instance.animalDatabase.animals.Length; i++)
        {
            GameObject ob = Instantiate(itemTemplate, transform.GetChild(0).GetChild(0));
            if (GameBootstrap.Instance.animalDatabase.animals[i].animalUnlockLevel > PlayerPrefs.GetInt("level"))
                ob.transform.GetChild(0).GetComponent<Image>().sprite = GameBootstrap.Instance.animalDatabase.animals[i].lockedPenIcon;
            else
                ob.transform.GetChild(0).GetComponent<Image>().sprite = GameBootstrap.Instance.animalDatabase.animals[i].penIcon;
            ob.transform.GetChild(0).GetComponent<Image>().SetNativeSize();
            if (Application.systemLanguage == SystemLanguage.Vietnamese)
            {
                ob.transform.GetChild(1).GetComponent<Text>().text = GameBootstrap.Instance.animalDatabase.animals[i].vietnamesePenName;
                if (PlayerPrefs.GetInt("level") < GameBootstrap.Instance.animalDatabase.animals[i].secondPenUnlockLevel)
                {
                    if (AnimalPersistence.GetPenQuantity(i) == 0)
                        if (PlayerPrefs.GetInt("level") < GameBootstrap.Instance.animalDatabase.animals[i].animalUnlockLevel)
                            ob.transform.GetChild(2).GetComponent<Text>().text = "Mở khóa: lv" + GameBootstrap.Instance.animalDatabase.animals[i].animalUnlockLevel;
                        else
                            ob.transform.GetChild(2).gameObject.SetActive(false);
                    else
                    if (AnimalPersistence.GetPenQuantity(i) == 1)
                        ob.transform.GetChild(2).GetComponent<Text>().text = "Mở khóa: lv" + GameBootstrap.Instance.animalDatabase.animals[i].secondPenUnlockLevel;
                    //else
                    //    ob.transform.GetChild(2).gameObject.SetActive(false);
                }
                else
                    ob.transform.GetChild(2).gameObject.SetActive(false);

            }
            else
            {
                ob.transform.GetChild(1).GetComponent<Text>().text = GameBootstrap.Instance.animalDatabase.animals[i].vietnamesePenName;
                if (PlayerPrefs.GetInt("level") < GameBootstrap.Instance.animalDatabase.animals[i].secondPenUnlockLevel)
                {
                    if (AnimalPersistence.GetPenQuantity(i) == 0)
                        if (PlayerPrefs.GetInt("level") < GameBootstrap.Instance.animalDatabase.animals[i].animalUnlockLevel)
                            ob.transform.GetChild(2).GetComponent<Text>().text = "Unlock: lv" + GameBootstrap.Instance.animalDatabase.animals[i].animalUnlockLevel;
                        else
                            ob.transform.GetChild(2).gameObject.SetActive(false);
                    else
                    if (AnimalPersistence.GetPenQuantity(i) == 1)
                        ob.transform.GetChild(2).GetComponent<Text>().text = "Unlock: lv" + GameBootstrap.Instance.animalDatabase.animals[i].secondPenUnlockLevel;
                    //else
                    //    ob.transform.GetChild(2).gameObject.SetActive(false);
                }
                else
                    ob.transform.GetChild(2).gameObject.SetActive(false);
            }
            ob.transform.GetChild(3).GetComponent<Text>().text = AnimalPersistence.GetPenQuantity(i) + "/" + AnimalPersistence.GetPenLimit(i);
            ob.transform.GetChild(4).GetComponent<Text>().text = AnimalPersistence.GetPenPrice(i).ToString();
            ob.name = GameBootstrap.Instance.animalDatabase.animals[i].vietnamesePenName;
            ob.transform.GetChild(0).GetComponent<AnimalPenShopItem>().itemTypeId = i;
        }
        Destroy(itemTemplate);
    }
    public void RefreshQuantity(int i)
    {
        if (i == 0)
        {
            transform.GetChild(0).GetChild(0).GetChild(i).GetChild(3).GetComponent<Text>().text = PlayerPrefs.GetInt("soodat") + "/" + PlayerPrefs.GetInt("soodatmax");
            if (Application.systemLanguage == SystemLanguage.Vietnamese)
            {

                if (PlayerPrefs.GetInt("soodat") == PlayerPrefs.GetInt("soodatmax"))
                {
                    if (PlayerPrefs.GetInt("soodatmax") < 33)
                    {
                        transform.GetChild(0).GetChild(0).GetChild(i).GetChild(2).gameObject.SetActive(true);
                        transform.GetChild(0).GetChild(0).GetChild(i).GetChild(2).GetComponent<Text>().text = "Mở khóa: lv" + PlayerPrefs.GetInt("capmoodat");
                    }
                    else
                    {
                        transform.GetChild(0).GetChild(0).GetChild(i).GetChild(2).gameObject.SetActive(false);
                    }
                }
                else
                {
                    transform.GetChild(0).GetChild(0).GetChild(i).GetChild(0).GetComponent<Image>().sprite = cropPlotIcon;
                    transform.GetChild(0).GetChild(0).GetChild(i).GetChild(2).gameObject.SetActive(false);
                }
            }
            else
            {

                if (PlayerPrefs.GetInt("soodat") == PlayerPrefs.GetInt("soodatmax"))
                {
                    if (PlayerPrefs.GetInt("soodatmax") < 33)
                    {
                        transform.GetChild(0).GetChild(0).GetChild(i).GetChild(2).gameObject.SetActive(true);
                        transform.GetChild(0).GetChild(0).GetChild(i).GetChild(2).GetComponent<Text>().text = "Unlock: lv" + PlayerPrefs.GetInt("capmoodat");
                    }
                    else
                    {
                        transform.GetChild(0).GetChild(0).GetChild(i).GetChild(2).gameObject.SetActive(false);
                    }
                }
                else
                {
                    transform.GetChild(0).GetChild(0).GetChild(i).GetChild(0).GetComponent<Image>().sprite = cropPlotIcon;
                    transform.GetChild(0).GetChild(0).GetChild(i).GetChild(2).gameObject.SetActive(false);
                }
            }
        }
        else
        {
            transform.GetChild(0).GetChild(0).GetChild(i).GetChild(3).GetComponent<Text>().text = AnimalPersistence.GetPenQuantity(i - 1) + "/" + AnimalPersistence.GetPenLimit(i - 1);
            if (Application.systemLanguage == SystemLanguage.Vietnamese)
            {
                if (PlayerPrefs.GetInt("level") < GameBootstrap.Instance.animalDatabase.animals[i - 1].secondPenUnlockLevel)
                {
                    transform.GetChild(0).GetChild(0).GetChild(i).GetChild(2).gameObject.SetActive(true);
                    if (AnimalPersistence.GetPenQuantity(i - 1) == 0)
                        if (PlayerPrefs.GetInt("level") < GameBootstrap.Instance.animalDatabase.animals[i - 1].animalUnlockLevel)
                            transform.GetChild(0).GetChild(0).GetChild(i).GetChild(2).GetComponent<Text>().text = "Mở khóa: lv" + GameBootstrap.Instance.animalDatabase.animals[i - 1].animalUnlockLevel;
                        else
                            transform.GetChild(0).GetChild(0).GetChild(i).GetChild(2).gameObject.SetActive(false);
                    else
                    if (AnimalPersistence.GetPenQuantity(i - 1) == 1)
                        transform.GetChild(0).GetChild(0).GetChild(i).GetChild(2).GetComponent<Text>().text = "Mở khóa: lv" + GameBootstrap.Instance.animalDatabase.animals[i - 1].secondPenUnlockLevel;
                }
                else
                    transform.GetChild(0).GetChild(0).GetChild(i).GetChild(2).gameObject.SetActive(false);

            }
            else
            {

                if (PlayerPrefs.GetInt("level") < GameBootstrap.Instance.animalDatabase.animals[i - 1].secondPenUnlockLevel)
                {
                    transform.GetChild(0).GetChild(0).GetChild(i).GetChild(2).gameObject.SetActive(true);
                    if (AnimalPersistence.GetPenQuantity(i - 1) == 0)
                        if (PlayerPrefs.GetInt("level") < GameBootstrap.Instance.animalDatabase.animals[i - 1].animalUnlockLevel)
                            transform.GetChild(0).GetChild(0).GetChild(i).GetChild(2).GetComponent<Text>().text = "Unlock: lv" + GameBootstrap.Instance.animalDatabase.animals[i - 1].animalUnlockLevel;
                        else
                            transform.GetChild(0).GetChild(0).GetChild(i).GetChild(2).gameObject.SetActive(false);
                    else
                    if (AnimalPersistence.GetPenQuantity(i - 1) == 1)
                        transform.GetChild(0).GetChild(0).GetChild(i).GetChild(2).GetComponent<Text>().text = "Unlock: lv" + GameBootstrap.Instance.animalDatabase.animals[i - 1].secondPenUnlockLevel;
                }
                else
                    transform.GetChild(0).GetChild(0).GetChild(i).GetChild(2).gameObject.SetActive(false);
            }
        }
    }
    public void checkIcon(int i)
    {

        if (GameBootstrap.Instance.animalDatabase.animals[i - 1].animalUnlockLevel > PlayerPrefs.GetInt("level"))
        {
            transform.GetChild(0).GetChild(0).GetChild(i).GetChild(0).GetComponent<Image>().sprite = GameBootstrap.Instance.animalDatabase.animals[i - 1].lockedPenIcon;
        }
        else
        {
            transform.GetChild(0).GetChild(0).GetChild(i).GetChild(0).GetComponent<Image>().sprite = GameBootstrap.Instance.animalDatabase.animals[i - 1].penIcon;
            //transform.GetChild(0).GetChild(0).GetChild(i).GetChild(2).gameObject.SetActive(false);
        }

    }
    public void RefreshPriceAvailability(int i)
    {
        transform.GetChild(0).GetChild(0).GetChild(i + 1).GetChild(4).GetComponent<Text>().text = AnimalPersistence.GetPenPrice(i).ToString();
    }
}
