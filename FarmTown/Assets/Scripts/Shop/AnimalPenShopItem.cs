using System.Collections;
using System.Collections.Generic;
using FarmTown.Save;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Serialization;
public class AnimalPenShopItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler,IPointerDownHandler
{
    private static void AssignAnimalPenIdentity(GameObject pen, int typeId, int instanceIndex)
    {
        var identity = pen.GetComponent<StableInstanceId>() ?? pen.AddComponent<StableInstanceId>();
        if (!identity.TryAssign(AnimalPersistence.DefaultPenStableId(typeId, instanceIndex)))
            throw new System.InvalidOperationException($"Cannot assign stable ID for animal pen {typeId}:{instanceIndex}.");
        pen.GetComponent<AnimalPen>().instanceIndex = instanceIndex;
        pen.name = AnimalPersistence.DefaultLegacyPenName(typeId, instanceIndex);
    }

    private static void AssignCropPlotIdentity(GameObject plot, int index)
    {
        var identity = plot.GetComponent<StableInstanceId>() ?? plot.AddComponent<StableInstanceId>();
        if (!identity.TryAssign(FarmingPlotPersistence.DefaultStableId(index)))
            throw new System.InvalidOperationException($"Cannot assign stable ID for crop plot {index}.");
        plot.name = $"CropPlot-{index}";
    }

    [FormerlySerializedAs("MainScroll"), SerializeField] ScrollRect scrollRect;
    [FormerlySerializedAs("Odat"), SerializeField] GameObject cropPlotPrefab;
    [FormerlySerializedAs("id")] public int itemTypeId;
    GameObject draggedItem;
    bool isDragging;
    Vector2 pointerStart, pointerEnd;
    public void OnBeginDrag(PointerEventData eventData)
    {
        pointerEnd = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        scrollRect.OnBeginDrag(eventData);
        if (pointerEnd.y - pointerStart.y > .005f)
        {
            if (itemTypeId == 7)//o dat
            {
                //if (PlayerPrefs.GetInt("capmoodat") <= PlayerPrefs.GetInt("level"))
                //{
                if (FarmingPlotPersistence.GetPlotCount() < PlayerPrefs.GetInt("soodatmax"))
                {
                    if (PlayerPrefs.GetInt("gold") >= 10)
                    {
                        Vector2 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        draggedItem = Instantiate(cropPlotPrefab, target, Quaternion.identity);
                        AssignCropPlotIdentity(draggedItem, FarmingPlotPersistence.GetPlotCount());
                        isDragging = true;
                        draggedItem.GetComponent<PolygonCollider2D>().isTrigger = true;
                        draggedItem.AddComponent<Rigidbody2D>();
                        draggedItem.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
                        draggedItem.AddComponent<GroundPlacementCollisionChecker>();
                        if (!PlayerPrefs.HasKey("huongdan"))
                            DialogShop.instance.close();
                        else
                            DialogShop.instance.dialogShop.GetComponent<Animator>().SetInteger("dialog", 1);
                        MainCamera.Instance.lockCam();
                    }
                    else
                    {
                        NotificationController.Instance.ShowCanvasNotification("Không đủ vàng!!", "Not enough Gold!!");
                    }
                }
                //}
            }
        else // Animal pen.
            {
                if (GameBootstrap.Instance.animalDatabase.animals[itemTypeId].animalUnlockLevel <= PlayerPrefs.GetInt("level"))
                {
                    if (AnimalPersistence.GetPenQuantity(itemTypeId) < AnimalPersistence.GetPenLimit(itemTypeId))
                    {
                        if (PlayerPrefs.GetInt("gold") >= AnimalPersistence.GetPenPrice(itemTypeId))
                        {
                            Vector2 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                            draggedItem = Instantiate(GameBootstrap.Instance.animalDatabase.animals[itemTypeId].penPrefab, target, Quaternion.identity);
                            isDragging = true;
                            draggedItem.GetComponent<AnimalPen>().animalTypeId = itemTypeId;
                            AssignAnimalPenIdentity(draggedItem, itemTypeId, AnimalPersistence.GetPenQuantity(itemTypeId));
                            draggedItem.GetComponent<PolygonCollider2D>().isTrigger = true;
                            draggedItem.AddComponent<Rigidbody2D>();
                            draggedItem.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
                            draggedItem.AddComponent<PlacementCollisionChecker>();
                            if (!PlayerPrefs.HasKey("huongdan"))
                                DialogShop.instance.close();
                            else
                                DialogShop.instance.dialogShop.GetComponent<Animator>().SetInteger("dialog", 1);
                            MainCamera.Instance.lockCam();
                        }
                        else
                        {
                            NotificationController.Instance.ShowCanvasNotification("Không đủ vàng!!", "Not enough Gold!!");
                        }
                    }
                }
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        scrollRect.OnDrag(eventData);
        if (isDragging == true)
        {
            Vector2 cursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 posCam = new Vector2(cursorPosition.x, cursorPosition.y);
            Vector3 target = new Vector3(posCam.x, posCam.y, 0);
            target = new Vector3(((int)(posCam.x / 0.78f)) * 0.78f, ((int)(posCam.y / 0.4f)) * 0.4f, 0);
            draggedItem.transform.position = target;
        }
    }


    public void OnEndDrag(PointerEventData eventData)
    {
        scrollRect.OnEndDrag(eventData);
        if (isDragging == true)
        {
            if (itemTypeId == 7)
            {
                if (draggedItem.GetComponent<GroundPlacementCollisionChecker>().hasCollision == true)
                {
                    NotificationController.Instance.ShowCanvasNotification("Không thể đặt ở đây!", "Can't put here");
                    Destroy(draggedItem);
                }
                else
                {
                    var plotIndex = FarmingPlotPersistence.GetPlotCount();
                    AssignCropPlotIdentity(draggedItem, plotIndex);
                    PlayerPrefs.SetFloat(SaveKeys.PlotPositionX(FarmingPlotPersistence.DefaultStableId(plotIndex)), draggedItem.transform.position.x);
                    PlayerPrefs.SetFloat(SaveKeys.PlotPositionY(FarmingPlotPersistence.DefaultStableId(plotIndex)), draggedItem.transform.position.y);
                    PlayerPrefs.SetFloat(LegacySaveKeys.PlotPositionX(plotIndex), draggedItem.transform.position.x);
                    PlayerPrefs.SetFloat(LegacySaveKeys.PlotPositionY(plotIndex), draggedItem.transform.position.y);
                    Destroy(draggedItem.GetComponent<Rigidbody2D>());
                    Destroy(draggedItem.GetComponent<GroundPlacementCollisionChecker>());
                    draggedItem.GetComponent<PolygonCollider2D>().isTrigger = false;
                    FarmingPlotPersistence.SetPlotCount(plotIndex + 1);
        // Update crop plot sorting order.
                    Vector2 target = draggedItem.transform.position;
                    draggedItem.GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt((13 - target.y) * 100f);
                    draggedItem.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt((13 - target.y) * 100f) + 1;
                    GameBootstrap.Instance.AddGold(-10);
                    if (PlayerPrefs.HasKey("huongdan"))
                    {
                        PlayerPrefs.SetInt("huongdan", PlayerPrefs.GetInt("huongdan") + 1);
                        if (PlayerPrefs.GetInt("huongdan") == 12)
                        {
                            TutorialController.instance.shopDragHand.SetActive(false);
                            DialogShop.instance.dialogShop.GetComponent<Animator>().SetInteger("dialog", 1);
                            TutorialController.instance.pointerHand.SetActive(true);
                            TutorialController.instance.MovePointerHand(GameBootstrap.Instance.OrderTarget.position);
                            MainCamera.Instance.MoveCamera(GameBootstrap.Instance.OrderTarget.position, true);
                            TutorialController.instance.ShowWorldMessage("Nhấn để vào đơn hàng", "Click to enter the order");
                            PlayerPrefs.SetInt("capmoodat", 4);
                        }
                    }
                    AnimalPenShopList.instance.RefreshQuantity(0);
                }
            }
            else
            {
                if (draggedItem.GetComponent<PlacementCollisionChecker>().hasCollision == true)
                {
                    NotificationController.Instance.ShowCanvasNotification("Không thể đặt ở đây!", "Can't put here");
                    Destroy(draggedItem);
                }
                else
                {
                    var instanceIndex = AnimalPersistence.GetPenQuantity(itemTypeId);
                    AssignAnimalPenIdentity(draggedItem, itemTypeId, instanceIndex);
                    AnimalPersistence.SetPenPosition(
                        AnimalPersistence.DefaultPenStableId(itemTypeId, instanceIndex),
                        AnimalPersistence.DefaultLegacyPenName(itemTypeId, instanceIndex),
                        draggedItem.transform.position);
                    Destroy(draggedItem.GetComponent<Rigidbody2D>());
                    Destroy(draggedItem.GetComponent<PlacementCollisionChecker>());
                    draggedItem.GetComponent<PolygonCollider2D>().isTrigger = false;
                    AnimalPersistence.SetPenQuantity(itemTypeId, instanceIndex + 1);
                    AnimalPenShopList.instance.RefreshQuantity(itemTypeId + 1);
        // Update animal pen sorting order.
                    Vector2 target = draggedItem.transform.position;
                    draggedItem.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt((13 - target.y) * 100f);
                    draggedItem.transform.GetChild(1).GetComponent<MeshRenderer>().sortingOrder = Mathf.RoundToInt((13 - target.y) * 100f) + 1;
                    draggedItem.transform.GetChild(2).GetComponent<MeshRenderer>().sortingOrder = Mathf.RoundToInt((13 - target.y) * 100f) + 1;
                    draggedItem.transform.GetChild(3).GetComponent<MeshRenderer>().sortingOrder = Mathf.RoundToInt((13 - target.y) * 100f) + 1;
                    if (itemTypeId != 0)
                        draggedItem.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt((13 - target.y) * 100f) + 2;
                    GameBootstrap.Instance.AddGold(-AnimalPersistence.GetPenPrice(itemTypeId));
                    AnimalPersistence.SetPenPrice(itemTypeId, AnimalPersistence.GetPenPrice(itemTypeId) + 500);
                    AnimalPenShopList.instance.RefreshPriceAvailability(itemTypeId);
                    if (PlayerPrefs.GetInt("huongdan") == 17)
                    {
                        TutorialController.instance.penDragHand.SetActive(false);
                        TutorialController.instance.animalIconPointerHand.SetActive(true);
                        PlayerPrefs.SetInt("huongdan", 18);
                        TutorialController.instance.ShowCanvasMessage("Nhấn vào icon vật nuôi", "Click on the pet icon");
                    }
                }
            }
            DialogShop.instance.open();
            isDragging = false;
            MainCamera.Instance.unLockCam();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        pointerStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
}
