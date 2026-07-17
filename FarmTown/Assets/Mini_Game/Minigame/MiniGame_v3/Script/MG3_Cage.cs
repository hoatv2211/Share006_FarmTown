using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MG3_Cage : MonoBehaviour
{
    [SerializeField] Rigidbody2D rg;
    private void OnEnable()
    {
        this.RegisterListener((int)EventID.OnCompleteKeyHandle, OnCompleteKeyHandle);
        this.RegisterListener((int)EventID.OnGameLost, OnGameLostHandle);
    }
    private void OnDisable()
    {
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnCompleteKeyHandle, OnCompleteKeyHandle);
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnGameLost, OnGameLostHandle);
    }

    private void OnGameLostHandle(object obj)
    {
        rg = null;
    }

    private void OnCompleteKeyHandle(object obj)
    {
        if(rg != null)
        {
            rg.simulated = true;
            return;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "enemy":
                collision.transform.SetParent(transform);
                collision.transform.localPosition = Vector3.zero;
                break;
        }
    }
}
