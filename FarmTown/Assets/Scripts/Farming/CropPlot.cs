using System;
using System.Collections;
using System.Collections.Generic;
using FarmTown.Save;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
//电子邮件puhalskijsemen@gmail.com
//源码网站 开vpn全局模式打开 http://web3incubators.com/
//电报https://t.me/gamecode999
//网页客服 http://web3incubators.com/kefu.html
public class CropPlot : MonoBehaviour, ISeedReceiver
{
    public static CropPlot FirstRuntimePlot { get; private set; }
    [FormerlySerializedAs("spr"), SerializeField] SpriteRenderer cropRenderer;
    [FormerlySerializedAs("idcay"), SerializeField] int cropId;
    [FormerlySerializedAs("time"), SerializeField] int remainingSeconds;
    [FormerlySerializedAs("timegoc"), SerializeField] int totalGrowthSeconds;
    [FormerlySerializedAs("fxTrong"), SerializeField] GameObject plantingEffectPrefab;
    [FormerlySerializedAs("fxExp"), SerializeField] GameObject experienceEffectPrefab;
    [FormerlySerializedAs("fxSanPham"), SerializeField] GameObject productRewardPrefab;
    [FormerlySerializedAs("fxThuHoach"), SerializeField] GameObject harvestEffectPrefab;
    int moveCheck;
    Vector2 pointerStart, pointerEnd;
    Coroutine moveCoroutine;
    StableInstanceId stableIdentity;
    string StableId => stableIdentity != null ? stableIdentity.StableId : string.Empty;
    string LegacyAlias => FarmingPlotPersistence.TryGetIndex(StableId, out var index) ? FarmingPlotPersistence.DefaultLegacyAlias(index) : string.Empty;
  
    private void Awake()
    {
        if (FirstRuntimePlot == null)
            FirstRuntimePlot = this;
    }

    private void Start()
    {
        stableIdentity = GetComponent<StableInstanceId>();
        UpdateSortingOrder();
        if (HasPlotKey(SaveKeys.PlotRemainingTime(StableId), LegacySaveKeys.PlotRemainingTime(LegacyAlias)))
        {
            cropId = GetPlotInt(SaveKeys.PlotCropId(StableId), LegacySaveKeys.PlotCropId(LegacyAlias));
            totalGrowthSeconds = GameBootstrap.Instance.cropDatabase.crops[cropId].growthSeconds;
            int offlineElapsedSeconds = Mathf.Abs(GameTime.TimeCurrent() - GetPlotInt(SaveKeys.PlotLastTimestamp(StableId), LegacySaveKeys.PlotLastTimestamp(LegacyAlias)));
            //cay da lon
            if (offlineElapsedSeconds >= GetPlotInt(SaveKeys.PlotRemainingTime(StableId), LegacySaveKeys.PlotRemainingTime(LegacyAlias)))
            {
                remainingSeconds = 0;
                cropRenderer.sprite = GameBootstrap.Instance.cropDatabase.crops[cropId].growthStage3;
                transform.GetChild(0).GetComponent<Animator>().enabled = true;
            }
            else
            {
                remainingSeconds = GetPlotInt(SaveKeys.PlotRemainingTime(StableId), LegacySaveKeys.PlotRemainingTime(LegacyAlias)) - offlineElapsedSeconds;
                StartCoroutine(GrowCrop());
            }
        }
        if (PlayerPrefs.HasKey("huongdan"))
        {
            if (PlayerPrefs.GetInt("huongdan") == 1)
            {
                remainingSeconds = 0;
                cropRenderer.sprite = GameBootstrap.Instance.cropDatabase.crops[0].growthStage3;
                transform.GetChild(0).GetComponent<Animator>().enabled = true;
            }
        }
    }
    public bool IsPointerOverUI()
    {
        PointerEventData pointerEvent = new PointerEventData(EventSystem.current);
        pointerEvent.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEvent, results);
        return results.Count > 0;
    }
    private void UpdateSortingOrder()
    {
        Vector2 target = transform.position;
        gameObject.GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt((13 - target.y) * 100f);
        transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = gameObject.GetComponent<SpriteRenderer>().sortingOrder + 1;
    }

    public void PlantCrop(int id)
    {
        if (cropRenderer.sprite == null)
        {
            if (FarmingPlotPersistence.GetCropQuantity(id) > 0)
            {
                FarmingPlotPersistence.SetCropQuantity(id, FarmingPlotPersistence.GetCropQuantity(id) - 1);
                Plant(id);
                InventoryController.instance.UpdateCropQuantity(id);
            }
            else
            {
                if (transform.GetChild(1).gameObject.activeInHierarchy == false)
                {
                    transform.GetChild(1).gameObject.SetActive(true);
                    transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = GameBootstrap.Instance.cropDatabase.crops[id].productIcon;
                    if (!PlayerPrefs.HasKey("sohg"))
                        PlayerPrefs.SetInt("sohg", 0);
                    var targetIndex = PlayerPrefs.HasKey(SaveKeys.OutOfSeedTargetCount) ? PlayerPrefs.GetInt(SaveKeys.OutOfSeedTargetCount) : PlayerPrefs.GetInt(LegacySaveKeys.OutOfSeedTargetCount);
                    PlayerPrefs.SetString(SaveKeys.OutOfSeedTarget(targetIndex), StableId);
                    PlayerPrefs.SetString(LegacySaveKeys.OutOfSeedTarget(targetIndex), LegacyAlias);
                    PlayerPrefs.SetInt(SaveKeys.OutOfSeedTargetCount, targetIndex + 1);
                    PlayerPrefs.SetInt(LegacySaveKeys.OutOfSeedTargetCount, targetIndex + 1);
                }
            }
        }
    }
    public void HideOutOfSeedIndicator()
    {
        transform.GetChild(1).gameObject.SetActive(false);
    }
    public void Plant(int id)
    {
        GameObject obj = Instantiate(plantingEffectPrefab, transform.position, Quaternion.identity);
        obj.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = GameBootstrap.Instance.cropDatabase.crops[id].productIcon;
        Destroy(obj, 1f);
        cropId = id;
        totalGrowthSeconds = GameBootstrap.Instance.cropDatabase.crops[cropId].growthSeconds;
        remainingSeconds = totalGrowthSeconds;
        cropRenderer.sprite = GameBootstrap.Instance.cropDatabase.crops[cropId].growthStage1;
        OrderPanel.instance.RefreshOrders();
        StartCoroutine(GrowCrop());
    }
    private void OnMouseDown()
    {
        if (!EventSystem.current.IsPointerOverGameObject(0))
        {
            if (!PlayerPrefs.HasKey("huongdan"))
                if (PlayerPrefs.GetInt("dangdichuyen") == 0)
                {
                    if (moveCoroutine != null)
                        StopCoroutine(moveCoroutine);
                    moveCoroutine = StartCoroutine(BeginMoveAfterHold());
                }
        }
    }
    IEnumerator BeginMoveAfterHold()
    {
        pointerStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        yield return new WaitForSeconds(.8f);
        pointerEnd = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Vector2.Distance(pointerStart, pointerEnd) < .2f)
        {
            moveCheck = 1;
            MovementDialog.instance.ShowMovementIndicator(transform.position);
            yield return new WaitForSeconds(.3f);
            MovementDialog.instance.originalPosition = transform.position;
            MainCamera.Instance.lockCam();
            moveCheck = 2;
            gameObject.AddComponent<GroundPlacementCollisionChecker>();
            gameObject.AddComponent<Rigidbody2D>();
            gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
            gameObject.GetComponent<PolygonCollider2D>().isTrigger = true;
            MainCamera.Instance.unLockCam();
            MovementDialog.instance.Open();
            MovementDialog.instance.confirmButton.onClick.AddListener(ConfirmMove);
            MovementDialog.instance.cancelButton.onClick.AddListener(CancelMove);
            PlayerPrefs.SetInt("dangdichuyen", 1);
        }


    }
    private void OnMouseDrag()
    {
        if (!EventSystem.current.IsPointerOverGameObject(0))
        {

            if (moveCheck == 2)
            {
                Vector2 cursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 posCam = new Vector2(cursorPosition.x, cursorPosition.y);
                Vector3 target = new Vector3(posCam.x, posCam.y, 0);
                target = new Vector3(((int)(posCam.x / 0.78f)) * 0.78f, ((int)(posCam.y / 0.4f)) * 0.4f, 0);
                transform.position = target;
            }
        }
    }
    private void OnMouseUp()
    {
        if (!EventSystem.current.IsPointerOverGameObject(0))
        {
            if (moveCoroutine != null)
                StopCoroutine(moveCoroutine);
            if (moveCheck == 1)
                moveCheck = 0;
            if (moveCheck == 2)
            {


            }
        }
    }
    public void ConfirmMove()
    {

        MovementDialog.instance.newPosition = transform.position;
        if (gameObject.GetComponent<GroundPlacementCollisionChecker>().hasCollision == true)
            transform.position = MovementDialog.instance.originalPosition;
        else
        {
            SavePosition();
        }
        Destroy(gameObject.GetComponent<Rigidbody2D>());
        Destroy(gameObject.GetComponent<GroundPlacementCollisionChecker>());
        gameObject.GetComponent<PolygonCollider2D>().isTrigger = false;
        moveCheck = 0;
        UpdateSortingOrder();
        MainCamera.Instance.unLockCam();
        MovementDialog.instance.Close();
        PlayerPrefs.SetInt("dangdichuyen", 0);
    }
    public void CancelMove()
    {
        gameObject.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255);
        Destroy(gameObject.GetComponent<Rigidbody2D>());
        Destroy(gameObject.GetComponent<PlacementCollisionChecker>());
        gameObject.GetComponent<PolygonCollider2D>().isTrigger = false;
        MainCamera.Instance.unLockCam();
        transform.position = MovementDialog.instance.originalPosition;
        MovementDialog.instance.Close();
        moveCheck = 0;
        PlayerPrefs.SetInt("dangdichuyen", 0);
    }
    private void OnMouseUpAsButton()
    {
        if (!EventSystem.current.IsPointerOverGameObject(0))
        {
            if (PlayerPrefs.GetInt("dangdichuyen") == 0)
            {
                if (!PlayerPrefs.HasKey("huongdan"))
                {
                    MovementDialog.instance.Close();
                    MainCamera.Instance.CloseSmallDialogs();
                    if (cropRenderer.sprite == null)
                    {
                        SeedSelectionPanel.instance.open();
                    }
                    else
                    {
                        if (remainingSeconds > 0)
                        {
                            //cay dang lon
                            PlayerPrefs.SetString(SaveKeys.SelectedPlot, StableId);
                            PlayerPrefs.SetString(LegacySaveKeys.SelectedPlot, LegacyAlias);
                            if (Application.systemLanguage == SystemLanguage.Vietnamese)
                                CropTimerPanel.instance.ShowTimer(transform.position, GameBootstrap.Instance.cropDatabase.crops[cropId].vietnameseName, remainingSeconds, totalGrowthSeconds);
                            else
                                CropTimerPanel.instance.ShowTimer(transform.position, GameBootstrap.Instance.cropDatabase.crops[cropId].englishName, remainingSeconds, totalGrowthSeconds);
                            CropTimerPanel.instance.gemCost = remainingSeconds / 60 + 1;
                        }
                        else
                        {
                            //cay da lon
                            SickleTool.instance.open(transform.position);
                            DialogShop.instance.close();
                            InventoryController.instance.Close();
                        }
                    }
                }
                else//ton tai huong dan
                {
                    if (PlayerPrefs.GetInt("huongdan") < 7)
                    {
                        SickleTool.instance.open(transform.position);
                        TutorialController.instance.secondaryUiDragHand.SetActive(true);
                        TutorialController.instance.pointerHand.SetActive(false);
                        TutorialController.instance.ShowWorldMessage("Kéo liềm để thu hoạch!", "Pull the sickle to harvest!");

                    }
                    if (PlayerPrefs.GetInt("huongdan") == 7 || PlayerPrefs.GetInt("huongdan") == 8)
                    {
                        PlayerPrefs.SetInt("huongdan", 8);
                        MovementDialog.instance.Close();
                        MainCamera.Instance.CloseSmallDialogs();
                        SeedSelectionPanel.instance.open();
                        TutorialController.instance.pointerHand.SetActive(false);
                        TutorialController.instance.uiDragHand.SetActive(true);
                    }
                    if(PlayerPrefs.GetInt("huongdan") == 8)
                    {
                        TutorialController.instance.ShowCameraMessage("Kéo hạt giống để trồng", "Pull seeds to plant");
                    }
                }
            }
        }
    }
    public void FinishGrowthWithDiamonds()
    {
        remainingSeconds = 0;

    }
    private void OnTriggerEnter2D(Collider2D target)
    {

        if (target.tag == "liem")
        {
            Harvest();
        }
    }
    public void Harvest()
    {
        if (remainingSeconds < 1 && cropRenderer.sprite != null)
        {
            cropRenderer.sprite = null;
            GameObject ob1 = Instantiate(productRewardPrefab, transform.position, Quaternion.identity);
            ob1.GetComponent<RewardFlyout>().id = 0;
            ob1.transform.GetChild(0).GetComponent<Image>().sprite = GameBootstrap.Instance.cropDatabase.crops[cropId].productIcon;
            ob1.GetComponent<RewardFlyout>().numberCoin = GameBootstrap.Instance.cropDatabase.crops[cropId].harvestYield;
            GameObject ob2 = Instantiate(experienceEffectPrefab, transform.position, Quaternion.identity);
            ob2.GetComponent<RewardFlyout>().id = 2;
            ob2.GetComponent<RewardFlyout>().numberCoin = GameBootstrap.Instance.cropDatabase.crops[cropId].experienceReward;
            transform.GetChild(0).GetComponent<Animator>().enabled = false;
            transform.GetChild(0).localScale = Vector3.one;
            FarmingPlotPersistence.SetCropQuantity(cropId, FarmingPlotPersistence.GetCropQuantity(cropId) + GameBootstrap.Instance.cropDatabase.crops[cropId].harvestYield);
            InventoryController.instance.UpdateCropQuantity(cropId);
            OrderPanel.instance.RefreshOrders();
            DeleteCropState();
            GameObject obj = Instantiate(harvestEffectPrefab, transform.position, Quaternion.identity);
            Destroy(obj, 2f);
            if (PlayerPrefs.HasKey("huongdan"))
            {
                PlayerPrefs.SetInt("huongdan", PlayerPrefs.GetInt("huongdan") + 1);

                if (PlayerPrefs.GetInt("huongdan") == 7)
                {
                    TutorialController.instance.pointerHand.SetActive(true);
                    TutorialController.instance.ShowWorldMessage("Nhấn vào ô đất để trồng", "Click on the field to start planting");
                }
            }
            AudioManager.Instance.Harvest();
        }
    }

    private static bool HasPlotKey(string englishKey, string legacyKey) => PlayerPrefs.HasKey(englishKey) || PlayerPrefs.HasKey(legacyKey);

    private static int GetPlotInt(string englishKey, string legacyKey) =>
        PlayerPrefs.HasKey(englishKey) ? PlayerPrefs.GetInt(englishKey) : PlayerPrefs.GetInt(legacyKey);

    private void SavePosition()
    {
        if (!FarmingPlotPersistence.TryGetIndex(StableId, out var index)) return;
        PlayerPrefs.SetFloat(SaveKeys.PlotPositionX(StableId), transform.position.x);
        PlayerPrefs.SetFloat(SaveKeys.PlotPositionY(StableId), transform.position.y);
        PlayerPrefs.SetFloat(LegacySaveKeys.PlotPositionX(index), transform.position.x);
        PlayerPrefs.SetFloat(LegacySaveKeys.PlotPositionY(index), transform.position.y);
    }

    private void SaveCropState()
    {
        PlayerPrefs.SetInt(SaveKeys.PlotCropId(StableId), cropId);
        PlayerPrefs.SetInt(SaveKeys.PlotRemainingTime(StableId), remainingSeconds);
        PlayerPrefs.SetInt(SaveKeys.PlotLastTimestamp(StableId), GameTime.TimeCurrent());
        PlayerPrefs.SetInt(LegacySaveKeys.PlotCropId(LegacyAlias), cropId);
        PlayerPrefs.SetInt(LegacySaveKeys.PlotRemainingTime(LegacyAlias), remainingSeconds);
        PlayerPrefs.SetInt(LegacySaveKeys.PlotLastTimestamp(LegacyAlias), GameTime.TimeCurrent());
    }

    private void DeleteCropState()
    {
        PlayerPrefs.DeleteKey(SaveKeys.PlotCropId(StableId));
        PlayerPrefs.DeleteKey(SaveKeys.PlotRemainingTime(StableId));
        PlayerPrefs.DeleteKey(SaveKeys.PlotLastTimestamp(StableId));
        PlayerPrefs.DeleteKey(LegacySaveKeys.PlotCropId(LegacyAlias));
        PlayerPrefs.DeleteKey(LegacySaveKeys.PlotRemainingTime(LegacyAlias));
        PlayerPrefs.DeleteKey(LegacySaveKeys.PlotLastTimestamp(LegacyAlias));
    }

    IEnumerator GrowCrop()
    {
        if (PlayerPrefs.GetString(SaveKeys.SelectedPlot, PlayerPrefs.GetString(LegacySaveKeys.SelectedPlot)) == StableId ||
            PlayerPrefs.GetString(LegacySaveKeys.SelectedPlot) == LegacyAlias)
        {
            CropTimerPanel.instance.gemCost = remainingSeconds / 60 + 1;
            if (Application.systemLanguage == SystemLanguage.Vietnamese)
                CropTimerPanel.instance.ShowTimer(transform.position, GameBootstrap.Instance.cropDatabase.crops[cropId].vietnameseName, remainingSeconds, totalGrowthSeconds);
            else
                CropTimerPanel.instance.ShowTimer(transform.position, GameBootstrap.Instance.cropDatabase.crops[cropId].englishName, remainingSeconds, totalGrowthSeconds);
        }
        SaveCropState();
        yield return new WaitForSeconds(1f);
        remainingSeconds -= 1;
        if (remainingSeconds > totalGrowthSeconds / 2)
        {
            cropRenderer.sprite = GameBootstrap.Instance.cropDatabase.crops[cropId].growthStage1;
        }
        else
        if (remainingSeconds > 0)
        {
            cropRenderer.sprite = GameBootstrap.Instance.cropDatabase.crops[cropId].growthStage2;
        }
        else
        {
            SaveCropState();
            transform.GetChild(0).GetComponent<Animator>().enabled = true;
            cropRenderer.sprite = GameBootstrap.Instance.cropDatabase.crops[cropId].growthStage3;
            if (PlayerPrefs.GetInt("huongdan") == 3)
            {
                TutorialController.instance.MovePointerHand(transform.position);
                TutorialController.instance.pointerHand.SetActive(true);
                PlayerPrefs.SetInt("huongdan", 4);
            }
        }
        if (remainingSeconds > 0)
        {
            StartCoroutine(GrowCrop());
        }
    }

    private void OnDestroy()
    {
        if (FirstRuntimePlot == this)
            FirstRuntimePlot = null;
    }
}
