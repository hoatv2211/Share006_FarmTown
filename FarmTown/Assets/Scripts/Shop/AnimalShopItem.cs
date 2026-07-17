using System.Collections;
using System.Collections.Generic;
using FarmTown.Save;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Serialization;
public class AnimalShopItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler,IPointerDownHandler
{
    [FormerlySerializedAs("MainScroll"), SerializeField] ScrollRect scrollRect;
    [FormerlySerializedAs("vatnuoi"), SerializeField] GameObject animalDragPrefab;
    [FormerlySerializedAs("id")] public int animalTypeId;
    GameObject draggedAnimal;
    bool isDragging;
    Vector2 pointerStart, pointerEnd;
    public void OnBeginDrag(PointerEventData eventData)
    {
        pointerEnd = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        scrollRect.OnBeginDrag(eventData);
        if (pointerEnd.y - pointerStart.y > .01f)
        {
            if (GameBootstrap.Instance.animalDatabase.animals[animalTypeId].animalUnlockLevel <= PlayerPrefs.GetInt("level"))
            {
                if (AnimalPersistence.GetAnimalQuantity(animalTypeId) < AnimalPersistence.GetAnimalLimit(animalTypeId))
                {
                    if (PlayerPrefs.GetInt("gold") >= AnimalPersistence.GetAnimalPrice(animalTypeId))
                    {
                        Vector2 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        draggedAnimal = Instantiate(animalDragPrefab, target, Quaternion.identity);
                        draggedAnimal.GetComponent<SpriteRenderer>().sprite = GameBootstrap.Instance.animalDatabase.animals[animalTypeId].shopIcon;
                        draggedAnimal.GetComponent<AnimalDragHandler>().animalTypeId = animalTypeId;
                        isDragging = true;
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

    public void OnDrag(PointerEventData eventData)
    {
        scrollRect.OnDrag(eventData);
        if (isDragging == true)
        {
            Vector2 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            draggedAnimal.transform.position = target;
        }
    }



    public void OnEndDrag(PointerEventData eventData)
    {
        scrollRect.OnEndDrag(eventData);
        if (isDragging == true)
        {
            if (draggedAnimal.GetComponent<AnimalDragHandler>().hasValidTarget == false)
            {
                NotificationController.Instance.ShowCanvasNotification("Không thể đặt ở đây!", "Can't put here");
            }
            else
            {
                draggedAnimal.GetComponent<AnimalDragHandler>().targetPen.GetComponent<AnimalPen>().PlaceAnimal();

            }
            Destroy(draggedAnimal);
            if (!PlayerPrefs.HasKey("huongdan"))
                DialogShop.instance.open();
            else
            {
                if (PlayerPrefs.GetInt("huongdan") != 22)
                    DialogShop.instance.open();
            }

            isDragging = false;
            MainCamera.Instance.unLockCam();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        pointerStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
}
