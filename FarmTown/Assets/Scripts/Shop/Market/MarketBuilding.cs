using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class MarketBuilding : MonoBehaviour {

    private void Start()
    {
        Vector3 temp = transform.position;
        gameObject.GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt((13 - temp.y) * 100f);
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
                    MarketPanel.instance.OpenDialog();
                }
            }
        }
    }
}
