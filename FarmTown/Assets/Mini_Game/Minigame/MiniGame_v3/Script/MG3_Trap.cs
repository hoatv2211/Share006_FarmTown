using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MG3_Trap : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] string stopHandle;
    [SerializeField] BoxCollider2D box;
    [SerializeField] bool isDestroy;
    [SerializeField] MG3_MouseHandle mouseHandle;
    
    private void OnEnable()
    {
        this.RegisterListener((int)EventID.OnCompleteKeyHandle, OnCompleteKeyHandle);
        this.RegisterListener((int)EventID.OnMouseDownHandle, OnMouseDownHandleHandle);
    }
    private void OnDisable()
    {
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnCompleteKeyHandle, OnCompleteKeyHandle);
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnMouseDownHandle, OnMouseDownHandleHandle);
    }

    private void OnMouseDownHandleHandle(object obj)
    {
        var msg = (MG3_MouseHandle)obj;
        if (msg != mouseHandle)
            return;
        if (anim != null)
            anim.enabled = true;
        if (box == null)
            return;
        box.enabled = false;
        Invoke("DelayHandle", 2f);
    }
    void DelayHandle()
    {
        this.PostEvent((int)EventID.OnCompleteKeyHandle, new MessagerKeyHandle { nameObjectAction = stopHandle });
        if (isDestroy)
            gameObject.Recycle();
    }
    private void OnCompleteKeyHandle(object obj)
    {
        var msg = (MessagerKeyHandle)obj;
        if (msg.nameObjectAction.Equals("animal_follow"))
        {
            anim.SetTrigger("idle");
        }
    }
}
