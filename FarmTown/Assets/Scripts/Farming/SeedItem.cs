using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
public class SeedItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler
{
    [FormerlySerializedAs("MainScroll"), SerializeField] ScrollRect scrollRect;
    [FormerlySerializedAs("id")] public int cropId;
    [FormerlySerializedAs("obSinh")] public GameObject seedDragPrefab;
    private GameObject draggedSeed;
    [FormerlySerializedAs("dialogHetHG"), SerializeField] GameObject outOfSeedsPanel;
    bool isDragging;
    Vector2 pointerStart, pointerEnd;
    public void OnBeginDrag(PointerEventData eventData)
    {
        scrollRect.OnBeginDrag(eventData);
        pointerEnd = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (pointerEnd.y - pointerStart.y > .01f)
        {
            if (PlayerPrefs.GetInt("level") >= GameBootstrap.Instance.cropDatabase.crops[cropId].unlockLevel)
            {
                isDragging = true;
                Vector2 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                draggedSeed = Instantiate(seedDragPrefab, target, Quaternion.identity);
                draggedSeed.GetComponent<SeedDragHandler>().cropId = cropId;
                draggedSeed.GetComponent<SpriteRenderer>().sprite = GameBootstrap.Instance.cropDatabase.crops[cropId].productIcon;
                MainCamera.Instance.lockCam();
                pointerStart = pointerEnd;
                SeedSelectionPanel.instance.close();
                if (PlayerPrefs.GetInt("huongdan") == 8)
                {
                    TutorialController.instance.uiDragHand.SetActive(false);

                }
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        scrollRect.OnDrag(eventData);
        if (isDragging)
        {
            Vector2 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            draggedSeed.transform.position = target;
        }

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        scrollRect.OnEndDrag(eventData);
        if (isDragging)
        {
            isDragging = false;
            Destroy(draggedSeed);
            MainCamera.Instance.unLockCam();
            if (PlayerPrefs.HasKey("sohg"))
            {
                MainCamera.Instance.lockCam();
                outOfSeedsPanel.SetActive(true);
                outOfSeedsPanel.GetComponent<OutOfSeedsPanel>().seedIcon.sprite = GameBootstrap.Instance.cropDatabase.crops[cropId].productIcon;
                outOfSeedsPanel.GetComponent<OutOfSeedsPanel>().seedCountText.text = PlayerPrefs.GetInt("sohg").ToString();
                if (Application.systemLanguage != SystemLanguage.Vietnamese)
                    outOfSeedsPanel.GetComponent<OutOfSeedsPanel>().gemCostText.text = "Buy for " + PlayerPrefs.GetInt("sohg").ToString();
                else
                    outOfSeedsPanel.GetComponent<OutOfSeedsPanel>().gemCostText.text = "Mua " + PlayerPrefs.GetInt("sohg").ToString();
                outOfSeedsPanel.GetComponent<OutOfSeedsPanel>().cropId = cropId;
            }
            SeedSelectionPanel.instance.checksl();
            if (PlayerPrefs.GetInt("huongdan") == 8)
            {
                TutorialController.instance.ShowWorldMessage("Nhấn vào icon shop", "Click to icon store");
                PlayerPrefs.SetInt("huongdan", 9);
                TutorialController.instance.shopPointerHand.SetActive(true);
                TutorialController.instance.shopHighlight.SetActive(true);
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        pointerStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
}
