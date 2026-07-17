using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlacementCollisionChecker : MonoBehaviour {
    [FormerlySerializedAs("check")] public bool hasCollision;
    private void OnTriggerStay2D(Collider2D target)
    {
        if (target.tag == "trung")
        {
            gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color32(100, 50, 50, 255);
            hasCollision = true;
        }
    }
    private void OnTriggerExit2D(Collider2D target)
    {
        if (target.tag == "trung")
        {
            gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255);
            hasCollision = false;
        }
    }
}
