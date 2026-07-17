using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class DialogObjectClickHandler : MonoBehaviour
{

    Animator anim;
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void OnMouseUpAsButton()
    {
        if (!EventSystem.current.IsPointerOverGameObject(0))
        {
            click();
        }
    }
    public void click()
    {
        if (!PlayerPrefs.HasKey("huongdan"))
        {
            if (PlayerPrefs.GetInt("dangdichuyen") == 0)
            {
                if (anim.enabled == false)
                    anim.enabled = true;
                anim.Play("test", -1, 0);
                MainCamera.Instance.MoveCamera(transform.position, true);
            }
            else
            {
                NotificationController.Instance.ShowWorldNotification("Bạn chưa hoàn thành di chuyển!", "You have not finished moving!", transform.position);
            }
        }
    }
}
