using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
public class AnimalDragHandler : MonoBehaviour
{
    [FormerlySerializedAs("id")] public int animalTypeId;
    [FormerlySerializedAs("check")] public bool hasValidTarget;
    [FormerlySerializedAs("ob")] public GameObject targetPen;
    private void Start()
    {
        gameObject.GetComponent<SpriteRenderer>().color = new Color32(100, 50, 50, 255);
        hasValidTarget = false;
    }
    private void OnTriggerStay2D(Collider2D target)
    {
        if (target.gameObject.layer == 9)
        {
            if (target.gameObject.GetComponent<AnimalPen>().animalTypeId == animalTypeId)
            {
                targetPen = target.gameObject;
                gameObject.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255);
                hasValidTarget = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D target)
    {
        if (target.gameObject.layer == 9)
        {
            if (target.gameObject.GetComponent<AnimalPen>().animalTypeId == animalTypeId)
            {
                gameObject.GetComponent<SpriteRenderer>().color = new Color32(100, 50, 50, 255);
                hasValidTarget = false;
            }
        }
    }
    
}
