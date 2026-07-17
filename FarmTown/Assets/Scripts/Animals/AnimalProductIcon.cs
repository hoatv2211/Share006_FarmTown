using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class AnimalProductIcon : MonoBehaviour {

    private void OnMouseUpAsButton()
    {
        if (!EventSystem.current.IsPointerOverGameObject(0))
        {
            transform.parent.GetComponent<FarmAnimal>().Harvest();
        }
    }
}
