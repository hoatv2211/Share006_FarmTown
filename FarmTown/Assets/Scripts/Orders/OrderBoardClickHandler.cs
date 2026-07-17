using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class OrderBoardClickHandler : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    private void OnMouseUpAsButton()
    {
        if (!EventSystem.current.IsPointerOverGameObject(0))
        {
            if (!PlayerPrefs.HasKey("huongdan"))
            {
                if (PlayerPrefs.GetInt("dangdichuyen") == 0)
                {
                    transform.parent.GetComponent<Animator>().Play("test", -1, 0);
                    MainCamera.Instance.MoveCamera(transform.position, true);
                    OrderPanel.instance.Open();
                }
            }
        }
    }
}
