using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class SeedDragHandler : MonoBehaviour {

    [FormerlySerializedAs("id")] public int cropId;
    private void OnTriggerEnter2D(Collider2D target)
    {
        var cay = target.gameObject.GetComponent<ISeedReceiver>();
        if (cay != null)
        {          
        cay.PlantCrop(cropId);
        }
    }

}
