using System.Collections;
using System.Collections.Generic;
using FarmTown.Save;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
public class GameBootstrap : MonoBehaviour
{
    public static GameBootstrap Instance { get; private set; }
    [FormerlySerializedAs("ct")] public BuildingDatabase buildingDatabase;
    [FormerlySerializedAs("vn")] public AnimalDatabase animalDatabase;
    [FormerlySerializedAs("tt")] public DecorationItemDatabase decorationItemDatabase;
    [FormerlySerializedAs("ns")] public CropDatabase cropDatabase;
    [FormerlySerializedAs("txtgold"), SerializeField] private Text goldText;
    [FormerlySerializedAs("txtdiamond"), SerializeField] private Text diamondText;
    [FormerlySerializedAs("txtexp"), SerializeField] private Text experienceText;
    [FormerlySerializedAs("txtlv"), SerializeField] private Text levelText;
    [FormerlySerializedAs("dialoglv"), SerializeField] private GameObject levelUpDialog;
    [FormerlySerializedAs("imgExp"), SerializeField] private Image experienceProgress;
    [FormerlySerializedAs("listVpNew"), SerializeField] private GameObject levelRewardList;
    [SerializeField] private Transform inventoryRewardTarget;
    [SerializeField] private Transform produceRewardTarget;
    [SerializeField] private Transform experienceRewardTarget;
    [SerializeField] private Transform goldRewardTarget;
    [SerializeField] private Transform diamondRewardTarget;
    [SerializeField] private Transform tutorialForestTreeTarget;
    [SerializeField] private Transform orderTarget;
    [SerializeField] private Transform rewardTruckTarget;

    public Transform InventoryRewardTarget => inventoryRewardTarget;
    public Transform ProduceRewardTarget => produceRewardTarget;
    public Transform ExperienceRewardTarget => experienceRewardTarget;
    public Transform GoldRewardTarget => goldRewardTarget;
    public Transform DiamondRewardTarget => diamondRewardTarget;
    public Transform TutorialForestTreeTarget => tutorialForestTreeTarget;
    public Transform OrderTarget => orderTarget;
    public Transform RewardTruckTarget => rewardTruckTarget;

    [System.Serializable]
    public struct LevelRewardDefinition
    {
        [FormerlySerializedAs("slnsthem")] public int bonusCropQuantity;
        [FormerlySerializedAs("icon")] public Sprite[] icons;
    }
    [FormerlySerializedAs("mangNew")] public LevelRewardDefinition[] levelRewards;
    [System.Serializable]
    public struct CropPlotPosition
    {
        public float x, y;
    }
    [FormerlySerializedAs("vtNew")] public CropPlotPosition[] defaultCropPlotPositions;
    [FormerlySerializedAs("odat"), SerializeField] GameObject cropPlotPrefab;
    void Awake()
    {
        new SaveMigrationService(new PlayerPrefsStore()).MigrateToLatest();

        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
        //PlayerPrefs.DeleteAll();
        if (!PlayerPrefs.HasKey("gold"))
        {
            PlayerPrefs.SetInt("gold", 1000);
            PlayerPrefs.SetInt("diamond", 20);
        }
        if (!PlayerPrefs.HasKey("level"))
        {
            PlayerPrefs.SetInt("level", 1);
            PlayerPrefs.SetInt("music", 1);
            PlayerPrefs.SetInt("sound", 1);
           
            //PlayerPrefs.SetInt("soodat", 0);
            PlayerPrefs.SetInt("soodatmax", 9);
            PlayerPrefs.SetInt("capmoodat", 1);
            PlayerPrefs.SetInt("giamuaodat", 10);
            PlayerPrefs.SetInt("kinhnghiem", 0);
            PlayerPrefs.SetInt("kinhnghiemmax", 9);
            PlayerPrefs.SetInt("huongdan", 1);
            for (int i = 0; i < 22; i++)
            {
                FarmingPlotPersistence.SetCropQuantity(i, 0);
                if (i == 0)
                    FarmingPlotPersistence.SetCropQuantity(i, 6);
            }
            for (int i = 0; i < 39; i++)
            {
                PlayerPrefs.SetInt("slvatpham" + i, 0);
            }
            PlayerPrefs.SetInt("slvatpham49", 2);
            //PlayerPrefs.SetInt("slvatpham33", 2);
            //PlayerPrefs.SetInt("slvatpham34", 2);
            for (int i = 0; i < animalDatabase.animals.Length; i++)
            {
                AnimalPersistence.SetPenQuantity(i, 0);
                AnimalPersistence.SetPenLimit(i, 1);
                AnimalPersistence.SetAnimalQuantity(i, 0);
                AnimalPersistence.SetAnimalLimit(i, 3);
                AnimalPersistence.SetPenPrice(i, i * 100 + 200);
                AnimalPersistence.SetAnimalPrice(i, i * 20 + 20);

            }
            for (int i = 0; i < 7; i++)
            {
                ProductionPersistence.SetBuildingCount(i, 0);
                PlayerPrefs.SetInt("slnhamaymax" + i, 1);
                PlayerPrefs.SetInt("gianhamay" + i, i * 100 + 150);
            }
            for (int i = 0; i < 7; i++)
            {
                PlayerPrefs.SetInt("sltrangtri" + i, 0);
                PlayerPrefs.SetInt("sltrangtrimax" + i, 3);
            }

        }
        if (!FarmingPlotPersistence.HasSavedPlotCount())
        {
            for (int i = 0; i < 6; i++)
            {
                GameObject ob = Instantiate(cropPlotPrefab, new Vector2(defaultCropPlotPositions[i].x, defaultCropPlotPositions[i].y), Quaternion.identity);
                var plotIndex = i;
                ConfigureCropPlot(ob, plotIndex, true);
                FarmingPlotPersistence.SetPlotCount(plotIndex + 1);
            }
        }
        else
        {
        // Restore crop plots.
            for (int i = 0; i < FarmingPlotPersistence.GetPlotCount(); i++)
            {
                GameObject ob = Instantiate(cropPlotPrefab, new Vector2(PlayerPrefs.HasKey(SaveKeys.PlotPositionX(FarmingPlotPersistence.DefaultStableId(i))) ? PlayerPrefs.GetFloat(SaveKeys.PlotPositionX(FarmingPlotPersistence.DefaultStableId(i))) : PlayerPrefs.GetFloat(LegacySaveKeys.PlotPositionX(i)), PlayerPrefs.HasKey(SaveKeys.PlotPositionY(FarmingPlotPersistence.DefaultStableId(i))) ? PlayerPrefs.GetFloat(SaveKeys.PlotPositionY(FarmingPlotPersistence.DefaultStableId(i))) : PlayerPrefs.GetFloat(LegacySaveKeys.PlotPositionY(i))), Quaternion.identity);
                ConfigureCropPlot(ob, i, false);
            }
        }
        PlayerPrefs.SetString(SaveKeys.SelectedPlot, string.Empty);
        PlayerPrefs.SetString(LegacySaveKeys.SelectedPlot, "a");
        goldText.text = PlayerPrefs.GetInt("gold").ToString();
        diamondText.text = PlayerPrefs.GetInt("diamond").ToString();
        levelText.text = PlayerPrefs.GetInt("level").ToString();
        experienceText.text = PlayerPrefs.GetInt("kinhnghiem") + "/" + PlayerPrefs.GetInt("kinhnghiemmax");
    }
    private static void ConfigureCropPlot(GameObject plot, int index, bool savePosition)
    {
        var identity = plot.GetComponent<StableInstanceId>() ?? plot.AddComponent<StableInstanceId>();
        if (!identity.TryAssign(FarmingPlotPersistence.DefaultStableId(index)))
        {
            Destroy(plot);
            throw new System.InvalidOperationException($"Cannot assign stable ID for crop plot {index}.");
        }
        plot.name = $"CropPlot-{index}";
        if (!savePosition) return;
        PlayerPrefs.SetFloat(SaveKeys.PlotPositionX(identity.StableId), plot.transform.position.x);
        PlayerPrefs.SetFloat(SaveKeys.PlotPositionY(identity.StableId), plot.transform.position.y);
        PlayerPrefs.SetFloat(LegacySaveKeys.PlotPositionX(index), plot.transform.position.x);
        PlayerPrefs.SetFloat(LegacySaveKeys.PlotPositionY(index), plot.transform.position.y);
    }

    void Start()
    {

        // Restore production buildings.
        for (int i = 0; i < buildingDatabase.buildings.Length; i++)
        {
            if (ProductionPersistence.GetBuildingCount(i) > 0)
            {
                for (int j = 0; j < ProductionPersistence.GetBuildingCount(i); j++)
                {
                    var stableId = ProductionPersistence.DefaultStableId(i, j);
                    var legacyName = ProductionPersistence.DefaultLegacyName(i, j);
                    Vector2 pos = new Vector2(ProductionPersistence.GetPositionX(stableId, legacyName), ProductionPersistence.GetPositionY(stableId, legacyName));
                    GameObject ob = Instantiate(buildingDatabase.buildings[i].prefab, pos, Quaternion.identity);
                    ob.GetComponent<ProductionBuilding>().InitializeIdentity(i, j);
                    ob.name = legacyName;
                }
            }
        }
        // Restore decoration items.
        for (int i = 0; i < decorationItemDatabase.decorationItems.Length; i++)
        {
            if (PlayerPrefs.HasKey("sltrangtri" + i))
            {
                for (int j = 0; j < PlayerPrefs.GetInt("sltrangtri" + i); j++)
                {
                    Vector2 pos = new Vector2(PlayerPrefs.GetFloat("xtrangtri" + i + j), PlayerPrefs.GetFloat("ytrangtri" + i + j));
                    GameObject ob = Instantiate(decorationItemDatabase.decorationItems[i].prefab, pos, Quaternion.identity);
                ob.GetComponent<DecorationItem>().instanceIndex = j;
                    ob.GetComponent<DecorationItem>().id = i;
                    ob.name = "trangtri" + i + j;
                }
            }
        }
        // Restore animal pens.
        for (int i = 0; i < animalDatabase.animals.Length; i++)
        {
            if (PlayerPrefs.HasKey(SaveKeys.AnimalPenQuantity(i)) || PlayerPrefs.HasKey(LegacySaveKeys.AnimalPenQuantity(i)))
            {
                for (int j = 0; j < AnimalPersistence.GetPenQuantity(i); j++)
                {
                    var stableId = AnimalPersistence.DefaultPenStableId(i, j);
                    var legacyName = AnimalPersistence.DefaultLegacyPenName(i, j);
                    Vector2 pos = new Vector2(
                        AnimalPersistence.GetPenPositionX(stableId, legacyName),
                        AnimalPersistence.GetPenPositionY(stableId, legacyName));
                    GameObject ob = Instantiate(animalDatabase.animals[i].penPrefab, pos, Quaternion.identity);
                    ob.GetComponent<AnimalPen>().instanceIndex = j;
                    ConfigureAnimalPen(ob, i, j);
                }
            }
        }
    }

    private static void ConfigureAnimalPen(GameObject pen, int typeId, int instanceIndex)
    {
        var identity = pen.GetComponent<StableInstanceId>() ?? pen.AddComponent<StableInstanceId>();
        if (!identity.TryAssign(AnimalPersistence.DefaultPenStableId(typeId, instanceIndex)))
        {
            Destroy(pen);
            throw new System.InvalidOperationException($"Cannot assign stable ID for animal pen {typeId}:{instanceIndex}.");
        }
        pen.name = AnimalPersistence.DefaultLegacyPenName(typeId, instanceIndex);
    }
    public void AddGold(int amount)
    {
        PlayerPrefs.SetInt("gold", PlayerPrefs.GetInt("gold") + amount);
        goldText.text = PlayerPrefs.GetInt("gold").ToString();
    }
    public void AddDiamonds(int amount)
    {
        PlayerPrefs.SetInt("diamond", PlayerPrefs.GetInt("diamond") + amount);
        diamondText.text = PlayerPrefs.GetInt("diamond").ToString();
    }
    public void AddExperience(int exp)
    {
        PlayerPrefs.SetInt("kinhnghiem", PlayerPrefs.GetInt("kinhnghiem") + exp);
        if (PlayerPrefs.GetInt("kinhnghiem") >= PlayerPrefs.GetInt("kinhnghiemmax"))
        {
            PlayerPrefs.SetInt("level", PlayerPrefs.GetInt("level") + 1);
            PlayerPrefs.SetInt("kinhnghiem", PlayerPrefs.GetInt("kinhnghiem") - PlayerPrefs.GetInt("kinhnghiemmax"));
            if (PlayerPrefs.GetInt("level") < 5)
                PlayerPrefs.SetInt("kinhnghiemmax", PlayerPrefs.GetInt("kinhnghiemmax") + 40);
            else if (PlayerPrefs.GetInt("level") >= 5 && PlayerPrefs.GetInt("level") < 10)
                PlayerPrefs.SetInt("kinhnghiemmax", PlayerPrefs.GetInt("kinhnghiemmax") + 100);
            else if (PlayerPrefs.GetInt("level") >= 10 && PlayerPrefs.GetInt("level") < 15)
                PlayerPrefs.SetInt("kinhnghiemmax", PlayerPrefs.GetInt("kinhnghiemmax") + 200);
            else if (PlayerPrefs.GetInt("level") >= 15 && PlayerPrefs.GetInt("level") < 20)
                PlayerPrefs.SetInt("kinhnghiemmax", PlayerPrefs.GetInt("kinhnghiemmax") + 300);
            else if (PlayerPrefs.GetInt("level") >= 20 && PlayerPrefs.GetInt("level") < 30)
                PlayerPrefs.SetInt("kinhnghiemmax", PlayerPrefs.GetInt("kinhnghiemmax") + 400);
            else if (PlayerPrefs.GetInt("level") >= 30)
                PlayerPrefs.SetInt("kinhnghiemmax", PlayerPrefs.GetInt("kinhnghiemmax") + 500);
            ShowLevelUpDialog();

        }
        levelText.text = PlayerPrefs.GetInt("level").ToString();
        experienceText.text = PlayerPrefs.GetInt("kinhnghiem") + "/" + PlayerPrefs.GetInt("kinhnghiemmax");
        experienceProgress.fillAmount = (float)PlayerPrefs.GetInt("kinhnghiem") / PlayerPrefs.GetInt("kinhnghiemmax");
    }
    public void ShowLevelUpDialog()
    {
        if (GameObject.FindGameObjectWithTag("drag") != null)
            Destroy(GameObject.FindGameObjectWithTag("drag"));
        if (GameObject.FindGameObjectWithTag("liem") != null)
            Destroy(GameObject.FindGameObjectWithTag("liem"));
        MainCamera.Instance.DisableAll();
        MobileFullVideo.instance.ShowFullNormal();
        AudioManager.Instance.lenlevel();
        if (MainCamera.Instance.camLock == false)
            MainCamera.Instance.lockCam();
        OrderPanel.instance.UpdateOrderAvailability();
        levelUpDialog.SetActive(true);
        if (Application.systemLanguage == SystemLanguage.Vietnamese)
            levelUpDialog.transform.GetChild(1).GetChild(3).GetComponent<Text>().text = "Lên cấp " + PlayerPrefs.GetInt("level");
        else
            levelUpDialog.transform.GetChild(1).GetChild(3).GetComponent<Text>().text = "Level up " + PlayerPrefs.GetInt("level");
        AddGold(100);
        AddDiamonds(5);
        PopulateLevelRewards();
    }
    public void CloseDialog()
    {
        if (GameObject.FindGameObjectWithTag("drag") != null)
            Destroy(GameObject.FindGameObjectWithTag("drag"));
        if (GameObject.FindGameObjectWithTag("liem") != null)
            Destroy(GameObject.FindGameObjectWithTag("liem"));
        if (GameObject.FindGameObjectWithTag("cua") != null)
            Destroy(GameObject.FindGameObjectWithTag("cua"));
        levelUpDialog.SetActive(false);
        MainCamera.Instance.unLockCam();
        for (int i = 0; i < 5; i++)
        {
            levelRewardList.transform.GetChild(i).localScale = Vector3.zero;
        }
        if (PlayerPrefs.HasKey("huongdan"))
        {
            TutorialController.instance.shopPointerHand.SetActive(true);
            TutorialController.instance.shopHighlight.SetActive(true);
            TutorialController.instance.ShowWorldMessage("Nhấn vào icon cửa hàng", "Click on the store icon");
        }
        if (PlayerPrefs.GetInt("level") != 2)
            SceneLoader.Instance.Load(2);
    }
    public void PopulateLevelRewards()
    {
        int i = PlayerPrefs.GetInt("level");
        if (levelRewards[i].bonusCropQuantity > 0)
        {
            levelRewardList.transform.GetChild(0).GetChild(0).localScale = Vector3.one;
        }
        else
        {
            levelRewardList.transform.GetChild(0).GetChild(0).localScale = Vector3.zero;
        }
        for (int j = 0; j < levelRewards[i].icons.Length; j++)
        {
            levelRewardList.transform.GetChild(j).localScale = Vector3.one;
            levelRewardList.transform.GetChild(j).GetComponent<Image>().sprite = levelRewards[i].icons[j];
            levelRewardList.transform.GetChild(j).GetComponent<Image>().SetNativeSize();
        }
        switch (i)
        {
            case 2:
                FarmingPlotPersistence.SetCropQuantity(1, 6);
                InventoryController.instance.UpdateCropQuantity(1);
                ProductionBuildingShopList.instance.checkIcon(0);
                AnimalPenShopList.instance.checkIcon(1);
                AnimalShopList.instance.checkIcon(0);
                AnimalPenShopList.instance.RefreshQuantity(1);
                break;
            case 3:
                FarmingPlotPersistence.SetCropQuantity(2, 6);
                ProductionBuildingShopList.instance.checkIcon(1);
                PlayerPrefs.SetInt("slvatpham49", PlayerPrefs.GetInt("slvatpham49") + 1);
                InventoryController.instance.UpdateProductQuantity(49);
                InventoryController.instance.UpdateCropQuantity(2);
                break;
            case 4:
                PlayerPrefs.SetInt("soodatmax", 12);
                PlayerPrefs.SetInt("capmoodat", 6);
                AnimalPenShopList.instance.RefreshQuantity(0);
                DecorationItemShopList.instance.check(0);
                PlayerPrefs.SetInt("slvatpham51", PlayerPrefs.GetInt("slvatpham51") + 1);
                InventoryController.instance.UpdateProductQuantity(51);
                break;
            case 5:
                ProductionBuildingShopList.instance.checkIcon(2);
                AnimalPenShopList.instance.RefreshQuantity(2);
                AnimalPenShopList.instance.checkIcon(2);
                AnimalShopList.instance.checkIcon(1);
                break;
            case 6:
                FarmingPlotPersistence.SetCropQuantity(3, 6);
                PlayerPrefs.SetInt("capmoodat", 9);
                InventoryController.instance.UpdateCropQuantity(2);
                PlayerPrefs.SetInt("soodatmax", 15);
                AnimalPenShopList.instance.RefreshQuantity(3);
                AnimalPenShopList.instance.RefreshQuantity(0);
                AnimalPenShopList.instance.checkIcon(3);
                AnimalShopList.instance.checkIcon(2);
                break;
            case 7:
                ProductionBuildingShopList.instance.checkIcon(3);
                DecorationItemShopList.instance.check(1);
                PlayerPrefs.SetInt("slvatpham50", PlayerPrefs.GetInt("slvatpham50") + 1);
                InventoryController.instance.UpdateProductQuantity(50);
                break;
            case 8:
                FarmingPlotPersistence.SetCropQuantity(4, 6);
                InventoryController.instance.UpdateCropQuantity(2);
                DecorationItemShopList.instance.check(2);
                break;
            case 9:
                AnimalPersistence.SetAnimalLimit(0, AnimalPersistence.GetAnimalLimit(0) + 3);
                AnimalPersistence.SetPenLimit(0, AnimalPersistence.GetPenLimit(0) + 1);
                AnimalPenShopList.instance.RefreshQuantity(1);
                AnimalShopList.instance.RefreshQuantity(0);
                PlayerPrefs.SetInt("soodatmax", 18);
                AnimalPenShopList.instance.RefreshQuantity(0);
                PlayerPrefs.SetInt("capmoodat", 12);
                AnimalPenShopList.instance.checkIcon(1);
                break;
            case 10:
                FarmingPlotPersistence.SetCropQuantity(5, 6);
                ProductionBuildingShopList.instance.checkIcon(4);
                InventoryController.instance.UpdateCropQuantity(2);
                DecorationItemShopList.instance.check(3);
                break;
            case 11:
                AnimalPersistence.SetAnimalLimit(1, AnimalPersistence.GetAnimalLimit(1) + 3);
                AnimalPersistence.SetPenLimit(1, AnimalPersistence.GetPenLimit(1) + 1);
                PlayerPrefs.SetInt("slnhamaymax0", PlayerPrefs.GetInt("slnhamaymax0") + 1);
                AnimalPenShopList.instance.RefreshQuantity(2);
                AnimalPenShopList.instance.checkIcon(2);
                AnimalShopList.instance.RefreshQuantity(1);
                ProductionBuildingShopList.instance.RefreshQuantity(0);
                PlayerPrefs.SetInt("slvatpham49", PlayerPrefs.GetInt("slvatpham49") + 1);
                InventoryController.instance.UpdateProductQuantity(49);
                break;
            case 12:
                PlayerPrefs.SetInt("soodatmax", 21);
                PlayerPrefs.SetInt("capmoodat", 17);
                AnimalPenShopList.instance.RefreshQuantity(0);
                FarmingPlotPersistence.SetCropQuantity(6, 6);
                InventoryController.instance.UpdateCropQuantity(2);
                DecorationItemShopList.instance.check(4);
                break;
            case 13:
                ProductionBuildingShopList.instance.checkIcon(5);
                AnimalPenShopList.instance.RefreshQuantity(4);
                AnimalPenShopList.instance.checkIcon(4);
                AnimalShopList.instance.checkIcon(3);
                break;
            case 14:
                AnimalPersistence.SetAnimalLimit(2, AnimalPersistence.GetAnimalLimit(2) + 3);
                AnimalPersistence.SetPenLimit(2, AnimalPersistence.GetPenLimit(2) + 1);
                AnimalPenShopList.instance.RefreshQuantity(3);
                AnimalShopList.instance.RefreshQuantity(2);

                FarmingPlotPersistence.SetCropQuantity(7, 6);
                InventoryController.instance.UpdateCropQuantity(2);
                DecorationItemShopList.instance.check(5);
                break;
            case 15:
                PlayerPrefs.SetInt("slnhamaymax1", PlayerPrefs.GetInt("slnhamaymax1") + 1);
                ProductionBuildingShopList.instance.RefreshQuantity(1);
                DecorationItemShopList.instance.check(6);
                PlayerPrefs.SetInt("slvatpham51", PlayerPrefs.GetInt("slvatpham51") + 1);
                InventoryController.instance.UpdateProductQuantity(51);
                break;
            case 16:
                FarmingPlotPersistence.SetCropQuantity(8, 6);
                InventoryController.instance.UpdateCropQuantity(2);
                PlayerPrefs.SetInt("slvatpham50", PlayerPrefs.GetInt("slvatpham50") + 1);
                InventoryController.instance.UpdateProductQuantity(50);
                break;
            case 17:
                PlayerPrefs.SetInt("soodatmax", 24);
                PlayerPrefs.SetInt("capmoodat", 21);
                AnimalPenShopList.instance.RefreshQuantity(0);
                AnimalPenShopList.instance.RefreshQuantity(5);
                AnimalPenShopList.instance.checkIcon(5);
                AnimalShopList.instance.checkIcon(4);
                break;
            case 18:
                FarmingPlotPersistence.SetCropQuantity(9, 6);
                InventoryController.instance.UpdateCropQuantity(2);
                break;
            case 19:
                ProductionBuildingShopList.instance.checkIcon(6);
                DecorationItemShopList.instance.check(7);
                AnimalPenShopList.instance.RefreshQuantity(6);
                AnimalPenShopList.instance.checkIcon(6);
                AnimalShopList.instance.checkIcon(5);
                break;
            case 20:
                FarmingPlotPersistence.SetCropQuantity(10, 6);
                InventoryController.instance.UpdateCropQuantity(2);
                DecorationItemShopList.instance.check(8);
                break;
            case 21:
                AnimalPersistence.SetAnimalLimit(3, AnimalPersistence.GetAnimalLimit(3) + 3);
                AnimalPersistence.SetPenLimit(3, AnimalPersistence.GetPenLimit(3) + 1);
                AnimalPenShopList.instance.RefreshQuantity(4);
                AnimalShopList.instance.RefreshQuantity(3);
                PlayerPrefs.SetInt("soodatmax", 27);
                PlayerPrefs.SetInt("capmoodat", 28);
                AnimalPenShopList.instance.checkIcon(4);
                AnimalPenShopList.instance.RefreshQuantity(0);
                break;
            case 22:
                FarmingPlotPersistence.SetCropQuantity(11, 6);
                InventoryController.instance.UpdateCropQuantity(2);
                DecorationItemShopList.instance.check(9);
                break;
            case 23:
                PlayerPrefs.SetInt("slnhamaymax2", PlayerPrefs.GetInt("slnhamaymax2") + 1);
                ProductionBuildingShopList.instance.RefreshQuantity(2);
                break;
            case 24:
                FarmingPlotPersistence.SetCropQuantity(12, 6);
                InventoryController.instance.UpdateCropQuantity(2);
                DecorationItemShopList.instance.check(10);
                break;
            case 25:
                AnimalPenShopList.instance.RefreshQuantity(7);
                AnimalPenShopList.instance.checkIcon(7);
                AnimalShopList.instance.checkIcon(6);
                DecorationItemShopList.instance.check(11);
                break;
            case 26:
                FarmingPlotPersistence.SetCropQuantity(13, 6);
                InventoryController.instance.UpdateCropQuantity(2);
                break;
            case 27:
                DecorationItemShopList.instance.check(12);
                break;
            case 28:
                FarmingPlotPersistence.SetCropQuantity(14, 6);
                InventoryController.instance.UpdateCropQuantity(2);
                PlayerPrefs.SetInt("soodatmax", 30);
                PlayerPrefs.SetInt("capmoodat", 34);
                AnimalPenShopList.instance.RefreshQuantity(0);
                break;
            case 29:
                AnimalPersistence.SetAnimalLimit(5, AnimalPersistence.GetAnimalLimit(5) + 3);
                AnimalPersistence.SetPenLimit(5, AnimalPersistence.GetPenLimit(5) + 1);
                DecorationItemShopList.instance.check(13);
                AnimalPenShopList.instance.RefreshQuantity(6);
                AnimalPenShopList.instance.checkIcon(6);
                AnimalShopList.instance.checkIcon(5);
                AnimalShopList.instance.RefreshQuantity(5);
                break;
            case 30:
                FarmingPlotPersistence.SetCropQuantity(15, 6);
                InventoryController.instance.UpdateCropQuantity(2);
                break;
            case 31:
                DecorationItemShopList.instance.check(14);
                break;
            case 32:
                FarmingPlotPersistence.SetCropQuantity(16, 6);
                InventoryController.instance.UpdateCropQuantity(2);
                break;
            case 33:
                AnimalPersistence.SetAnimalLimit(4, AnimalPersistence.GetAnimalLimit(4) + 3);
                AnimalPersistence.SetPenLimit(4, AnimalPersistence.GetPenLimit(4) + 1);
                AnimalPenShopList.instance.RefreshQuantity(5);
                AnimalPenShopList.instance.checkIcon(5);
                AnimalShopList.instance.RefreshQuantity(4);
                break;
            case 34:
                FarmingPlotPersistence.SetCropQuantity(17, 6);
                InventoryController.instance.UpdateCropQuantity(2);
                PlayerPrefs.SetInt("soodatmax", 33);
                AnimalPenShopList.instance.RefreshQuantity(0);
                break;
            case 35:
                PlayerPrefs.SetInt("slnhamaymax3", PlayerPrefs.GetInt("slnhamaymax3") + 1);
                ProductionBuildingShopList.instance.RefreshQuantity(3);
                DecorationItemShopList.instance.check(15);
                break;
            case 36:
                FarmingPlotPersistence.SetCropQuantity(18, 6);
                InventoryController.instance.UpdateCropQuantity(2);
                break;
            case 37:
                PlayerPrefs.SetInt("slnhamaymax4", PlayerPrefs.GetInt("slnhamaymax4") + 1);
                ProductionBuildingShopList.instance.RefreshQuantity(4);
                break;
            case 38:
                FarmingPlotPersistence.SetCropQuantity(19, 6);
                InventoryController.instance.UpdateCropQuantity(2);
                DecorationItemShopList.instance.check(16);
                AnimalPersistence.SetAnimalLimit(6, AnimalPersistence.GetAnimalLimit(6) + 3);
                AnimalPersistence.SetPenLimit(6, AnimalPersistence.GetPenLimit(6) + 1);
                AnimalPenShopList.instance.RefreshQuantity(7);
                AnimalPenShopList.instance.checkIcon(7);
                AnimalShopList.instance.checkIcon(6);
                break;
            case 39:
                PlayerPrefs.SetInt("slnhamaymax5", PlayerPrefs.GetInt("slnhamaymax5") + 1);
                ProductionBuildingShopList.instance.RefreshQuantity(5);
                break;
            case 40:
                FarmingPlotPersistence.SetCropQuantity(20, 6);
                InventoryController.instance.UpdateCropQuantity(2);
                break;
            case 41:
                PlayerPrefs.SetInt("slnhamaymax6", PlayerPrefs.GetInt("slnhamaymax6") + 1);
                ProductionBuildingShopList.instance.RefreshQuantity(6);
                break;
            case 42:
                FarmingPlotPersistence.SetCropQuantity(21, 6);
                InventoryController.instance.UpdateCropQuantity(2);
                break;

        }

    }
    public void ClaimLevelReward()
    {
        AddExperience(PlayerPrefs.GetInt("kinhnghiemmax") - PlayerPrefs.GetInt("kinhnghiem"));
        AddGold(2000);
        AddDiamonds(100);
    }
}
