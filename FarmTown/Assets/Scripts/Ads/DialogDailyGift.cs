using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;
public class DialogDailyGift : MonoBehaviour
{
    [FormerlySerializedAs("day"), SerializeField] private GameObject[] dailyRewardCards;
    bool check;
    [FormerlySerializedAs("iconTien"), SerializeField] private GameObject rewardFlyoutPrefab;
    private void Start()
    {
        string homnay = System.DateTime.Now.DayOfWeek.ToString();
        if (!PlayerPrefs.HasKey("ngaydaily"))
        {

            PlayerPrefs.SetString("ngaydaily", System.DateTime.Now.DayOfWeek.ToString());
        }
        else
        {
            if (PlayerPrefs.GetString("ngaydaily") == homnay)
            {
                if (!PlayerPrefs.HasKey("nhan" + homnay))
                {

                }
                else
                {
                    Close();
                }
            }
            else
            {
                PlayerPrefs.DeleteKey("ngaydaily" + homnay);
            }
        }
        if (homnay == "Monday")
        {
            dailyRewardCards[0].transform.GetChild(1).GetComponent<Animator>().enabled = true;

        }
        if (homnay == "Tuesday")
        {
            dailyRewardCards[1].transform.GetChild(1).GetComponent<Animator>().enabled = true;

        }
        if (homnay == "Wednesday")
        {
            dailyRewardCards[2].transform.GetChild(1).GetComponent<Animator>().enabled = true;

        }
        if (homnay == "Thursday")
        {
            dailyRewardCards[3].transform.GetChild(1).GetComponent<Animator>().enabled = true;

        }
        if (homnay == "Friday")
        {
            dailyRewardCards[4].transform.GetChild(1).GetComponent<Animator>().enabled = true;

        }
        if (homnay == "Saturday")
        {
            dailyRewardCards[5].transform.GetChild(1).GetComponent<Animator>().enabled = true;

        }
        if (homnay == "Sunday")
        {
            dailyRewardCards[6].transform.GetChild(1).GetComponent<Animator>().enabled = true;

        }
    }

    public void Close()
    {
        AudioManager.Instance.click();
        MainCamera.Instance.unLockCam();
        gameObject.SetActive(false);
        if (PlayerPrefs.HasKey("huongdan"))
        {
            if (PlayerPrefs.GetInt("huongdan") == 1)
                TutorialController.instance.ShowWorldMessage("Bạn hãy nhấn vào ô đất để bắt đầu thu hoạch", "Click on the land to start harvest");
        }

    }
    public void Claim(string dayName)
    {
        if (System.DateTime.Now.DayOfWeek.ToString() == dayName)
        {
            if (!PlayerPrefs.HasKey("nhan" + dayName))
            {
                PlayerPrefs.SetString("ngaydaily", System.DateTime.Now.DayOfWeek.ToString());
                PlayerPrefs.SetInt("nhan" + dayName, 1);
                GameObject ob = Instantiate(rewardFlyoutPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                if (dayName == "Monday")
                {
                    dailyRewardCards[0].transform.GetChild(1).GetComponent<Animator>().enabled = true;
                    ob.GetComponent<RewardFlyout>().id = 3;
                    ob.GetComponent<RewardFlyout>().numberCoin = 50;
                    GameBootstrap.Instance.AddGold(50);
                }
                if (dayName == "Tuesday")
                {
                    dailyRewardCards[1].transform.GetChild(1).GetComponent<Animator>().enabled = true;
                    ob.GetComponent<RewardFlyout>().id = 4;
                    ob.GetComponent<RewardFlyout>().numberCoin = 10;
                    GameBootstrap.Instance.AddDiamonds(10);
                }
                if (dayName == "Wednesday")
                {
                    dailyRewardCards[2].transform.GetChild(1).GetComponent<Animator>().enabled = true;
                    ob.GetComponent<RewardFlyout>().id = 3;
                    ob.GetComponent<RewardFlyout>().numberCoin = 150;
                    GameBootstrap.Instance.AddGold(100);
                }
                if (dayName == "Thursday")
                {
                    dailyRewardCards[3].transform.GetChild(1).GetComponent<Animator>().enabled = true;
                    ob.GetComponent<RewardFlyout>().id = 4;
                    ob.GetComponent<RewardFlyout>().numberCoin = 30;
                    GameBootstrap.Instance.AddDiamonds(15);
                }
                if (dayName == "Friday")
                {
                    dailyRewardCards[4].transform.GetChild(1).GetComponent<Animator>().enabled = true;
                    ob.GetComponent<RewardFlyout>().id = 3;
                    ob.GetComponent<RewardFlyout>().numberCoin = 300;
                    GameBootstrap.Instance.AddGold(200);
                }
                if (dayName == "Saturday")
                {
                    dailyRewardCards[5].transform.GetChild(1).GetComponent<Animator>().enabled = true;
                    ob.GetComponent<RewardFlyout>().id = 4;
                    ob.GetComponent<RewardFlyout>().numberCoin = 50;
                    GameBootstrap.Instance.AddDiamonds(20);
                }
                if (dayName == "Sunday")
                {
                    dailyRewardCards[6].transform.GetChild(1).GetComponent<Animator>().enabled = true;
                    ob.GetComponent<RewardFlyout>().id = 3;
                    ob.GetComponent<RewardFlyout>().numberCoin = 500;
                    GameBootstrap.Instance.AddGold(500);
                }
                Close();
            }

        }
        else
        {
            NotificationController.Instance.ShowCanvasNotification("Bạn không thể nhận phần quà này hôm nay!", "You can't receive this gift today!");
        }
    }

}
