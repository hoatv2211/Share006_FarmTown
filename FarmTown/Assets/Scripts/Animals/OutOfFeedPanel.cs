using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;
public class OutOfFeedPanel : MonoBehaviour
{

    [FormerlySerializedAs("slhg")] public Text targetCountText;
    [FormerlySerializedAs("slkimcuong")] public Text gemCostText;
    [FormerlySerializedAs("iconhg")] public Image feedIcon;
    [FormerlySerializedAs("id")] public int feedCropId;
    public void BuyDiamonds()
    {
        if (PlayerPrefs.GetInt("hetthucan") <= PlayerPrefs.GetInt("diamond"))
        {
            GameBootstrap.Instance.AddDiamonds(-PlayerPrefs.GetInt("hetthucan"));
            for (int i = 0; i < PlayerPrefs.GetInt("hetthucan"); i++)
            {
                FarmAnimal.FindRuntimeAnimal(PlayerPrefs.GetString("duongdanvatnuoi" + i)).StartFeeding();
            }
            Close();
        }
        else
        {
            NotificationController.Instance.ShowCanvasNotification("Không đủ kim cương!", "Not enough diamonds!");
            //khong du kim cuong
            Close();
        }
    }
    public void Close()
    {
        for (int i = 0; i < PlayerPrefs.GetInt("hetthucan"); i++)
        {
            FarmAnimal.FindRuntimeAnimal(PlayerPrefs.GetString("duongdanvatnuoi" + i)).HideStatusIcon();
        }
        PlayerPrefs.DeleteKey("hetthucan");
        gameObject.SetActive(false);
        MainCamera.Instance.unLockCam();
    }
}
