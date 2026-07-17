using FarmTown.Save;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Serialization;

public class FarmAnimal : MonoBehaviour, IFeedReceiver
{
    private static readonly Dictionary<string, FarmAnimal> RuntimeAnimals = new Dictionary<string, FarmAnimal>();
    private const string WaitingState = "doi";
    private const string FeedingState = "an";
    private const string ReadyState = "thuhoach";
    private const string IdleAnimation = "Idle";
    private const string FeedingAnimation = "Feeding";
    private const string ReadyAnimation = "Ready";
    public string CurrentState
    {
        get
        {
            EnsureIdentity();
            return GetState();
        }
    }
    private int feedCropId;
    private int animalTypeId;
    [FormerlySerializedAs("time")] public int remainingSeconds;
    [FormerlySerializedAs("timegoc")] public int totalProductionSeconds;
    private SkeletonAnimation skeletonAnimation;
    string penStableId;
    string legacyPenName;
    string legacyAnimalName;
    string runtimeLookupKey;
    int slotIndex;
    [FormerlySerializedAs("iconExp"), SerializeField] GameObject experienceRewardPrefab;
    [FormerlySerializedAs("iconSp"), SerializeField] GameObject productRewardPrefab;
    [FormerlySerializedAs("FxAn"), SerializeField] GameObject feedingEffectPrefab;
    [FormerlySerializedAs("HieuUngThuHoach"), SerializeField] GameObject harvestEffectPrefab;
    void Start()
    {
        EnsureIdentity();
        var pen = transform.parent.GetComponent<AnimalPen>();
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        feedCropId = pen.feedCropId;
        animalTypeId = pen.animalTypeId;
        totalProductionSeconds = GameBootstrap.Instance.animalDatabase.animals[animalTypeId].productionSeconds;
        if (!AnimalPersistence.HasAnimalState(penStableId, slotIndex, legacyAnimalName, legacyPenName))
        {
            SetState(WaitingState);
        }
        else
        {
            if (GetState() == WaitingState)
            {
                skeletonAnimation.AnimationName = IdleAnimation;
            }
            if (GetState() == ReadyState)
            {
                skeletonAnimation.AnimationName = ReadyAnimation;
                transform.GetChild(0).gameObject.SetActive(true);
            }
            if (GetState() == FeedingState)
            {
                int timekhiExitGame = Mathf.Abs(GameTime.TimeCurrent() - AnimalPersistence.GetAnimalLastTimestamp(penStableId, slotIndex, legacyAnimalName, legacyPenName));
                if (timekhiExitGame >= AnimalPersistence.GetAnimalRemainingTime(penStableId, slotIndex, legacyAnimalName, legacyPenName))
                {
                    remainingSeconds = 0;
                    SetState(ReadyState);
                    skeletonAnimation.AnimationName = ReadyAnimation;
                    transform.GetChild(0).gameObject.SetActive(true);
                }
                else
                {
                    remainingSeconds = AnimalPersistence.GetAnimalRemainingTime(penStableId, slotIndex, legacyAnimalName, legacyPenName) - timekhiExitGame;
                    skeletonAnimation.AnimationName = FeedingAnimation;
                    StartCoroutine(UpdateProductionCountdown());
                }
            }
        }
    }

    private void OnMouseUpAsButton()
    {
        if (!EventSystem.current.IsPointerOverGameObject(0))
        {
            MainCamera.Instance.DisableAll();
            if (GetState() == WaitingState)
            {
                AnimalFeedingPanel.instance.open(transform.parent.position, feedCropId);
                if (PlayerPrefs.GetInt("huongdan") == 22)
                {
                    TutorialController.instance.pointerHand.SetActive(false);
                    TutorialController.instance.animalFeedDragHand.SetActive(true);
                }
            }
            else
           if (GetState() == ReadyState)
            {
                Harvest();
            }
            else
               if (GetState() == FeedingState)
            {

                //hien thoi gian
                PlayerPrefs.SetString("duongdan", runtimeLookupKey);
                if (Application.systemLanguage == SystemLanguage.Vietnamese)
                    AnimalTimerPanel.instance.ShowTimer(transform.position, GameBootstrap.Instance.animalDatabase.animals[animalTypeId].vietnameseName, remainingSeconds, totalProductionSeconds);
                else
                    AnimalTimerPanel.instance.ShowTimer(transform.position, GameBootstrap.Instance.animalDatabase.animals[animalTypeId].englishName, remainingSeconds, totalProductionSeconds);
                AnimalTimerPanel.instance.gemCost = remainingSeconds / 60 + 1;
            }
            if (!isSoundOnCooldown)
            {
                isSoundOnCooldown = true;
                StartCoroutine(ResetSoundCooldown());
                if (animalTypeId == 0)
                    AudioManager.Instance.ga();
                if (animalTypeId == 1)
                    AudioManager.Instance.bo();
                if (animalTypeId == 2)
                    AudioManager.Instance.Duck();
                if (animalTypeId == 3)
                    AudioManager.Instance.lon();
                if (animalTypeId == 4)
                    AudioManager.Instance.cuu();
                if (animalTypeId == 5)
                    AudioManager.Instance.bufalo();
                if (animalTypeId == 6)
                    AudioManager.Instance.goat();
            }
        }
    }
    bool isSoundOnCooldown;
    IEnumerator ResetSoundCooldown()
    {
        yield return new WaitForSeconds(6f);
        isSoundOnCooldown = false;
    }
    public void Feed(int idta)
    {
        if (idta == feedCropId)
        {
            if (GetState() == WaitingState)
            {
                if (FarmingPlotPersistence.GetCropQuantity(feedCropId) > 0)
                {
                    FarmingPlotPersistence.SetCropQuantity(feedCropId, FarmingPlotPersistence.GetCropQuantity(feedCropId) - 1);
                    OrderPanel.instance.RefreshOrders();
                    StartFeeding();
                    InventoryController.instance.UpdateCropQuantity(feedCropId);
                }
                else
                {
                    if (transform.GetChild(1).gameObject.activeInHierarchy == false)
                    {
                        transform.GetChild(1).gameObject.SetActive(true);
                        if (!PlayerPrefs.HasKey("hetthucan"))
                            PlayerPrefs.SetInt("hetthucan", 0);
                        PlayerPrefs.SetString("duongdanvatnuoi" + PlayerPrefs.GetInt("hetthucan"), runtimeLookupKey);
                        PlayerPrefs.SetInt("hetthucan", PlayerPrefs.GetInt("hetthucan") + 1);
                    }
                }
            }
            else
            if (GetState() == ReadyState)
            {
                NotificationController.Instance.ShowWorldNotification("Bạn chưa thu hoạch!", "You have not harvested yet!", transform.position);
            }
            else
                if (GetState() == FeedingState)
            {
                NotificationController.Instance.ShowWorldNotification("Vật nuôi đang ăn!", "Pets are eating!", transform.position);

            }
        }
    }
    public void StartFeeding()
    {
        GameObject obj = Instantiate(feedingEffectPrefab, transform.position, Quaternion.identity);
        obj.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = GameBootstrap.Instance.cropDatabase.crops[feedCropId].productIcon;
        Destroy(obj, 1f);
        SetState(FeedingState);
        InventoryController.instance.UpdateCropQuantity(feedCropId);
        remainingSeconds = totalProductionSeconds;
        skeletonAnimation.AnimationName = FeedingAnimation;
        StartCoroutine(UpdateProductionCountdown());
    }
    public void Harvest()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        skeletonAnimation.AnimationName = IdleAnimation;
        SetState(WaitingState);
        GameObject ob1 = Instantiate(productRewardPrefab, transform.position, Quaternion.identity);
        ob1.GetComponent<RewardFlyout>().id = 1;
        ob1.transform.GetChild(0).GetComponent<Image>().sprite = GameBootstrap.Instance.animalDatabase.animals[animalTypeId].productIcon;
        ob1.GetComponent<RewardFlyout>().numberCoin = GameBootstrap.Instance.animalDatabase.animals[animalTypeId].productYield;
        GameObject ob2 = Instantiate(experienceRewardPrefab, transform.position, Quaternion.identity);
        ob2.GetComponent<RewardFlyout>().id = 2;
        ob2.GetComponent<RewardFlyout>().numberCoin = GameBootstrap.Instance.animalDatabase.animals[animalTypeId].experienceReward;
        //transform.GetChild(0).GetComponent<Animator>().enabled = false;
        PlayerPrefs.SetInt("slvatpham" + (animalTypeId + 42).ToString(), PlayerPrefs.GetInt("slvatpham" + (animalTypeId + 42).ToString()) + GameBootstrap.Instance.animalDatabase.animals[animalTypeId].productYield);
        OrderPanel.instance.RefreshOrders();
        InventoryController.instance.UpdateProductQuantity(animalTypeId + 42);
        GameObject ob3 = Instantiate(harvestEffectPrefab, transform.position, Quaternion.identity);
        Destroy(ob3, 2f);
        AudioManager.Instance.Harvest();
    }
    public void UseDiamonds()
    {
        remainingSeconds = 0;
    }
    public void HideStatusIcon()
    {
        transform.GetChild(1).gameObject.SetActive(false);
    }
    IEnumerator UpdateProductionCountdown()
    {
        if (PlayerPrefs.GetString("duongdan") == runtimeLookupKey)
        {
            if (Application.systemLanguage == SystemLanguage.Vietnamese)
                AnimalTimerPanel.instance.ShowTimer(transform.position, GameBootstrap.Instance.animalDatabase.animals[animalTypeId].vietnameseName, remainingSeconds, totalProductionSeconds);
            else
                AnimalTimerPanel.instance.ShowTimer(transform.position, GameBootstrap.Instance.animalDatabase.animals[animalTypeId].englishName, remainingSeconds, totalProductionSeconds);
            AnimalTimerPanel.instance.gemCost = remainingSeconds / 60 + 1;
        }
        AnimalPersistence.SetAnimalRemainingTime(penStableId, slotIndex, legacyAnimalName, legacyPenName, remainingSeconds);
        AnimalPersistence.SetAnimalLastTimestamp(penStableId, slotIndex, legacyAnimalName, legacyPenName, GameTime.TimeCurrent());
        yield return new WaitForSeconds(1f);
        remainingSeconds -= 1;
        if (remainingSeconds > 0)
        {
            StartCoroutine(UpdateProductionCountdown());
        }
        else
        {
            SetState(ReadyState);
            skeletonAnimation.AnimationName = ReadyAnimation;
            transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    private string GetState() => AnimalPersistence.GetAnimalState(
        penStableId, slotIndex, legacyAnimalName, legacyPenName);

    private void SetState(string value) => AnimalPersistence.SetAnimalState(
        penStableId, slotIndex, legacyAnimalName, legacyPenName, value);

    private void EnsureIdentity()
    {
        if (!string.IsNullOrEmpty(penStableId)) return;
        var pen = transform.parent.GetComponent<AnimalPen>();
        var identity = transform.parent.GetComponent<StableInstanceId>() ?? transform.parent.gameObject.AddComponent<StableInstanceId>();
        if (!identity.TryAssign(AnimalPersistence.DefaultPenStableId(pen.animalTypeId, pen.instanceIndex)))
            throw new System.InvalidOperationException($"Cannot assign stable ID for animal pen {pen.animalTypeId}:{pen.instanceIndex}.");
        penStableId = identity.StableId;
        legacyPenName = AnimalPersistence.DefaultLegacyPenName(pen.animalTypeId, pen.instanceIndex);
        slotIndex = transform.GetSiblingIndex() - 1;
        legacyAnimalName = AnimalPersistence.DefaultLegacyAnimalName(pen.animalTypeId, slotIndex);
        runtimeLookupKey = penStableId + ":" + slotIndex;
        RuntimeAnimals[runtimeLookupKey] = this;
        RuntimeAnimals[legacyAnimalName] = this;
    }

    public static FarmAnimal FindRuntimeAnimal(string lookupKey)
    {
        if (RuntimeAnimals.TryGetValue(lookupKey, out var animal) && animal != null)
            return animal;

        return null;
    }

    private void OnDestroy()
    {
        if (!string.IsNullOrEmpty(runtimeLookupKey) && RuntimeAnimals.TryGetValue(runtimeLookupKey, out var animal) && animal == this)
            RuntimeAnimals.Remove(runtimeLookupKey);
        if (!string.IsNullOrEmpty(legacyAnimalName) && RuntimeAnimals.TryGetValue(legacyAnimalName, out animal) && animal == this)
            RuntimeAnimals.Remove(legacyAnimalName);
    }
}
