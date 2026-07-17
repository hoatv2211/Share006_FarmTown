using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;

public class AnimalTimerPanel : MonoBehaviour {

    public static AnimalTimerPanel instance;
    [FormerlySerializedAs("txtName"), SerializeField] Text animalNameText;
    [FormerlySerializedAs("txtTime"), SerializeField] Text timeText;
    [FormerlySerializedAs("txtkc"), SerializeField] Text gemCostText;
    [FormerlySerializedAs("imgTime"), SerializeField] Image progressImage;
    [FormerlySerializedAs("btnkc")] public Button useGemsButton;
    [FormerlySerializedAs("sokc")] public int gemCost;
    private void Start()
    {
        instance = this;
    }
    public void ShowTimer(Vector2 position, string animalName, int remainingTime, int totalTime)
    {
        transform.position = position;
        transform.GetChild(0).transform.localScale = Vector3.one;
        //transform.GetChild(0).GetComponent<Animator>().enabled = true;
        //transform.GetChild(0).GetComponent<Animator>().Play("dialogLiem", -1, 0);
        animalNameText.text = animalName;
        timeText.text = (remainingTime / 60).ToString() + ":" + (remainingTime % 60).ToString();
        gemCostText.text = gemCost.ToString();
        progressImage.fillAmount = 1 - (float)remainingTime / totalTime;
        AnimalFeedingPanel.instance.close();
    }
    public void UseDiamonds()
    {
        if (PlayerPrefs.GetInt("diamond") >= gemCost)
        {
            GameBootstrap.Instance.AddDiamonds(-gemCost);
            FarmAnimal.FindRuntimeAnimal(PlayerPrefs.GetString("duongdan")).UseDiamonds();
            Close();
        }
        else
        {
            NotificationController.Instance.ShowCanvasNotification("không đủ kim cương!", "Not enough diamonds!");
            Close();
        }
    }
    public void Close()
    {
        PlayerPrefs.SetString("duongdan", "a");
        transform.GetChild(0).localScale = Vector3.zero;
    }
}
