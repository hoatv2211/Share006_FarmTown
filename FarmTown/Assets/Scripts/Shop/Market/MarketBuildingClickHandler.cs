using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MarketBuildingClickHandler : MonoBehaviour
{
    private void OnMouseUpAsButton()
    {
        if (!EventSystem.current.IsPointerOverGameObject(0))
        {
            if (!PlayerPrefs.HasKey("huongdan"))
                if (PlayerPrefs.GetInt("dangdichuyen") == 0)
                {
                    MarketPanel.instance.OpenDialog();
                }
        }
    }
}
