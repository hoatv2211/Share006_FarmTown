using FarmTown.Save;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
public class AnimalFeedingPanel : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    public static AnimalFeedingPanel instance;
    private Image feedIcon;
    private int feedCropId;
    private Text quantityText;
    [FormerlySerializedAs("obsinh")] public GameObject feedDragPrefab;
    [FormerlySerializedAs("dialogHetTa")] public GameObject outOfFeedPanel;
    private GameObject draggedFeed;

    private void Start()
    {
        instance = this;
        feedIcon = gameObject.GetComponent<Image>();
        quantityText = transform.GetChild(0).GetComponent<Text>();
    }
    public void open(Vector2 pos, int id)
    {
        // transform.parent.localScale = Vector3.one;
        transform.parent.parent.position = pos;
        transform.parent.GetComponent<Animator>().enabled = true;
        transform.parent.GetComponent<Animator>().Play("dialogLiem", -1, 0);
        feedCropId = id;
        feedIcon.sprite = GameBootstrap.Instance.cropDatabase.crops[feedCropId].productIcon;
        quantityText.text = "x" + FarmingPlotPersistence.GetCropQuantity(feedCropId);
        feedIcon.SetNativeSize();
            AnimalTimerPanel.instance.Close();
    }
    public void close()
    {
        transform.parent.GetComponent<Animator>().enabled = false;
        transform.parent.localScale = Vector3.zero;
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        Vector2 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        draggedFeed = Instantiate(feedDragPrefab, target, Quaternion.identity);
        draggedFeed.GetComponent<SpriteRenderer>().sprite = GameBootstrap.Instance.cropDatabase.crops[feedCropId].productIcon;
        draggedFeed.GetComponent<FeedDragHandler>().feedCropId = feedCropId;
        close();
        MainCamera.Instance.lockCam();
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        draggedFeed.transform.position = target;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (PlayerPrefs.HasKey("huongdan"))
        {
            TutorialController.instance.animalFeedDragHand.SetActive(false);
            PlayerPrefs.DeleteKey("huongdan");
            TutorialController.instance.ShowCameraMessage("Hướng dẫn hoàn thành!!!", "Guide completed!!!");
        }      
        MainCamera.Instance.unLockCam();
        if (PlayerPrefs.HasKey("hetthucan"))
        {
            MainCamera.Instance.lockCam();
            outOfFeedPanel.SetActive(true);
            outOfFeedPanel.GetComponent<OutOfFeedPanel>().feedIcon.sprite = GameBootstrap.Instance.cropDatabase.crops[feedCropId].productIcon;
            outOfFeedPanel.GetComponent<OutOfFeedPanel>().targetCountText.text = PlayerPrefs.GetInt("hetthucan").ToString();
            if (Application.systemLanguage != SystemLanguage.Vietnamese)
                outOfFeedPanel.GetComponent<OutOfFeedPanel>().gemCostText.text = "Buy for " + PlayerPrefs.GetInt("hetthucan").ToString();
            else
                outOfFeedPanel.GetComponent<OutOfFeedPanel>().gemCostText.text = "Mua " + PlayerPrefs.GetInt("hetthucan").ToString();
            outOfFeedPanel.GetComponent<OutOfFeedPanel>().feedCropId = feedCropId;
        }
        Destroy(draggedFeed);
    }
}
