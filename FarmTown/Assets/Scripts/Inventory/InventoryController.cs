using FarmTown.Save;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
public class InventoryController : MonoBehaviour
{
    public static InventoryController instance;
    [FormerlySerializedAs("anim"), SerializeField] private Animator animator;
    [FormerlySerializedAs("ScrollVP"), SerializeField] private GameObject productScroll;
    [FormerlySerializedAs("ScrollNS"), SerializeField] private GameObject cropScroll;
    [FormerlySerializedAs("itemns"), SerializeField] private GameObject cropItemPrefab;
    [FormerlySerializedAs("itemvp"), SerializeField] private GameObject productItemPrefab;
    [FormerlySerializedAs("contentns"), SerializeField] private Transform cropContent;
    [FormerlySerializedAs("contentvp"), SerializeField] private Transform productContent;
    [FormerlySerializedAs("btnNSOpen"), SerializeField] private Sprite cropTabOpenSprite;
    [FormerlySerializedAs("btnNSClose"), SerializeField] private Sprite cropTabClosedSprite;
    [FormerlySerializedAs("btnVPOpen"), SerializeField] private Sprite productTabOpenSprite;
    [FormerlySerializedAs("btnVPClose"), SerializeField] private Sprite productTabClosedSprite;
    [FormerlySerializedAs("btnNS"), SerializeField] private Image cropTabImage;
    [FormerlySerializedAs("btnVP"), SerializeField] private Image productTabImage;
    [FormerlySerializedAs("iconVang"), SerializeField] private GameObject coinFlyoutPrefab;
    [FormerlySerializedAs("hieuung"), SerializeField] private GameObject saleEffectPrefab;
    public enum ProductCategory
    {
        Crop = 0,
        Product = 1
    }
    [System.Serializable]
    public struct ProductionIngredient
    {
        [FormerlySerializedAs("loai")] public ProductCategory category;
        [FormerlySerializedAs("id")] public int itemId;
        [FormerlySerializedAs("soluong")] public int quantity;

    }
    [System.Serializable]
    public struct ProductDefinition
    {
        [FormerlySerializedAs("id")] public int itemId;
        [FormerlySerializedAs("ten")] public string vietnameseName;
        [FormerlySerializedAs("name")] public string englishName;
        [FormerlySerializedAs("giamua")] public int purchasePrice;
        [FormerlySerializedAs("giaban")] public int salePrice;
        [FormerlySerializedAs("capmo")] public int unlockLevel;
        [FormerlySerializedAs("time")] public int productionSeconds;
        [FormerlySerializedAs("exp")] public int experienceReward;
        [FormerlySerializedAs("icon")] public Sprite icon;
        [FormerlySerializedAs("SoNl")] public ProductionIngredient[] ingredients;
    }
    [FormerlySerializedAs("mangVP")] public ProductDefinition[] products;
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {

    // Crops.
        for (int i = 0; i < 22; i++)
        {
            GameObject ob = Instantiate(cropItemPrefab, cropContent);
            ob.name = "item" + i;
            if (Application.systemLanguage == SystemLanguage.Vietnamese)
            {
                ob.transform.GetChild(0).GetComponent<Text>().text = GameBootstrap.Instance.cropDatabase.crops[i].vietnameseProductName;
            }
            else
            {
                ob.transform.GetChild(0).GetComponent<Text>().text = GameBootstrap.Instance.cropDatabase.crops[i].englishProductName;
            }
            ob.transform.GetChild(1).GetComponent<Image>().sprite = GameBootstrap.Instance.cropDatabase.crops[i].productIcon;
            ob.transform.GetChild(1).GetComponent<Image>().SetNativeSize();
            ob.transform.GetChild(2).GetComponent<Text>().text = "x" + FarmingPlotPersistence.GetCropQuantity(i).ToString();
            ob.transform.GetChild(3).GetComponent<Text>().text = (FarmingPlotPersistence.GetCropQuantity(i) * GameBootstrap.Instance.cropDatabase.crops[i].salePrice).ToString();
            int id = i;
            ob.transform.GetChild(4).GetComponent<Button>().onClick.AddListener(() => SellCrop(id));
            if (FarmingPlotPersistence.GetCropQuantity(i) < 1)
            {
                ob.SetActive(false);
            }
        }
        Destroy(cropItemPrefab);

        for (int i = 0; i < products.Length; i++)
        {
            GameObject ob = Instantiate(productItemPrefab, productContent);
            ob.name = "item" + i;
            if (Application.systemLanguage == SystemLanguage.Vietnamese)
            {
                ob.transform.GetChild(0).GetComponent<Text>().text = products[i].vietnameseName;
            }
            else
            {
                ob.transform.GetChild(0).GetComponent<Text>().text = products[i].englishName;
            }
            ob.transform.GetChild(1).GetComponent<Image>().sprite = products[i].icon;
            ob.transform.GetChild(1).GetComponent<Image>().SetNativeSize();
            ob.transform.GetChild(2).GetComponent<Text>().text = "x" + PlayerPrefs.GetInt("slvatpham" + i).ToString();
            ob.transform.GetChild(3).GetComponent<Text>().text = (PlayerPrefs.GetInt("slvatpham" + i) * products[i].salePrice).ToString();
            int id = i;
            ob.transform.GetChild(4).GetComponent<Button>().onClick.AddListener(() => SellProduct(id));
            if (PlayerPrefs.GetInt("slvatpham" + i) < 1)
            {
                ob.SetActive(false);
            }
        }
        Destroy(productItemPrefab);
    }
    public void UpdateCropQuantity(int i)
    {
        if (cropContent.GetChild(i).gameObject.activeInHierarchy == false)
            cropContent.GetChild(i).gameObject.SetActive(true);
        cropContent.GetChild(i).GetChild(2).GetComponent<Text>().text = "x" + FarmingPlotPersistence.GetCropQuantity(i).ToString();
        cropContent.GetChild(i).GetChild(3).GetComponent<Text>().text = (FarmingPlotPersistence.GetCropQuantity(i) * GameBootstrap.Instance.cropDatabase.crops[i].salePrice).ToString();
        if (FarmingPlotPersistence.GetCropQuantity(i) == 0)
            cropContent.GetChild(i).gameObject.SetActive(false);
    }
    public void UpdateProductQuantity(int i)
    {
        if (productContent.GetChild(i).gameObject.activeInHierarchy == false)
            productContent.GetChild(i).gameObject.SetActive(true);
        productContent.GetChild(i).GetChild(2).GetComponent<Text>().text = "x" + PlayerPrefs.GetInt("slvatpham" + i).ToString();
        productContent.GetChild(i).GetChild(3).GetComponent<Text>().text = (PlayerPrefs.GetInt("slvatpham" + i) * products[i].salePrice).ToString();
        if (PlayerPrefs.GetInt("slvatpham" + i) == 0)
        {
            productContent.GetChild(i).gameObject.SetActive(false);
        }
    }
    public void SellCrop(int id)
    {
        Vector2 target = Camera.main.transform.position;
        GameObject ob = Instantiate(coinFlyoutPrefab, target, Quaternion.identity);
        ob.GetComponent<RewardFlyout>().id = 3;
        ob.GetComponent<RewardFlyout>().numberCoin = FarmingPlotPersistence.GetCropQuantity(id) * GameBootstrap.Instance.cropDatabase.crops[id].salePrice;
        GameBootstrap.Instance.AddGold(FarmingPlotPersistence.GetCropQuantity(id) * GameBootstrap.Instance.cropDatabase.crops[id].salePrice);
        FarmingPlotPersistence.SetCropQuantity(id, FarmingPlotPersistence.GetCropQuantity(id) - FarmingPlotPersistence.GetCropQuantity(id));
        cropScroll.transform.GetChild(0).GetChild(0).GetChild(id).gameObject.SetActive(false);
        OrderPanel.instance.RefreshOrders();
        AudioManager.Instance.Harvest();
        Vector2 postay = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        GameObject ob2 = Instantiate(saleEffectPrefab, target, Quaternion.identity);
        Destroy(ob2, 2f);
    }
    public void SellProduct(int id)
    {
        Vector2 target = Camera.main.transform.position;
        GameObject ob = Instantiate(coinFlyoutPrefab, target, Quaternion.identity);
        ob.GetComponent<RewardFlyout>().id = 3;
        ob.GetComponent<RewardFlyout>().numberCoin = PlayerPrefs.GetInt("slvatpham" + id) * products[id].salePrice;
        GameBootstrap.Instance.AddGold(PlayerPrefs.GetInt("slvatpham" + id) * products[id].salePrice);
        PlayerPrefs.SetInt("slvatpham" + id, PlayerPrefs.GetInt("slvatpham" + id) - PlayerPrefs.GetInt("slvatpham" + id));
        productScroll.transform.GetChild(0).GetChild(0).GetChild(id).gameObject.SetActive(false);
        OrderPanel.instance.RefreshOrders();
        AudioManager.Instance.Harvest();
        Vector2 postay = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        GameObject ob2 = Instantiate(saleEffectPrefab, target, Quaternion.identity);
        Destroy(ob2, 2f);
    }
    public void Open()
    {
        if (animator.enabled == false)
            animator.enabled = true;
        animator.SetInteger("dialog", 2);
        DialogShop.instance.close();
        MainCamera.Instance.CloseSmallDialogs();
        AudioManager.Instance.click();
    }
    public void ShowCrops()
    {
        cropScroll.transform.localScale = Vector3.one;
        productScroll.transform.localScale = Vector3.zero;
        cropTabImage.sprite = cropTabOpenSprite;
        productTabImage.sprite = productTabClosedSprite;
        AudioManager.Instance.click();
    }
    public void ShowProducts()
    {
        cropScroll.transform.localScale = Vector3.zero;
        productScroll.transform.localScale = Vector3.one;
        cropTabImage.sprite = cropTabClosedSprite;
        productTabImage.sprite = productTabOpenSprite;
        AudioManager.Instance.click();
    }
    public void Close()
    {
        animator.SetInteger("dialog", 1);
    }
}
