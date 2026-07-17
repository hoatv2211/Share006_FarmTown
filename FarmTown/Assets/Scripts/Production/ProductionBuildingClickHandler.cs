using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class ProductionBuildingClickHandler : MonoBehaviour
{

    private void OnMouseUpAsButton()
    {
        if (!EventSystem.current.IsPointerOverGameObject(0))
        {
            if (!PlayerPrefs.HasKey("huongdan"))
            {
                transform.parent.GetComponent<ProductionBuilding>().ProductionBuildingClickHandler();
                //transform.parent.GetComponent<DialogObjectClickHandler>().click();
            }
            else
            {
                if (PlayerPrefs.GetInt("huongdan") == 16)
                { transform.parent.GetComponent<ProductionBuilding>().ProductionBuildingClickHandler(); }
            }
        }
    }
}
