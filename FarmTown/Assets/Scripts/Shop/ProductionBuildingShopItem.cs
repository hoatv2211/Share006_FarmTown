using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using FarmTown.Save;
public class ProductionBuildingShopItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler,IPointerDownHandler
{
    [SerializeField] ScrollRect MainScroll;
    public int id;
    GameObject obkeo;
    int draggedInstanceIndex;
    bool checkkeo;
    Vector2 pos1, pos2;
    public void OnBeginDrag(PointerEventData eventData)
    {
        pos2 = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        MainScroll.OnBeginDrag(eventData);
        if (pos2.y - pos1.y > 0.01f)
        {
            if (GameBootstrap.Instance.buildingDatabase.buildings[id].unlockLevel <= PlayerPrefs.GetInt("level"))
            {
                if (ProductionPersistence.GetBuildingCount(id) < PlayerPrefs.GetInt("slnhamaymax" + id))
                {
                    if (PlayerPrefs.GetInt("gold") >= PlayerPrefs.GetInt("gianhamay" + id))
                    {
                        Vector2 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        obkeo = Instantiate(GameBootstrap.Instance.buildingDatabase.buildings[id].prefab, target, Quaternion.identity);
                        draggedInstanceIndex = ProductionPersistence.GetBuildingCount(id);
                        obkeo.name = ProductionPersistence.DefaultLegacyName(id, draggedInstanceIndex);
                        var previewBuilding = obkeo.GetComponent<ProductionBuilding>();
                        previewBuilding.InitializeIdentity(id, draggedInstanceIndex);
                        previewBuilding.enabled = false;
                        checkkeo = true;
                        obkeo.GetComponent<PolygonCollider2D>().isTrigger = true;
                        obkeo.AddComponent<Rigidbody2D>();
                        obkeo.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
                        obkeo.AddComponent<PlacementCollisionChecker>();
                        if (!PlayerPrefs.HasKey("huongdan"))
                            DialogShop.instance.close();
                        else
                            DialogShop.instance.dialogShop.GetComponent<Animator>().SetInteger("dialog", 1);
                        MainCamera.Instance.lockCam();
                    }
                    else
                    {
                        NotificationController.Instance.ShowCanvasNotification("Không đủ vàng!!", "Not enough gold!!");
                    }
                }
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        MainScroll.OnDrag(eventData);
        if (checkkeo == true)
        {
            Vector2 cursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 posCam = new Vector2(cursorPosition.x, cursorPosition.y);
            Vector3 target = new Vector3(posCam.x, posCam.y, 0);
            target = new Vector3(((int)(posCam.x / 0.62f)) * 0.62f, ((int)(posCam.y / 0.32f)) * 0.32f, 0);
            obkeo.transform.position = target;
        }
    }



    public void OnEndDrag(PointerEventData eventData)
    {
        MainScroll.OnEndDrag(eventData);
        if (checkkeo == true)
        {
            if (obkeo.GetComponent<PlacementCollisionChecker>().hasCollision == true)
            {
                NotificationController.Instance.ShowCanvasNotification("Không thể đặt ở đây!", "Can't put here");
                Destroy(obkeo);
            }
            else
            {
                var instanceIndex = draggedInstanceIndex;
                var stableId = ProductionPersistence.DefaultStableId(id, instanceIndex);
                var legacyName = ProductionPersistence.DefaultLegacyName(id, instanceIndex);
                obkeo.name = legacyName;
                ProductionPersistence.SetPosition(stableId, legacyName, obkeo.transform.position);
                var building = obkeo.GetComponent<ProductionBuilding>();
                if (building.State.QueueCapacity == 0)
                    building.State.SetQueueCapacity(3);
                building.enabled = true;
                Destroy(obkeo.GetComponent<Rigidbody2D>());
                Destroy(obkeo.GetComponent<PlacementCollisionChecker>());
                obkeo.GetComponent<PolygonCollider2D>().isTrigger = false;
                ProductionPersistence.SetBuildingCount(id, instanceIndex + 1);
                ProductionBuildingShopList.instance.RefreshQuantity(id);
                Vector2 target = obkeo.transform.position;
                obkeo.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt((13 - target.y) * 100f);
                GameBootstrap.Instance.AddGold(-PlayerPrefs.GetInt("gianhamay" + id));
                PlayerPrefs.SetInt("gianhamay" + id, PlayerPrefs.GetInt("gianhamay" + id) + 500);
                ProductionBuildingShopList.instance.RefreshPrice(id);
                if (PlayerPrefs.GetInt("huongdan") == 15)
                {
                    TutorialController.instance.shopDragHand.SetActive(false);
                    TutorialController.instance.pointerHand.SetActive(true);
                    TutorialController.instance.MovePointerHand(obkeo.transform.position);
                    PlayerPrefs.SetInt("huongdan", 16);
                    TutorialController.instance.ShowWorldMessage("Nhấn để bắt đầu sản xuất", "Click to start production");
                }
            }
            DialogShop.instance.open();
            checkkeo = false;
            MainCamera.Instance.unLockCam();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        pos1 = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
}
