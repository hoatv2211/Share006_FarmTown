using System.Collections;
using System.Collections.Generic;
using FarmTown.Save;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
//电子邮件puhalskijsemen@gmail.com
//源码网站 开vpn全局模式打开 http://web3incubators.com/
//电报https://t.me/gamecode999
//网页客服 http://web3incubators.com/kefu.html
public class CropTimerPanel : MonoBehaviour
{
    public static CropTimerPanel instance;
    [FormerlySerializedAs("txtName"), SerializeField] Text cropNameText;
    [FormerlySerializedAs("txtTime"), SerializeField] Text timeText;
    [FormerlySerializedAs("txtkc"), SerializeField] Text gemCostText;
    [FormerlySerializedAs("imgTime"), SerializeField] Image progressImage;
    [FormerlySerializedAs("btnkc")] public Button useGemsButton;
    [FormerlySerializedAs("sokc")] public int gemCost;
    private void Start()
    {
        instance = this;
    }
    public void ShowTimer(Vector2 pos, string name, int time, int totalSeconds)
    {
        transform.position = pos;
        transform.GetChild(0).transform.localScale = Vector3.one;
        //transform.GetChild(0).GetComponent<Animator>().enabled = true;
        // Timer panel animation remains disabled.
        cropNameText.text = name;
        timeText.text = (time / 60).ToString() + ":" + (time % 60).ToString();
        gemCostText.text = gemCost.ToString();
        progressImage.fillAmount = 1 - (float)time / totalSeconds;
    }
    public void UseDiamonds()
    {
        if (PlayerPrefs.GetInt("diamond") >= gemCost)
        {
            GameBootstrap.Instance.AddDiamonds(-gemCost);
        if (TryGetSelectedPlot(out var selectedPlot)) selectedPlot.FinishGrowthWithDiamonds();
        Close();
        }
        else
        {
            if (TryGetSelectedPlot(out var selectedPlot)) NotificationController.Instance.ShowWorldNotification("Không đủ kim cương!", "Not enough diamond!", selectedPlot.transform.position);
        Close();
        }
    }
    private static bool TryGetSelectedPlot(out CropPlot plot)
    {
        plot = null;
        var stableId = PlayerPrefs.GetString(SaveKeys.SelectedPlot);
        if (stableId.Length == 0 && FarmingPlotPersistence.TryGetStableId(PlayerPrefs.GetString(LegacySaveKeys.SelectedPlot), out var migratedId)) stableId = migratedId;
        if (!StableInstanceId.TryFind(stableId, out var identity)) return false;
        plot = identity.GetComponent<CropPlot>();
        return plot != null;
    }
    public void Close()
    {
        PlayerPrefs.SetString(SaveKeys.SelectedPlot, string.Empty);
        PlayerPrefs.SetString(LegacySaveKeys.SelectedPlot, "a");
        transform.GetChild(0).localScale = Vector3.zero;
    }
}
