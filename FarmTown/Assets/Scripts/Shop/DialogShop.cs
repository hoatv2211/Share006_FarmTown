using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DialogShop : MonoBehaviour
{
    public static DialogShop instance;
    public GameObject dialogShop;
    [SerializeField] Image[] imgButton;
    [SerializeField] GameObject[] shop;
    [SerializeField] Sprite[] spropen, sprlock;
    private void Start()
    {
        instance = this;
        dialogShop = transform.GetChild(0).gameObject;
        SelectShopTab(0);
        
    }

    public void open()
    {
        if (!PlayerPrefs.HasKey("huongdan"))
        {
            if (dialogShop.GetComponent<Animator>().enabled == false)
                dialogShop.GetComponent<Animator>().enabled = true;
            dialogShop.GetComponent<Animator>().SetInteger("dialog", 2);
            SeedSelectionPanel.instance.close();
            //MainCamera.Instance.lockCam();
            MainCamera.Instance.CloseSmallDialogs();
        }
        else
        {
            if (PlayerPrefs.GetInt("huongdan") == 9)
                TutorialController.instance.ShowCanvasMessage("Kéo ô đất ra map", "Pull the field off the map");
            if (PlayerPrefs.GetInt("huongdan") == 9 || PlayerPrefs.GetInt("huongdan") == 10 || PlayerPrefs.GetInt("huongdan") == 11)
            {
                if (dialogShop.GetComponent<Animator>().enabled == false)
                    dialogShop.GetComponent<Animator>().enabled = true;
                dialogShop.GetComponent<Animator>().SetInteger("dialog", 2);
                MainCamera.Instance.CloseSmallDialogs();
                SelectShopTab(0);
                TutorialController.instance.shopPointerHand.SetActive(false);
                TutorialController.instance.shopHighlight.SetActive(false);
                TutorialController.instance.shopDragHand.SetActive(true);

            }
            if (PlayerPrefs.GetInt("huongdan") == 15 && PlayerPrefs.GetInt("level") == 2)
            {
                if (dialogShop.GetComponent<Animator>().enabled == false)
                    dialogShop.GetComponent<Animator>().enabled = true;
                dialogShop.GetComponent<Animator>().SetInteger("dialog", 2);
                MainCamera.Instance.CloseSmallDialogs();
                SelectShopTab(2);
                TutorialController.instance.shopPointerHand.SetActive(false);
                TutorialController.instance.shopHighlight.SetActive(false);
                TutorialController.instance.shopDragHand.SetActive(true);
                TutorialController.instance.ShowCanvasMessage("Kéo nhà bánh mì ra map", "Pull the bakery off the map");
            }
            if (PlayerPrefs.GetInt("huongdan") == 17)
            {
                if (dialogShop.GetComponent<Animator>().enabled == false)
                    dialogShop.GetComponent<Animator>().enabled = true;
                dialogShop.GetComponent<Animator>().SetInteger("dialog", 2);
                MainCamera.Instance.CloseSmallDialogs();
                SelectShopTab(0);
                TutorialController.instance.shopPointerHand.SetActive(false);
                TutorialController.instance.shopHighlight.SetActive(false);
                TutorialController.instance.penDragHand.SetActive(true);
                TutorialController.instance.ShowCanvasMessage("Kéo chuồng gà ra map", "Pull the chicken coop to the map");
            }
            if (PlayerPrefs.GetInt("huongdan") == 18)
            {
                if (dialogShop.GetComponent<Animator>().enabled == false)
                    dialogShop.GetComponent<Animator>().enabled = true;
                dialogShop.GetComponent<Animator>().SetInteger("dialog", 2);
                // dialogHG.instance.close();
                //MainCamera.Instance.CloseSmallDialogs();
                SelectShopTab(0);
            }
            if (PlayerPrefs.GetInt("huongdan") >= 19 && PlayerPrefs.GetInt("huongdan") < 22)
            {
                if (dialogShop.GetComponent<Animator>().enabled == false)
                    dialogShop.GetComponent<Animator>().enabled = true;
                dialogShop.GetComponent<Animator>().SetInteger("dialog", 2);
                //dialogHG.instance.close();
                //MainCamera.Instance.CloseSmallDialogs();
                SelectShopTab(1);
            }

        }
        AudioManager.Instance.click();
    }
    public void close()
    {
        if (!PlayerPrefs.HasKey("huongdan"))
            dialogShop.GetComponent<Animator>().SetInteger("dialog", 1);
        //MainCamera.Instance.unLockCam();
    }
    public void SelectShopTab(int i)
    {
        if (!PlayerPrefs.HasKey("huongdan"))
        {
            for (int j = 0; j < 4; j++)
            {
                if (j == i)
                {
                    imgButton[j].sprite = spropen[j];
                    shop[j].GetComponent<ScrollRect>().enabled = true;
                    shop[j].transform.GetChild(0).GetChild(0).GetComponent<RectTransform>().anchoredPosition =new Vector2(0,0);
                    shop[j].transform.localScale = Vector3.one;
                }
                else
                {
                    imgButton[j].sprite = sprlock[j];
                    shop[j].transform.localScale = Vector3.zero;
                    shop[j].GetComponent<ScrollRect>().enabled = false;
                }
            }
        }
        else
        {
            if (PlayerPrefs.GetInt("huongdan") == 18)
            {
                if (i == 1)
                {
                    PlayerPrefs.SetInt("huongdan", 19);
                    TutorialController.instance.ShowCanvasMessage("Kéo gà vào chuồng", "Pull the chicken into the cage");
                    TutorialController.instance.shopDragHand.SetActive(true);
                    TutorialController.instance.animalIconPointerHand.SetActive(false);
                    for (int j = 0; j < 4; j++)
                    {
                        if (j == i)
                        {
                            imgButton[j].sprite = spropen[j];
                            shop[j].GetComponent<ScrollRect>().enabled = true;
                            shop[j].transform.GetChild(0).GetChild(0).GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
                            shop[j].transform.localScale = Vector3.one;
                        }
                        else
                        {
                            imgButton[j].sprite = sprlock[j];
                            shop[j].transform.localScale = Vector3.zero;
                            shop[j].GetComponent<ScrollRect>().enabled = false;
                        }
                    }
                }
            }
            if (PlayerPrefs.GetInt("huongdan") == 15)
            {
                if (i == 2)
                {

                    for (int j = 0; j < 4; j++)
                    {
                        if (j == i)
                        {
                            imgButton[j].sprite = spropen[j];
                            shop[j].GetComponent<ScrollRect>().enabled = true;
                            shop[j].transform.GetChild(0).GetChild(0).GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
                            shop[j].transform.localScale = Vector3.one;
                        }
                        else
                        {
                            imgButton[j].sprite = sprlock[j];
                            shop[j].transform.localScale = Vector3.zero;
                            shop[j].GetComponent<ScrollRect>().enabled = false;
                        }
                    }
                }
            }
            if (PlayerPrefs.GetInt("huongdan") == 17)
            {
                if (i == 0)
                {

                    for (int j = 0; j < 4; j++)
                    {
                        if (j == i)
                        {
                            imgButton[j].sprite = spropen[j];
                            shop[j].GetComponent<ScrollRect>().enabled = true;
                            shop[j].transform.GetChild(0).GetChild(0).GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
                            shop[j].transform.localScale = Vector3.one;
                        }
                        else
                        {
                            imgButton[j].sprite = sprlock[j];
                            shop[j].transform.localScale = Vector3.zero;
                            shop[j].GetComponent<ScrollRect>().enabled = false;
                        }
                    }
                }
            }
        }
    }
}
