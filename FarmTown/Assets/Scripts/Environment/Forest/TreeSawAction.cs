using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
public class TreeSawAction : MonoBehaviour
{
    [FormerlySerializedAs("hieuung")] [SerializeField] GameObject effectPrefab;
    [FormerlySerializedAs("exp")] [SerializeField] GameObject rewardPrefab;
    [FormerlySerializedAs("dungcua")] [SerializeField] GameObject toolUsePrefab;
    [FormerlySerializedAs("DialogDungKc")] [SerializeField] GameObject gemPurchaseDialog;
    Animator animator;
    [FormerlySerializedAs("check")] public bool isBeingCleared;
    [FormerlySerializedAs("iconcua")] [SerializeField] Sprite sawIcon;
    private void Awake()
    {
        if (PlayerPrefs.HasKey("dahuy" + gameObject.name))
        {
            Destroy(gameObject);
        }
        Vector3 temp = transform.TransformDirection(transform.position);
        gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt((13 - temp.y) * 100f);
        gameObject.transform.GetChild(1).GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt((13 - temp.y) * 100f) + 1;

    }
    void Start()
    {

        animator = transform.GetChild(1).GetComponent<Animator>();

    }
    private void OnMouseUpAsButton()
    {
        if (!EventSystem.current.IsPointerOverGameObject(0))
        {
            if (isBeingCleared == false)
            {
                if (PlayerPrefs.HasKey("huongdan"))
                {
                    if (PlayerPrefs.GetInt("huongdan") == 14)
                    {
                        ShowToolPanel();
                        TutorialController.instance.pointerHand.SetActive(false);
                        TutorialController.instance.downwardDragHand.SetActive(true);
                        TutorialController.instance.MoveDownwardDragHand(new Vector2(transform.position.x, transform.position.y + 2f));
                        TutorialController.instance.ShowWorldMessage("Kéo icon cưa xuống gốc", "Pull the saw icon down to the root");
                    }
                }
                else
                {
                    MainCamera.Instance.DisableAll();
                    ShowToolPanel();

                }
            }
        }
    }
    public void ShowToolPanel()
    {
        EnvironmentToolPanel.instance.Open(transform.position, 0);
        PlayerPrefs.SetString("choncayrung", gameObject.name);
    }
    IEnumerator RunSawSequence()
    {
        GameObject toolUse = Instantiate(toolUsePrefab, transform.position, Quaternion.identity);
        toolUse.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = sawIcon;
        yield return new WaitForSeconds(1f);
        Destroy(toolUse);
        GameObject effect = Instantiate(effectPrefab, transform.position, Quaternion.identity);
        AudioManager.Instance.PlaySawSound();
        yield return new WaitForSeconds(4f);
        Destroy(effect);
        animator.SetBool("cua", true);
        yield return new WaitForSeconds(2f);
        GameObject reward = Instantiate(rewardPrefab, transform.position, Quaternion.identity);
        reward.GetComponent<RewardFlyout>().id = 2;
        reward.GetComponent<RewardFlyout>().numberCoin = 3;
        if (PlayerPrefs.HasKey("huongdan"))
        {
            GameBootstrap.Instance.AddExperience(PlayerPrefs.GetInt("kinhnghiemmax") - PlayerPrefs.GetInt("kinhnghiem"));
        }
        Destroy(gameObject);

    }
    private void OnTriggerEnter2D(Collider2D target)
    {
        if (target.tag == "cua")
        {
            if (PlayerPrefs.GetInt("dangdichuyen") == 0)
            {
                if (PlayerPrefs.GetString("choncayrung") == gameObject.name)
                {
                    if (isBeingCleared == false)
                    {
                        if (PlayerPrefs.GetInt("slvatpham49") > 0)
                        {
                            PlayerPrefs.SetInt("slvatpham49", PlayerPrefs.GetInt("slvatpham49") - 1);
                            Clear();
                            if (PlayerPrefs.GetInt("huongdan") == 14)
                            {
                                PlayerPrefs.SetInt("huongdan", 15);
                                TutorialController.instance.downwardDragHand.SetActive(false);
                            }
                        }
                        else
                        {
                            gemPurchaseDialog.SetActive(true);
                            gemPurchaseDialog.transform.GetChild(1).GetChild(3).GetComponent<Button>().onClick.RemoveAllListeners();
                            gemPurchaseDialog.transform.GetChild(1).GetChild(1).GetComponent<Image>().sprite = sawIcon;
                            gemPurchaseDialog.transform.GetChild(1).GetChild(1).GetComponent<Image>().SetNativeSize();
                            gemPurchaseDialog.transform.GetChild(1).GetChild(3).GetComponent<Button>().onClick.AddListener(ClearWithGems);
                            gemPurchaseDialog.transform.GetChild(1).GetChild(4).GetComponent<Button>().onClick.AddListener(CloseDialog);

                        }

                    }
                }
            }
        }
    }
    public void Clear()
    {
        PlayerPrefs.SetInt("dahuy" + gameObject.name, 1);
        isBeingCleared = true;
        StartCoroutine(RunSawSequence());
        InventoryController.instance.UpdateProductQuantity(49);
    }
    public void ClearWithGems()
    {
        if (PlayerPrefs.GetInt("diamond") >= 5)
        {
            gemPurchaseDialog.SetActive(false);
            GameBootstrap.Instance.AddDiamonds(-5);
            Clear();

        }
        else
        {
            NotificationController.Instance.ShowCanvasNotification("Bạn không đủ kim cương!", "You 're not enough diamonds!");
        }
        AudioManager.Instance.click();
    }
    public void CloseDialog()
    {

        gemPurchaseDialog.SetActive(false);
        AudioManager.Instance.click();
    }
}
