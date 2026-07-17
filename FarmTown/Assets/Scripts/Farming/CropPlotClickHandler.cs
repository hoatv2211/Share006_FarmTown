using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CropPlotClickHandler : MonoBehaviour
{

    Animator anim;
    float downtime;
    void Start()
    {
        anim = GetComponent<Animator>();
    }
    private void OnMouseDown()
    {
        if (!EventSystem.current.IsPointerOverGameObject(0))
        {
            downtime = Time.time;
        }
    }
    private void OnMouseUpAsButton()
    {
        if (!EventSystem.current.IsPointerOverGameObject(0))
        {
            if (PlayerPrefs.GetInt("dangdichuyen") == 0)
            {
                if (anim.enabled == false)
                    anim.enabled = true;
                anim.Play(AnimatorParameters.CropPlotClick, -1, 0);
                MainCamera.Instance.MoveCamera(transform.position, false);
            }
        }
    }
    public void click()
    {
        anim.Play(AnimatorParameters.CropPlotClick, -1, 0);
        MainCamera.Instance.MoveCamera(transform.position, false);
    }
}
