using FarmTown.Save;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;

public class ProductionQueue : MonoBehaviour, IProductionInputReceiver
{
    public static ProductionQueue instance;
    [FormerlySerializedAs("btnKc1")] public GameObject firstSlotUpgradeButton;
    [FormerlySerializedAs("btnKc2")] public GameObject secondSlotUpgradeButton;
    [FormerlySerializedAs("osanxuat4")] public GameObject fourthProductionSlot;
    [FormerlySerializedAs("osanxuat5")] public GameObject fifthProductionSlot;
    [FormerlySerializedAs("btnKimCuong")] public GameObject diamondButton;
    [FormerlySerializedAs("txtTime")] public Text timeText;
    [FormerlySerializedAs("txtKC")] public Text diamondCostText;

    private ProductionBuilding SelectedBuilding
    {
        get
        {
            var stableId = ProductionPersistence.GetSelectedBuilding();
            if (ProductionBuilding.TryFind(stableId, out var building)) return building;
            throw new System.InvalidOperationException($"Selected production building '{stableId}' is not active.");
        }
    }

    private void Start()
    {
        instance = this;
    }

    public void AddDraggedProduct(int id)
    {
        var availableIngredientCount = 0;
        for (var index = 0; index < InventoryController.instance.products[id].ingredients.Length; index++)
        {
            var ingredient = InventoryController.instance.products[id].ingredients[index];
            if (ingredient.category == InventoryController.ProductCategory.Crop)
            {
                if (FarmingPlotPersistence.GetCropQuantity(ingredient.itemId) >= ingredient.quantity)
                    availableIngredientCount++;
            }
            else if (PlayerPrefs.GetInt("slvatpham" + ingredient.itemId) >= ingredient.quantity)
            {
                availableIngredientCount++;
            }
        }

        if (availableIngredientCount != InventoryController.instance.products[id].ingredients.Length)
        {
            NotificationController.Instance.ShowCanvasNotification("Không đủ nguyên liệu!", "Not enough material!");
            return;
        }

        var building = SelectedBuilding;
        var state = building.State;
        if (!state.CanEnqueue)
        {
            NotificationController.Instance.ShowCanvasNotification("Hết ô trống để sản xuất!", "Out of empty boxes to produce!");
            return;
        }

        ConsumeIngredients(id);
        if (!state.TryEnqueue(id))
        {
            RefundIngredients(id);
            NotificationController.Instance.ShowCanvasNotification("Hết ô trống để sản xuất!", "Out of empty boxes to produce!");
            return;
        }

        if (state.QueueCount == 1)
        {
            transform.GetChild(0).GetChild(1).localScale = Vector3.one;
            diamondButton.SetActive(true);
            building.StartProduction();
        }

        var slot = transform.GetChild(state.QueueCount - 1).GetChild(0);
        slot.localScale = Vector3.one;
        slot.GetComponent<Image>().sprite = InventoryController.instance.products[id].icon;

        if (PlayerPrefs.GetInt("huongdan") == 16)
        {
            TutorialController.instance.productionDragHand.SetActive(false);
            TutorialController.instance.speedUpPointerHand.SetActive(true);
            TutorialController.instance.ShowCanvasMessage("Nhấn vào icon kim cương để bỏ qua thời gian chờ", "Click on the diamond icon to skip the waiting time");
        }
    }

    public void ConsumeIngredients(int productId)
    {
        for (var index = 0; index < InventoryController.instance.products[productId].ingredients.Length; index++)
        {
            var ingredient = InventoryController.instance.products[productId].ingredients[index];
            if (ingredient.category == InventoryController.ProductCategory.Crop)
            {
                FarmingPlotPersistence.SetCropQuantity(ingredient.itemId, FarmingPlotPersistence.GetCropQuantity(ingredient.itemId) - ingredient.quantity);
                InventoryController.instance.UpdateCropQuantity(ingredient.itemId);
            }
            else
            {
                PlayerPrefs.SetInt("slvatpham" + ingredient.itemId, PlayerPrefs.GetInt("slvatpham" + ingredient.itemId) - ingredient.quantity);
                InventoryController.instance.UpdateProductQuantity(ingredient.itemId);
            }
        }
        OrderPanel.instance.RefreshOrders();
    }

    private void RefundIngredients(int productId)
    {
        for (var index = 0; index < InventoryController.instance.products[productId].ingredients.Length; index++)
        {
            var ingredient = InventoryController.instance.products[productId].ingredients[index];
            if (ingredient.category == InventoryController.ProductCategory.Crop)
            {
                FarmingPlotPersistence.SetCropQuantity(ingredient.itemId, FarmingPlotPersistence.GetCropQuantity(ingredient.itemId) + ingredient.quantity);
                InventoryController.instance.UpdateCropQuantity(ingredient.itemId);
            }
            else
            {
                PlayerPrefs.SetInt("slvatpham" + ingredient.itemId, PlayerPrefs.GetInt("slvatpham" + ingredient.itemId) + ingredient.quantity);
                InventoryController.instance.UpdateProductQuantity(ingredient.itemId);
            }
        }
        OrderPanel.instance.RefreshOrders();
    }

    public void RefreshQueueSlots()
    {
        var state = SelectedBuilding.State;
        for (var slotIndex = 1; slotIndex <= 5; slotIndex++)
        {
            var slot = transform.GetChild(slotIndex - 1);
            if (slotIndex <= state.QueueCount)
            {
                if (slotIndex == 1)
                {
                    slot.GetChild(1).localScale = Vector3.one;
                    diamondButton.SetActive(true);
                }
                slot.GetChild(0).localScale = Vector3.one;
                slot.GetChild(0).GetComponent<Image>().sprite = InventoryController.instance.products[state.GetQueuedProduct(slotIndex)].icon;
            }
            else
            {
                if (slotIndex == 1)
                {
                    slot.GetChild(1).localScale = Vector3.zero;
                    diamondButton.SetActive(false);
                }
                slot.GetChild(0).localScale = Vector3.zero;
            }
        }
    }

    public void RefreshQueueCapacity()
    {
        var capacity = SelectedBuilding.State.QueueCapacity;
        firstSlotUpgradeButton.SetActive(capacity == 3);
        secondSlotUpgradeButton.SetActive(capacity == 4);
        fourthProductionSlot.SetActive(capacity >= 4);
        fifthProductionSlot.SetActive(capacity >= 5);
    }

    public void AddQueueSlot()
    {
        if (PlayerPrefs.GetInt("diamond") < 6)
        {
            NotificationController.Instance.ShowCanvasNotification("Không đủ kim cương!", "Not enough diamond!");
            return;
        }

        var state = SelectedBuilding.State;
        if (state.QueueCapacity >= 5) return;
        GameBootstrap.Instance.AddDiamonds(-6);
        state.SetQueueCapacity(state.QueueCapacity + 1);
        RefreshQueueCapacity();
    }
}
