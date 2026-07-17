using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class MG3_Enemy : MonoBehaviour
{
    [SerializeField] Transform posGoto;
    [SerializeField] Transform posGoto2;
    [SerializeField] bool isDestroy;
    [SerializeField] string nameStopHandle;
    GameObject tempObj;
    Sequence sequence;
    bool isTriger;
    private void Start()
    {
        sequence = DOTween.Sequence();
    }
    private void OnEnable()
    {
        this.RegisterListener((int)EventID.OnMouseDownHandle, OnMouseDownHandleHandle);
        this.RegisterListener((int)EventID.OnCompleteRotate, OnCompleteRotateHandle);
        this.RegisterListener((int)EventID.OnCompleteKeyHandle, OnCompleteKeyHandle);
    }
    private void OnDisable()
    {
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnMouseDownHandle, OnMouseDownHandleHandle);
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnCompleteRotate, OnCompleteRotateHandle);
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnCompleteKeyHandle, OnCompleteKeyHandle);
    }
    private void OnCompleteKeyHandle(object obj)
    {
        var msg = (MessagerKeyHandle)obj;
        if (msg.nameObjectAction.Equals(name) && posGoto != null)
        {
            if (posGoto2 != null)
            {
                if (transform.position == posGoto2.position)
                    return;
                if (transform.position == posGoto.position)
                {
                    sequence.Append(transform.DOMove(posGoto2.position, 1f).OnComplete(() =>
                    {
                        this.PostEvent((int)EventID.OnCompleteKeyHandle, new MessagerKeyHandle { nameObjectAction = nameStopHandle });
                    }));
                    return;
                }
            }

            sequence.Append(transform.DOMove(posGoto.position, 1f).OnComplete(() =>
            {
                name = "animal_done";
                if (isDestroy)
                    gameObject.Recycle();
            }));

        }

    }
    private void OnCompleteRotateHandle(object obj)
    {
        var msg = (RotateDirection)obj;
        //Debug.Log("=> OnCompleteRotate = " + msg);
        if (posGoto == null)
            return;
        if (msg == RotateDirection.right && !name.Equals("raft_move"))
        {
            if (posGoto2 != null)
            {
                if (transform.position == posGoto2.position)
                    return;
            }
            sequence.Append(transform.DOMove(posGoto.position, 1f).OnComplete(() =>
            {
            }));
        }
        else
        if (msg == RotateDirection.down && transform.position == posGoto.position)
        {
            if (posGoto2 != null)
                sequence.Append(transform.DOMove(posGoto2.position, 1f).OnComplete(() =>
                {
                }));
        }
        else
        if (msg == RotateDirection.down &&( name.Equals("bao")))
        {
            sequence.Append(transform.DOMove(posGoto.position, 1f).OnComplete(() =>
            {
            }));
        }
        else
        if (msg == RotateDirection.up &&name.Equals("raft_move"))
        {
            sequence.Append(transform.DOMove(posGoto.position, 1f).OnComplete(() =>
            {
            }));
        }
    }

    private void OnMouseDownHandleHandle(object obj)
    {
        var msg = (MG3_MouseHandle)obj;
        switch (name)
        {
            case "Tiger":
                if (msg == MG3_MouseHandle.Eat)
                {
                    sequence.Append(transform.DOMove(posGoto.position, 1f).OnComplete(() =>
                    {
                        this.PostEvent((int)EventID.OnConfinement, name);
                    }));
                }
                break;
            case "Crocodile":
                if(msg == MG3_MouseHandle.Eat)
                {
                    sequence.Append(transform.DOMove(posGoto.position, 1f).OnComplete(() =>
                    {
                        this.PostEvent((int)EventID.OnCompleteKeyHandle, new MessagerKeyHandle { nameObjectAction = nameStopHandle});
                    }));
                }
                break;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "enemy":
            case "draft":
                if (!isTriger)
                {
                    isTriger = true;
                    sequence.Kill();
                    this.PostEvent((int)EventID.OnShowFxDestroy, new MessageFx { pos = transform.position });
                    collision.Recycle();
                }
                
                break;
            case "duiga":
                if(tempObj == null)
                {
                    tempObj = collision.gameObject;
                    Invoke("DelayEat", 1f);
                }
                break;
        }
        if(collision.tag== "draft")
        {
            this.PostEvent((int)EventID.OnGameLost);
        }
    }
    void DelayEat()
    {
        sequence.Kill();
        this.PostEvent((int)EventID.OnShowFxDestroy, new MessageFx { pos = transform.position });
        if (tempObj != null)
            tempObj.Recycle();
    }
}
