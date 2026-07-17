using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductionInputDragHandler : MonoBehaviour {
    public int productId;
    public void OnTriggerEnter2D(Collider2D target)
    {
        var vp = target.gameObject.GetComponent<IProductionInputReceiver>();
        if (vp != null)
        {
        vp.AddDraggedProduct(productId);
        }
    }
}
