using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class CompletedProductIcon : MonoBehaviour {

    private void OnMouseUpAsButton()
    {
        if (!EventSystem.current.IsPointerOverGameObject(0))
        {
        transform.parent.GetComponent<ProductionBuilding>().CollectProduct();
        }
    }
}
