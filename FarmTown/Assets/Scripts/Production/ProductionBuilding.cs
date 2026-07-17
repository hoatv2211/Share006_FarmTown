using FarmTown.Save;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
public class ProductionBuilding : MonoBehaviour
{
    private static readonly Dictionary<string, ProductionBuilding> RuntimeBuildings = new Dictionary<string, ProductionBuilding>();
    [FormerlySerializedAs("stt")] public int instanceIndex;
    [FormerlySerializedAs("idNhaMay")] public int buildingTypeId;
    [FormerlySerializedAs("idVatPham")] public int currentProductId;
    [FormerlySerializedAs("time")] public int remainingSeconds;
    private int gemCost;
    bool check;
    [FormerlySerializedAs("iconSp"), SerializeField] private GameObject productRewardPrefab;
    [FormerlySerializedAs("iconExp"), SerializeField] private GameObject experienceRewardPrefab;
    [FormerlySerializedAs("harvestEffectPrefab"), SerializeField] private GameObject harvestEffectPrefab;
    Coroutine move;
    Vector2 pos1, pos2;
    private int checkDragMovement;
    [SerializeField] Sprite sprite;
    bool checkOpen;
    private string stableId;
    private string legacyName;
    private ProductionState productionState;
    public string StableId { get { EnsureIdentity(); return stableId; } }
    public string LegacyName { get { EnsureIdentity(); return legacyName; } }
    public ProductionState State { get { EnsureIdentity(); return productionState ?? (productionState = ProductionState.Load(stableId, legacyName)); } }
    private void Start()
    {
        EnsureIdentity();
        productionState = ProductionState.Load(stableId, legacyName);
        RestoreOfflineProduction();
        UpdateSortingOrder();
        RestoreCompletedProducts();
        if (ProductionPersistence.IsFlipped(stableId, legacyName))
            transform.GetChild(0).localScale = new Vector3(transform.GetChild(0).localScale.x * -1, transform.GetChild(0).localScale.y);

    }
    void RestoreOfflineProduction()
    {
        var elapsed = Mathf.Max(0, GameTime.TimeCurrent() - State.LastTimestamp);
        var firstRemainingTime = State.RemainingTime;
        if (State.QueueCount > 0 && firstRemainingTime == 0 && State.LastTimestamp > 0)
        {
            TryCompleteReadyHead();
            return;
        }
        var originalQueueCount = State.QueueCount;
        for (var originalSlot = 1; originalSlot <= originalQueueCount && State.QueueCount > 0; originalSlot++)
        {
            var productId = State.GetQueuedProduct(1);
            var duration = originalSlot == 1 && firstRemainingTime > 0
                ? firstRemainingTime
            : InventoryController.instance.products[productId].productionSeconds;
            if (elapsed < duration)
            {
                currentProductId = productId;
                remainingSeconds = duration - elapsed;
                transform.GetChild(0).GetComponent<Animator>().enabled = true;
                StartCoroutine(RunProductionTimer());
                return;
            }

            elapsed -= duration;
            if (!State.TryGetFreeCompletedSlot(out var completedSlot))
            {
                currentProductId = productId;
                remainingSeconds = 0;
                State.SetTimer(0, GameTime.TimeCurrent());
                return;
            }
            State.SetCompletedProduct(completedSlot, productId);
            State.RemoveQueuedProduct(1);
            transform.GetChild(completedSlot).gameObject.SetActive(true);
            transform.GetChild(completedSlot).GetChild(0).GetComponent<SpriteRenderer>().sprite = InventoryController.instance.products[productId].icon;
        }
    }
    private void OnMouseDown()
    {
        if (!EventSystem.current.IsPointerOverGameObject(0))
        {
            if (!PlayerPrefs.HasKey("huongdan"))
                if (PlayerPrefs.GetInt("dangdichuyen") == 0)
                {
                    if (move != null)
                        StopCoroutine(move);
                    move = StartCoroutine(BeginMoveAfterHold());
                }
        }
    }
    IEnumerator BeginMoveAfterHold()
    {
        pos1 = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        yield return new WaitForSeconds(.8f);
        pos2 = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Vector2.Distance(pos1, pos2) < .2f)
        {
            checkDragMovement = 1;
            MovementDialog.instance.ShowMovementIndicator(transform.position);
            yield return new WaitForSeconds(.3f);
            MovementDialog.instance.originalPosition = transform.position;
            MainCamera.Instance.lockCam();
            checkDragMovement = 2;
            gameObject.AddComponent<PlacementCollisionChecker>();
            gameObject.AddComponent<Rigidbody2D>();
            gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
            gameObject.GetComponent<PolygonCollider2D>().isTrigger = true;
            MainCamera.Instance.unLockCam();
            MovementDialog.instance.Open();
            MovementDialog.instance.confirmButton.onClick.AddListener(ConfirmMove);
            MovementDialog.instance.cancelButton.onClick.AddListener(CancelMove);
            MovementDialog.instance.rotateButton.onClick.AddListener(Rotate);
            PlayerPrefs.SetInt("dangdichuyen", 1);
        }


    }
    private void OnMouseDrag()
    {
        if (!EventSystem.current.IsPointerOverGameObject(0))
        {

            if (checkDragMovement == 2)
            {
                Vector2 cursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 posCam = new Vector2(cursorPosition.x, cursorPosition.y);
                Vector3 target = new Vector3(posCam.x, posCam.y, 0);
                target = new Vector3(((int)(posCam.x / 0.62f)) * 0.62f, ((int)(posCam.y / 0.32f)) * 0.32f, 0);
                transform.position = target;
            }
        }
    }
    private void OnMouseUp()
    {
        if (!EventSystem.current.IsPointerOverGameObject(0))
        {
            if (move != null)
                StopCoroutine(move);
            if (checkDragMovement == 1)
                checkDragMovement = 0;
            if (checkDragMovement == 2)
            {


            }
        }
    }
    public void ConfirmMove()
    {

        MovementDialog.instance.newPosition = transform.position;
        if (gameObject.GetComponent<PlacementCollisionChecker>().hasCollision == true)
            transform.position = MovementDialog.instance.originalPosition;
        else
        {
            ProductionPersistence.SetPosition(stableId, legacyName, transform.position);
        }
        Destroy(gameObject.GetComponent<Rigidbody2D>());
        Destroy(gameObject.GetComponent<PlacementCollisionChecker>());
        gameObject.GetComponent<PolygonCollider2D>().isTrigger = false;
        checkDragMovement = 0;
        UpdateSortingOrder();
        MainCamera.Instance.unLockCam();
        MovementDialog.instance.Close();
        PlayerPrefs.SetInt("dangdichuyen", 0);
    }
    public void CancelMove()
    {
        gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255);
        Destroy(gameObject.GetComponent<Rigidbody2D>());
        Destroy(gameObject.GetComponent<PlacementCollisionChecker>());
        gameObject.GetComponent<PolygonCollider2D>().isTrigger = false;
        MainCamera.Instance.unLockCam();
        transform.position = MovementDialog.instance.originalPosition;
        MovementDialog.instance.Close();
        checkDragMovement = 0;
        PlayerPrefs.SetInt("dangdichuyen", 0);
    }
    public void Rotate()
    {
        transform.GetChild(0).localScale = new Vector3(transform.GetChild(0).localScale.x * -1, transform.GetChild(0).localScale.y);
        ProductionPersistence.SetFlipped(stableId, legacyName, !ProductionPersistence.IsFlipped(stableId, legacyName));
    }
    private void OnMouseUpAsButton()
    {
        if (!EventSystem.current.IsPointerOverGameObject(0))
        {
            if (!PlayerPrefs.HasKey("huongdan"))
            {
                if (PlayerPrefs.GetInt("dangdichuyen") == 0)
                {
                    MainCamera.Instance.DisableAll();
                    ProductionBuildingClickHandler();
                }
            }
            else
            {
                if (PlayerPrefs.GetInt("huongdan") == 16)
                {
                    MainCamera.Instance.DisableAll();
                    ProductionBuildingClickHandler();
                }
            }
        }
    }
    public void ProductionBuildingClickHandler()
    {
        if (check)
        {
            CollectProduct();
            if (gameObject.GetComponent<Animator>().enabled == false)
                gameObject.GetComponent<Animator>().enabled = true;
            gameObject.GetComponent<Animator>().Play("test", -1, 0);
            MainCamera.Instance.MoveCamera(transform.position, false);
            AudioManager.Instance.Harvest();
        }
        else
        {
            if (!checkOpen)
            {
                checkOpen = true;
                StartCoroutine(ResetOpenGuard());
                if (gameObject.GetComponent<Animator>().enabled == false)
                    gameObject.GetComponent<Animator>().enabled = true;
                gameObject.GetComponent<Animator>().Play("test", -1, 0);
                //MainCamera.Instance.MoveCamera(transform.position, true);
                MainCamera.Instance.moveKhiCamLock(transform.position);
                ProductionPanel.instance.Open();
                ProductionPanel.instance.ShowBuildingProducts(buildingTypeId);
                ProductionPersistence.SetSelectedBuilding(stableId, legacyName);
                ProductionQueue.instance.RefreshQueueCapacity();
                ProductionQueue.instance.RefreshQueueSlots();
                ProductionQueue.instance.diamondButton.GetComponent<Button>().onClick.RemoveAllListeners();
                ProductionQueue.instance.diamondButton.GetComponent<Button>().onClick.AddListener(UseDiamonds);
                if (PlayerPrefs.GetInt("huongdan") == 16)
                {
                    TutorialController.instance.pointerHand.SetActive(false);
                    TutorialController.instance.productionDragHand.SetActive(true);
                    TutorialController.instance.ShowCanvasMessage("Kéo bánh mì vào nhà máy để sản xuất", "Pull the bread into the factory to produce");
                }
                if (buildingTypeId == 0)
                    AudioManager.Instance.bakery();
                if (buildingTypeId == 1)
                    AudioManager.Instance.popcorn();
                if (buildingTypeId == 2)
                    AudioManager.Instance.milk();
                if (buildingTypeId == 3)
                    AudioManager.Instance.oven();
                if (buildingTypeId == 4)
                    AudioManager.Instance.soup();
                if (buildingTypeId >= 5)
                    AudioManager.Instance.oven();
            }
        }

    }
    IEnumerator ResetOpenGuard()
    {
        yield return new WaitForSeconds(1f);
        checkOpen = false;
    }
    public void UseDiamonds()
    {
        if (gemCost <= PlayerPrefs.GetInt("diamond"))
        {
            GameBootstrap.Instance.AddDiamonds(-gemCost);
            remainingSeconds = 0;
            if (PlayerPrefs.HasKey("huongdan"))
            {
                TutorialController.instance.speedUpPointerHand.SetActive(false);
                ProductionPanel.instance.Close();
                TutorialController.instance.pointerHand.SetActive(true);
                TutorialController.instance.ShowWorldMessage("Nhấn để thu hoạch vật phẩm", "Click to harvest produce");
            }
        }
    }
    public void CollectProduct()
    {
        check = false;
        for (int i = 1; i <= 5; i++)
        {
            if (State.HasCompletedProduct(i))
            {
                var completedProductId = State.GetCompletedProduct(i);
                PlayerPrefs.SetInt("slvatpham" + completedProductId, PlayerPrefs.GetInt("slvatpham" + completedProductId) + 1);
                InventoryController.instance.UpdateProductQuantity(completedProductId);
                GameObject ob = Instantiate(productRewardPrefab, new Vector2(transform.position.x + Random.Range(-.7f, .7f), transform.position.y + Random.Range(0f, .7f)), Quaternion.identity);
                GameObject ob2 = Instantiate(experienceRewardPrefab, new Vector2(transform.position.x + Random.Range(-.7f, .7f), transform.position.y + Random.Range(0f, .7f)), Quaternion.identity);
                ob2.GetComponent<RewardFlyout>().id = 2;
                ob2.GetComponent<RewardFlyout>().numberCoin = InventoryController.instance.products[completedProductId].experienceReward;
                ob.GetComponent<RewardFlyout>().id = 1;
                ob.GetComponent<RewardFlyout>().numberCoin = 1;
                ob.transform.GetChild(0).GetComponent<Image>().sprite = InventoryController.instance.products[completedProductId].icon;
                ob.transform.GetChild(0).GetComponent<Image>().SetNativeSize();
                transform.GetChild(i).gameObject.SetActive(false);
                InventoryController.instance.UpdateProductQuantity(completedProductId);
                State.DeleteCompletedProduct(i);
                if (PlayerPrefs.HasKey("huongdan"))
                {
                    PlayerPrefs.SetInt("huongdan", 17);
                    TutorialController.instance.pointerHand.SetActive(false);
                    TutorialController.instance.shopPointerHand.SetActive(true);
                    TutorialController.instance.shopHighlight.SetActive(true);
                    TutorialController.instance.productionDragHand.SetActive(false);
                    TutorialController.instance.ShowWorldMessage("Nhấn vào cửa hàng", "Click to icon shop");
                }
            }
        }
        OrderPanel.instance.RefreshOrders();
        TryCompleteReadyHead();
        GameObject obj = Instantiate(harvestEffectPrefab, transform.position, Quaternion.identity);
        Destroy(obj, 2f);
    }
    public void RestoreCompletedProducts()
    {
        for (int i = 1; i <= 5; i++)
        {
            if (State.HasCompletedProduct(i))
            {
                check = true;
                transform.GetChild(i).gameObject.SetActive(true);
                transform.GetChild(i).GetChild(0).GetComponent<SpriteRenderer>().sprite = InventoryController.instance.products[State.GetCompletedProduct(i)].icon;
            }
        }
    }
    public void StartProduction()
    {
        //san xuat vp dau tien
        currentProductId = State.GetQueuedProduct(1);
                remainingSeconds = InventoryController.instance.products[currentProductId].productionSeconds;
        OrderPanel.instance.RefreshOrders();
        StartCoroutine(RunProductionTimer());
        transform.GetChild(0).GetComponent<Animator>().enabled = true;
    }

    private bool TryCompleteReadyHead()
    {
        if (State.QueueCount == 0 || State.RemainingTime != 0 || !State.TryGetFreeCompletedSlot(out var completedSlot))
            return false;

        var productId = State.GetQueuedProduct(1);
        State.SetCompletedProduct(completedSlot, productId);
        State.RemoveQueuedProduct(1);
        transform.GetChild(completedSlot).gameObject.SetActive(true);
        transform.GetChild(completedSlot).GetChild(0).GetComponent<SpriteRenderer>().sprite = InventoryController.instance.products[productId].icon;
        check = true;
        if (State.QueueCount > 0)
            StartProduction();
        else
        {
            transform.GetChild(0).GetComponent<Animator>().enabled = false;
            transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = sprite;
        }
        return true;
    }
    IEnumerator RunProductionTimer()
    {
        if (ProductionPersistence.GetSelectedBuilding() == stableId)
        {
            ProductionQueue.instance.timeText.text = (remainingSeconds / 60).ToString() + "m " + (remainingSeconds % 60).ToString() + "s";
            ProductionQueue.instance.diamondCostText.text = (remainingSeconds / 60 + 1).ToString();
            gemCost = (int)(remainingSeconds / 60) + 1;
        }
        State.SetTimer(remainingSeconds, GameTime.TimeCurrent());
        yield return new WaitForSeconds(1f);
        remainingSeconds -= 1;
        if (remainingSeconds > 0)
        {
            StartCoroutine(RunProductionTimer());
        }
        else
        {
            check = true;
            if (!State.TryGetFreeCompletedSlot(out var completedSlot))
            {
                State.SetTimer(0, GameTime.TimeCurrent());
                transform.GetChild(0).GetComponent<Animator>().enabled = false;
                yield break;
            }
            State.SetCompletedProduct(completedSlot, currentProductId);
            transform.GetChild(completedSlot).gameObject.SetActive(true);
            transform.GetChild(completedSlot).GetChild(0).GetComponent<SpriteRenderer>().sprite = InventoryController.instance.products[currentProductId].icon;
            State.RemoveQueuedProduct(1);
            Debug.Log("vatphamhoanthanh:" + currentProductId);
            if (State.QueueCount > 0)
                StartProduction();
            else
            {
                transform.GetChild(0).GetComponent<Animator>().enabled = false;
                transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = sprite;
            }
            if (ProductionPersistence.GetSelectedBuilding() == stableId)
            ProductionQueue.instance.RefreshQueueSlots();
        }
    }

    public void InitializeIdentity(int typeId, int instanceIndex)
    {
        buildingTypeId = typeId;
        this.instanceIndex = instanceIndex;
        EnsureIdentity();
    }

    public static bool TryFind(string buildingStableId, out ProductionBuilding building)
    {
        return RuntimeBuildings.TryGetValue(buildingStableId, out building) && building != null;
    }

    private void EnsureIdentity()
    {
        if (!string.IsNullOrEmpty(stableId)) return;
        legacyName = ProductionPersistence.DefaultLegacyName(buildingTypeId, instanceIndex);
        var identity = GetComponent<StableInstanceId>() ?? gameObject.AddComponent<StableInstanceId>();
        if (!identity.TryAssign(ProductionPersistence.DefaultStableId(buildingTypeId, instanceIndex)))
            throw new System.InvalidOperationException($"Cannot assign stable ID for production building {buildingTypeId}:{instanceIndex}.");
        stableId = identity.StableId;
        RuntimeBuildings[stableId] = this;
    }

    private void OnDestroy()
    {
        UnregisterRuntimeBuilding();
    }

    private void OnEnable()
    {
        if (string.IsNullOrEmpty(stableId)) return;
        if (RuntimeBuildings.TryGetValue(stableId, out var existing) && existing != null && existing != this)
        {
            Debug.LogError($"Duplicate production building stable ID '{stableId}' on {name}.", this);
            enabled = false;
            return;
        }
        RuntimeBuildings[stableId] = this;
    }

    private void OnDisable()
    {
        UnregisterRuntimeBuilding();
    }

    private void UnregisterRuntimeBuilding()
    {
        if (!string.IsNullOrEmpty(stableId) && RuntimeBuildings.TryGetValue(stableId, out var building) && building == this)
            RuntimeBuildings.Remove(stableId);
    }
    public void UpdateSortingOrder()
    {
        Vector2 target = transform.position;
        transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt((13 - target.y) * 100f);
    }
}
