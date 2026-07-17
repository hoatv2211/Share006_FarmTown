using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class CropStorageBuilding : MonoBehaviour
{
    private void Start()
    {
        Vector3 temp = transform.position;
        gameObject.GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt((13 - temp.y) * 100f);
    }
    private void OnMouseUpAsButton()
    {
        if (!EventSystem.current.IsPointerOverGameObject(0))
        {
            if (!PlayerPrefs.HasKey("huongdan"))
                if (PlayerPrefs.GetInt("dangdichuyen") == 0)
                {
                    if (gameObject.GetComponent<Animator>().enabled == false)
                        gameObject.GetComponent<Animator>().enabled = true;
                    gameObject.GetComponent<Animator>().Play("test", -1, 0);
                    MainCamera.Instance.MoveCamera(transform.position, false);
                    InventoryController.instance.Open();
                    InventoryController.instance.ShowCrops();
                }
        }
    }
}
