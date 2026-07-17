using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class MagicTreeClickHandler : MonoBehaviour {

    private void OnMouseUpAsButton()
    {
        if (!EventSystem.current.IsPointerOverGameObject(0))
        {
            if (!PlayerPrefs.HasKey("huongdan"))
            {
                MainCamera.Instance.DisableAll();
                transform.parent.GetComponent<MagicCoinTree>().Interact();
            }
        }
    }
}
