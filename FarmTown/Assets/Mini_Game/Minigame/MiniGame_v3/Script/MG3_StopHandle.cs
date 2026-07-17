using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MG3_StopHandle : MonoBehaviour
{
    [SerializeField] BoxCollider2D box;
    public Transform nextPosMove;
    private void OnEnable()
    {
        this.RegisterListener((int)EventID.OnCompleteKeyHandle, OnCompleteKeyHandle);
        this.RegisterListener((int)EventID.OnCompleteRotate, OnCompleteRotateHandle);
        this.RegisterListener((int)EventID.OnContiniueMove, OnContiniueMoveHandle);
    }
    private void OnDisable()
    {
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnCompleteKeyHandle, OnCompleteKeyHandle);
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnCompleteRotate, OnCompleteRotateHandle);
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnContiniueMove, OnContiniueMoveHandle);
    }

    private void OnContiniueMoveHandle(object obj)
    {
        box.enabled = false;
    }

    private void OnCompleteRotateHandle(object obj)
    {
        var msg = (RotateDirection)obj;
        if (name.Equals("StopHanlde (2)"))
        {
            box.enabled = true;
        }

        if (msg == RotateDirection.left && name.Equals("StopHanlde (1)"))
        {
            box.enabled = false;
        }
        else
        {
            if (msg == RotateDirection.right && name.Equals("StopHanlde (2)"))
            {
                box.enabled = false;
            }
        }
    }
    private void OnCompleteKeyHandle(object obj)
    {
        var msg = (MessagerKeyHandle)obj;
        if (msg.nameObjectAction.Equals(name))
        {
            nextPosMove = msg.posPlayerMoveTo;
            box.enabled = false;
            if (name == "StopHanlde_disable_raft")
                transform.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            if (msg.nameObjectAction.Equals("animal_done") && name.Equals("StopHanlde"))
            {
                box.enabled = false;
            }
            else
            {
                if (msg.nameObjectAction.Equals("animal") && name.Equals("StopHanlde_follow"))
                     box.enabled = false;
            }
        }
    }
}
