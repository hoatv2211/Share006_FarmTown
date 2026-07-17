using System.Collections;
using System.Collections.Generic;
using FarmTown.Save;
using UnityEngine;
using UnityEngine.UI;

public class AnimalShopList : MonoBehaviour
{
    public static AnimalShopList instance;
    private GameObject itemTemplate;
    private void Start()
    {
        instance = this;
        itemTemplate = transform.GetChild(0).GetChild(0).GetChild(0).gameObject;
        for (int i = 0; i < GameBootstrap.Instance.animalDatabase.animals.Length; i++)
        {
            GameObject ob = Instantiate(itemTemplate, transform.GetChild(0).GetChild(0));
            if (GameBootstrap.Instance.animalDatabase.animals[i].animalUnlockLevel > PlayerPrefs.GetInt("level"))
                ob.transform.GetChild(0).GetComponent<Image>().sprite = GameBootstrap.Instance.animalDatabase.animals[i].lockedShopIcon;
            else
                ob.transform.GetChild(0).GetComponent<Image>().sprite = GameBootstrap.Instance.animalDatabase.animals[i].shopIcon;
            ob.transform.GetChild(0).GetComponent<Image>().SetNativeSize();
            if (Application.systemLanguage == SystemLanguage.Vietnamese)
            {
                ob.transform.GetChild(1).GetComponent<Text>().text = GameBootstrap.Instance.animalDatabase.animals[i].vietnameseName;
                if (PlayerPrefs.GetInt("level") < GameBootstrap.Instance.animalDatabase.animals[i].secondPenUnlockLevel)
                {
                    ob.transform.GetChild(2).gameObject.SetActive(true);
                    if (AnimalPersistence.GetAnimalQuantity(i) < 3)
                        if (PlayerPrefs.GetInt("level") < GameBootstrap.Instance.animalDatabase.animals[i].animalUnlockLevel)
                            ob.transform.GetChild(2).GetComponent<Text>().text = "Mở khóa: lv" + GameBootstrap.Instance.animalDatabase.animals[i].animalUnlockLevel;
                        else
                            ob.transform.GetChild(2).gameObject.SetActive(false);
                    else
                    if (AnimalPersistence.GetAnimalQuantity(i) == 3)
                        ob.transform.GetChild(2).GetComponent<Text>().text = "Mở khóa: lv" + GameBootstrap.Instance.animalDatabase.animals[i].secondPenUnlockLevel;
                }
                else
                    ob.transform.GetChild(2).gameObject.SetActive(false);
            }
            else
            {
                ob.transform.GetChild(1).GetComponent<Text>().text = GameBootstrap.Instance.animalDatabase.animals[i].englishName;
                if (PlayerPrefs.GetInt("level") < GameBootstrap.Instance.animalDatabase.animals[i].secondPenUnlockLevel)
                {
                    ob.transform.GetChild(2).gameObject.SetActive(true);
                    if (AnimalPersistence.GetAnimalQuantity(i) < 3)
                        if (PlayerPrefs.GetInt("level") < GameBootstrap.Instance.animalDatabase.animals[i].animalUnlockLevel)
                            ob.transform.GetChild(2).GetComponent<Text>().text = "Unlock: lv" + GameBootstrap.Instance.animalDatabase.animals[i].animalUnlockLevel;
                        else
                            ob.transform.GetChild(2).gameObject.SetActive(false);
                    else
                    if (AnimalPersistence.GetAnimalQuantity(i) == 3)
                        ob.transform.GetChild(2).GetComponent<Text>().text = "Unlock: lv" + GameBootstrap.Instance.animalDatabase.animals[i].secondPenUnlockLevel;
                }
                else
                    ob.transform.GetChild(2).gameObject.SetActive(false);
            }
            ob.transform.GetChild(3).GetComponent<Text>().text = AnimalPersistence.GetAnimalQuantity(i) + "/" + AnimalPersistence.GetAnimalLimit(i);
            ob.transform.GetChild(4).GetComponent<Text>().text = AnimalPersistence.GetAnimalPrice(i).ToString();
            ob.transform.GetChild(0).GetComponent<AnimalShopItem>().animalTypeId = i;
            ob.name = GameBootstrap.Instance.animalDatabase.animals[i].vietnameseName;
        }
        Destroy(itemTemplate);
    }
    public void RefreshQuantity(int i)
    {
        transform.GetChild(0).GetChild(0).GetChild(i).GetChild(3).GetComponent<Text>().text = AnimalPersistence.GetAnimalQuantity(i) + "/" + AnimalPersistence.GetAnimalLimit(i);
        if (Application.systemLanguage == SystemLanguage.Vietnamese)
        {
                if (PlayerPrefs.GetInt("level") < GameBootstrap.Instance.animalDatabase.animals[i].secondPenUnlockLevel)
            {
                transform.GetChild(0).GetChild(0).GetChild(i).GetChild(2).gameObject.SetActive(true);
                if (AnimalPersistence.GetAnimalQuantity(i) < 3)
                    if (PlayerPrefs.GetInt("level") < GameBootstrap.Instance.animalDatabase.animals[i].animalUnlockLevel)
                        transform.GetChild(0).GetChild(0).GetChild(i).GetChild(2).GetComponent<Text>().text = "Mở khóa: lv" + GameBootstrap.Instance.animalDatabase.animals[i].animalUnlockLevel;
                    else
                        transform.GetChild(0).GetChild(0).GetChild(i).GetChild(2).gameObject.SetActive(false);
                else
                if (AnimalPersistence.GetAnimalQuantity(i) == 3)
                    transform.GetChild(0).GetChild(0).GetChild(i).GetChild(2).GetComponent<Text>().text = "Mở khóa: lv" + GameBootstrap.Instance.animalDatabase.animals[i].secondPenUnlockLevel;
            }
            else
                transform.GetChild(0).GetChild(0).GetChild(i).GetChild(2).gameObject.SetActive(false);

        }
        else
        {

            if (PlayerPrefs.GetInt("level") < GameBootstrap.Instance.animalDatabase.animals[i].secondPenUnlockLevel)
            {
                transform.GetChild(0).GetChild(0).GetChild(i).GetChild(2).gameObject.SetActive(true);
                if (AnimalPersistence.GetAnimalQuantity(i) < 3)
                    if (PlayerPrefs.GetInt("level") < GameBootstrap.Instance.animalDatabase.animals[i].animalUnlockLevel)
                        transform.GetChild(0).GetChild(0).GetChild(i).GetChild(2).GetComponent<Text>().text = "Unlock: lv" + GameBootstrap.Instance.animalDatabase.animals[i].animalUnlockLevel;
                    else
                        transform.GetChild(0).GetChild(0).GetChild(i).GetChild(2).gameObject.SetActive(false);
                else
                if (AnimalPersistence.GetAnimalQuantity(i) == 3)
                    transform.GetChild(0).GetChild(0).GetChild(i).GetChild(2).GetComponent<Text>().text = "Unlock: lv" + GameBootstrap.Instance.animalDatabase.animals[i].secondPenUnlockLevel;
            }
            else
                transform.GetChild(0).GetChild(0).GetChild(i).GetChild(2).gameObject.SetActive(false);
        }
    }
    public void checkIcon(int i)
    {
        if (GameBootstrap.Instance.animalDatabase.animals[i].animalUnlockLevel > PlayerPrefs.GetInt("level"))
            transform.GetChild(0).GetChild(0).GetChild(i).GetChild(0).GetComponent<Image>().sprite = GameBootstrap.Instance.animalDatabase.animals[i].lockedShopIcon;
        else
        {
            transform.GetChild(0).GetChild(0).GetChild(i).GetChild(0).GetComponent<Image>().sprite = GameBootstrap.Instance.animalDatabase.animals[i].shopIcon;
            transform.GetChild(0).GetChild(0).GetChild(i).GetChild(2).gameObject.SetActive(false);
        }
    }
    public void RefreshPriceAvailability(int i)
    {
        transform.GetChild(0).GetChild(0).GetChild(i).GetChild(4).GetComponent<Text>().text = AnimalPersistence.GetAnimalPrice(i).ToString();
    }
}
