using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
public class DecorationItemShopItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler,IPointerDownHandler
{
    [FormerlySerializedAs("MainScroll")] [SerializeField] ScrollRect mainScroll;
    [FormerlySerializedAs("id")] public int decorationId;
    GameObject draggedItem;
    Vector2 pointerDownPosition, currentPointerPosition;
    bool isDragging;
    [FormerlySerializedAs("unlock")] public GameObject lockedOverlay;
    [FormerlySerializedAs("iconBay")] [SerializeField] GameObject rewardFlyoutPrefab;
    public void RefreshUnlockState()
    {
        if (GameBootstrap.Instance.decorationItemDatabase.decorationItems[decorationId].unlockLevel <= PlayerPrefs.GetInt("level"))
        {
            if (PlayerPrefs.GetInt("gold") >= GameBootstrap.Instance.decorationItemDatabase.decorationItems[decorationId].purchasePrice)
            {
                transform.GetComponent<Image>().sprite = GameBootstrap.Instance.decorationItemDatabase.decorationItems[decorationId].shopIcon;
                lockedOverlay.SetActive(false);
            }
            else
            {
                NotificationController.Instance.ShowCanvasNotification("Không đủ vàng!!", "Not enough gold");
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        mainScroll.OnBeginDrag(eventData);
        currentPointerPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (currentPointerPosition.y - pointerDownPosition.y > .01f)
        {
            if (GameBootstrap.Instance.decorationItemDatabase.decorationItems[decorationId].unlockLevel <= PlayerPrefs.GetInt("level"))
            {
                Vector2 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                draggedItem = Instantiate(GameBootstrap.Instance.decorationItemDatabase.decorationItems[decorationId].prefab, target, Quaternion.identity);
                isDragging = true;
                draggedItem.AddComponent<Rigidbody2D>();
                draggedItem.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
                draggedItem.AddComponent<GroundPlacementCollisionChecker>();
                DialogShop.instance.close();
                MainCamera.Instance.lockCam();
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        mainScroll.OnDrag(eventData);
        if (isDragging == true)
        {
            Vector2 cursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 cursorWorldPosition = new Vector2(cursorPosition.x, cursorPosition.y);
            Vector3 target = new Vector3(cursorWorldPosition.x, cursorWorldPosition.y, 0);
            target = new Vector3(((int)(cursorWorldPosition.x / 0.62f)) * 0.62f, ((int)(cursorWorldPosition.y / 0.32f)) * 0.32f, 0);
            draggedItem.transform.position = target;
        }
    }


    public void OnEndDrag(PointerEventData eventData)
    {
        mainScroll.OnEndDrag(eventData);
        if (isDragging == true)
        {
            if (draggedItem.GetComponent<GroundPlacementCollisionChecker>().hasCollision == true)
            {
                NotificationController.Instance.ShowCanvasNotification("Không thể đặt ở đây!", "Can't put here");
                Destroy(draggedItem);
            }
            else
            {
                Destroy(draggedItem.GetComponent<Rigidbody2D>());
                Destroy(draggedItem.GetComponent<GroundPlacementCollisionChecker>());
                draggedItem.GetComponent<PolygonCollider2D>().isTrigger = false;
                draggedItem.name = "trangtri" + decorationId + PlayerPrefs.GetInt("sltrangtri" + decorationId);
                PlayerPrefs.SetFloat("x" + draggedItem.name, draggedItem.transform.position.x);
                PlayerPrefs.SetFloat("y" + draggedItem.name, draggedItem.transform.position.y);
                PlayerPrefs.SetInt("sltrangtri" + decorationId, PlayerPrefs.GetInt("sltrangtri" + decorationId) + 1);
                Vector2 target = draggedItem.transform.position;
                draggedItem.GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt((13 - target.y) * 100f);
                GameBootstrap.Instance.AddGold(-GameBootstrap.Instance.decorationItemDatabase.decorationItems[decorationId].purchasePrice);
                GameObject rewardFlyout = Instantiate(rewardFlyoutPrefab, draggedItem.transform.position, Quaternion.identity);
                rewardFlyout.GetComponent<RewardFlyout>().id = 2;
                rewardFlyout.GetComponent<RewardFlyout>().numberCoin = GameBootstrap.Instance.decorationItemDatabase.decorationItems[decorationId].experienceReward;
            }
            DialogShop.instance.open();
            isDragging = false;
            MainCamera.Instance.unLockCam();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        pointerDownPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
}
