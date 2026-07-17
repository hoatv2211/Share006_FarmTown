using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CropStorageClickHandler : MonoBehaviour
{
    Animator anim;
    void Start()
    {
        anim = transform.parent.GetComponent<Animator>();
    }

    private void OnMouseUpAsButton()
    {
        if (!EventSystem.current.IsPointerOverGameObject(0))
        {
            if (!PlayerPrefs.HasKey("huongdan"))
                if (PlayerPrefs.GetInt("dangdichuyen") == 0)
                {
                    if (anim.enabled == false)
                        anim.enabled = true;
                    anim.Play("test", -1, 0);
                    MainCamera.Instance.MoveCamera(transform.position, false);
                    InventoryController.instance.Open();
                    InventoryController.instance.ShowCrops();
                }
        }
    }

}
