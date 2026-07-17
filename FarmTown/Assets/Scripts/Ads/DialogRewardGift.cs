using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DialogRewardGift : MonoBehaviour
{

    [SerializeField] GameObject icon;
    public static DialogRewardGift instance;
    int ma;
    public Text tb;
    // 0: experience, 1: gold, 2: experience.
    private void Start()
    {
        instance = this;
        if (System.DateTime.Now.Day != PlayerPrefs.GetInt("ngay"))
        {
            PlayerPrefs.SetInt("ngay", System.DateTime.Now.Day);
            for (int i = 0; i <= 3; i++)
            {
                PlayerPrefs.SetInt("xem" + i, 5);
            }
        }
    }
    public void Open()
    {
        if (!PlayerPrefs.HasKey("huongdan"))
        {
            AudioManager.Instance.click();
            MainCamera.Instance.lockCam();
            gameObject.transform.localScale = Vector3.one;
            gameObject.GetComponent<Animator>().enabled = true;
            gameObject.GetComponent<Animator>().Play("dialog", -1, 0);
        }
    }
    public void Close()
    {
        AudioManager.Instance.click();
        MainCamera.Instance.unLockCam();
        gameObject.transform.localScale = Vector3.zero;
    }
    public void WatchRewardAd(int id)
    {
        AudioManager.Instance.click();
        ma = id;
        if (PlayerPrefs.GetInt("xem" + id) > 0)
        {
            if (MobileRewardAd.instance.rewardBasedVideoAd.CanShowAd())
            {
                // ShowDialog();
                MobileRewardAd.instance.ShowRewardedAd();
            }
            else
            {
                NotificationController.Instance.ShowCanvasNotification("Mời bạn thử lại lần khác, chúc bạn may mắn!", "Please try again later, good luck!");
            }
        }
        else
        {
      
            NotificationController.Instance.ShowCanvasNotification("Lượt xem trong ngày của bạn đã hết!", "Your day view has expired!");
        }

    }
    public void ShowDialog()
    {
        MainCamera.Instance.lockCam();
        transform.GetChild(1).gameObject.SetActive(false);
        transform.GetChild(2).gameObject.SetActive(true);
        if (Application.systemLanguage == SystemLanguage.Vietnamese)
        {
            if (ma == 0)
            {
                tb.text = "Chúc mừng bạn nhận được 100 vàng";
            }
            if (ma == 1)
            {
                tb.text = "Chúc mừng bạn nhận được 10 kim cương";
            }
            if (ma == 2)
            {
                tb.text = "Chúc mừng bạn nhận được 10 kinh nghiệm";
            }
            
        }
        else
        {
            if (ma == 0)
            {
                tb.text = "Congratulations on receiving 100 gold";
            }
            if (ma == 1)
            {
                tb.text = "Congratulations on receiving 10 diamond";
            }
            if (ma == 2)
            {
                tb.text = "Congratulations on receiving 10 experience";
            }
            
        }
    }
   
    public void ClaimReward()
    {
        Vector2 pos = Camera.main.transform.position;
        MainCamera.Instance.unLockCam();
        PlayerPrefs.SetInt("xem" + ma, PlayerPrefs.GetInt("xem" + ma) - 1);
        GameObject ob = Instantiate(icon, pos, Quaternion.identity);
        if (ma == 0)
        {
            ob.GetComponent<RewardFlyout>().id = 3;
            ob.GetComponent<RewardFlyout>().numberCoin = 100;
            GameBootstrap.Instance.AddGold(100);
        }
        if (ma == 1)
        {
            ob.GetComponent<RewardFlyout>().id = 4;
            ob.GetComponent<RewardFlyout>().numberCoin = 10;
            GameBootstrap.Instance.AddDiamonds(10);
        }
        if (ma == 2)
        {
            ob.GetComponent<RewardFlyout>().id = 2;
            ob.GetComponent<RewardFlyout>().numberCoin = 10;
          
        }
        transform.GetChild(2).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(true);
        gameObject.transform.localScale = Vector3.zero;
        AudioManager.Instance.Harvest();
    }

}
