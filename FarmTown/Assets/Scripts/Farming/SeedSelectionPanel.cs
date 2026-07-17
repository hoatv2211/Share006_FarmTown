using FarmTown.Save;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
public class SeedSelectionPanel : MonoBehaviour
{

    public static SeedSelectionPanel instance;
    [FormerlySerializedAs("hg")] public GameObject seedItemTemplate;
    [FormerlySerializedAs("content")] public GameObject seedListContent;
    private void Start()
    {
        instance = this;
        for (int i = 0; i < 22; i++)
        {
            GameObject obj = Instantiate(seedItemTemplate, transform.GetChild(0).GetChild(0).GetChild(0));
            obj.GetComponent<Image>().sprite = GameBootstrap.Instance.cropDatabase.crops[i].productIcon;
            obj.GetComponent<SeedItem>().cropId = i;
            obj.name = GameBootstrap.Instance.cropDatabase.crops[i].vietnameseName;
            obj.transform.GetChild(0).GetComponent<Text>().text = FarmingPlotPersistence.GetCropQuantity(i).ToString();
            if (PlayerPrefs.GetInt("level") >= GameBootstrap.Instance.cropDatabase.crops[i].unlockLevel)
                obj.transform.GetChild(1).gameObject.SetActive(false);
        }
        Destroy(seedItemTemplate);
    }
    public void checksl()
    {
        for (int i = 0; i < 22; i++)
        {
            seedListContent.transform.GetChild(i).GetChild(0).GetComponent<Text>().text = FarmingPlotPersistence.GetCropQuantity(i).ToString();
            if (PlayerPrefs.GetInt("level") >= GameBootstrap.Instance.cropDatabase.crops[i].unlockLevel)
                seedListContent.transform.GetChild(i).GetChild(1).gameObject.SetActive(false);
        }
    }
    public void open()
    {
        checksl();
        if (gameObject.GetComponent<Animator>().enabled == false)
            gameObject.GetComponent<Animator>().enabled = true;
        gameObject.GetComponent<Animator>().SetInteger(AnimatorParameters.SeedSelectionState, 1);
        DialogShop.instance.close();
        //MainCamera.Instance.lockCam();
    }
    public void close()
    {
        if (!PlayerPrefs.HasKey("huongdan"))
        {
            gameObject.GetComponent<Animator>().SetInteger(AnimatorParameters.SeedSelectionState, 2);
        }
        else
        {
                gameObject.GetComponent<Animator>().SetInteger(AnimatorParameters.SeedSelectionState, 2);
        }

        //MainCamera.Instance.unLockCam();
    }

  

}
