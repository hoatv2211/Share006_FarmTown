using FarmTown.Save;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
public class ProductItem : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
{
    [FormerlySerializedAs("idVp")] public int productId;
    [FormerlySerializedAs("obSinh"), SerializeField] GameObject dragPrefab;
    private GameObject draggedItem;
    private bool check;
    [FormerlySerializedAs("dialogTT"), SerializeField] GameObject productInfoDialog;
    [FormerlySerializedAs("tbUnLock"), SerializeField] GameObject unlockMessage;
    [FormerlySerializedAs("txtName"), SerializeField] Text productNameText;
    [FormerlySerializedAs("txttime"), SerializeField] Text productionTimeText;
    [FormerlySerializedAs("mangNL"), SerializeField] GameObject[] ingredientSlots;
    Vector2 pointerStart, pointerEnd;
    Coroutine showDialogCoroutine;
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (InventoryController.instance.products[productId].unlockLevel <= PlayerPrefs.GetInt("level"))
        {
            Vector2 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            check = true;
            draggedItem = Instantiate(dragPrefab, target, Quaternion.identity);
            draggedItem.GetComponent<SpriteRenderer>().sprite = InventoryController.instance.products[productId].icon;
            draggedItem.GetComponent<ProductionInputDragHandler>().productId = productId;
        }

    }

    public void OnDrag(PointerEventData eventData)
    {
        if (check)
        {
            Vector2 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            draggedItem.transform.position = target;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (check)
        {
            check = false;
            Destroy(draggedItem);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {

    }

    public void ShowProductInfo()
    {
        productInfoDialog.SetActive(true);
        RefreshIngredientInfo();
        productInfoDialog.transform.position = transform.parent.GetChild(1).position;
    }
    public void ShowUnlockMessage()
    {
        unlockMessage.SetActive(true);
        unlockMessage.transform.position = transform.parent.GetChild(1).position;
        if (Application.systemLanguage == SystemLanguage.Vietnamese)
        {
            unlockMessage.transform.GetChild(0).GetComponent<Text>().text = InventoryController.instance.products[productId].vietnameseName;
            unlockMessage.transform.GetChild(1).GetComponent<Text>().text = "Mở khóa ở cấp độ " + InventoryController.instance.products[productId].unlockLevel;
        }
        else
        {
            unlockMessage.transform.GetChild(0).GetComponent<Text>().text = InventoryController.instance.products[productId].englishName;
            unlockMessage.transform.GetChild(1).GetComponent<Text>().text = "Unlock at level " + InventoryController.instance.products[productId].unlockLevel;
        }
        productInfoDialog.transform.position = transform.parent.GetChild(1).position;
    }
    public void RefreshIngredientInfo()
    {
        if (Application.systemLanguage == SystemLanguage.Vietnamese)
        {
            productNameText.text = InventoryController.instance.products[productId].vietnameseName;
            productionTimeText.text = InventoryController.instance.products[productId].productionSeconds / 60 + " phút";
        }
        else
        {
            productNameText.text = InventoryController.instance.products[productId].englishName;
            productionTimeText.text = InventoryController.instance.products[productId].productionSeconds / 60 + " min";
        }
        for (int i = 0; i < InventoryController.instance.products[productId].ingredients.Length; i++)
        {
            ingredientSlots[i].SetActive(true);
            int id = InventoryController.instance.products[productId].ingredients[i].itemId;
            if (InventoryController.instance.products[productId].ingredients[i].category == InventoryController.ProductCategory.Crop)
            {
                ingredientSlots[i].GetComponent<Image>().sprite = GameBootstrap.Instance.cropDatabase.crops[id].productIcon;
                ingredientSlots[i].transform.GetChild(0).GetComponent<Text>().text = FarmingPlotPersistence.GetCropQuantity(id) + "/" + InventoryController.instance.products[productId].ingredients[i].quantity;
            }
            else
            {
                ingredientSlots[i].GetComponent<Image>().sprite = InventoryController.instance.products[id].icon;
                ingredientSlots[i].transform.GetChild(0).GetComponent<Text>().text = PlayerPrefs.GetInt("slvatpham" + id) + "/" + InventoryController.instance.products[productId].ingredients[i].quantity;
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (showDialogCoroutine != null)
            StopCoroutine(showDialogCoroutine);
        showDialogCoroutine = StartCoroutine(ShowProductInfoDelayed());
    }
    IEnumerator ShowProductInfoDelayed()
    {
        pointerStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        yield return new WaitForSeconds(1f);
        pointerEnd = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Vector2.Distance(pointerStart, pointerEnd) < .1f)
        {
            unlockMessage.SetActive(false);
            productInfoDialog.SetActive(false);
            for (int i = 0; i < ingredientSlots.Length; i++)
                ingredientSlots[i].SetActive(false);
            if (InventoryController.instance.products[productId].unlockLevel <= PlayerPrefs.GetInt("level"))
            {
                ShowProductInfo();
            }
            else
            {
                ShowUnlockMessage();
            }
        }
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        unlockMessage.SetActive(false);
        productInfoDialog.SetActive(false);
    }
}
