using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class LandExpansion : MonoBehaviour {

    [SerializeField] GameObject obMothem,DialogMo;
	void Awake () {
        if (PlayerPrefs.HasKey("movungdat"))
        {
            obMothem.SetActive(true);
            gameObject.SetActive(false);
        }
	}

    private void OnMouseUpAsButton()
    {
        if (!EventSystem.current.IsPointerOverGameObject(0))
        {
            if (PlayerPrefs.GetInt("level") < 25)
            {
                Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                NotificationController.Instance.ShowWorldNotification("Vùng đất được mở ở cấp độ 25", "The land is unlock at level 25", pos);
            }
            else
            {
                DialogMo.SetActive(true);
            }
        }
    }
    public void Open()
    {
        if (PlayerPrefs.GetInt("gold") >= 5000)
        {
            GameBootstrap.Instance.AddGold(-5000);
            obMothem.SetActive(true);
            DialogMo.SetActive(false);
            gameObject.SetActive(false);
            PlayerPrefs.SetInt("movungdat", 1);
        }
    }
    public void close()
    {
        DialogMo.SetActive(false);
    }
}

