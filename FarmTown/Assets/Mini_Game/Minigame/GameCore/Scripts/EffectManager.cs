using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class EffectManager : MonoBehaviour
{

    [SerializeField] ParticleSystem prFxDestroy;

    private void Start()
    {
        prFxDestroy.CreatePool(2);
    }
    private void OnEnable()
    {
        this.RegisterListener((int)EventID.OnShowFxDestroy, OnShowFxDestroyHandle);
    }
    private void OnDisable()
    {
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnShowFxDestroy, OnShowFxDestroyHandle);
    }

    private void OnShowFxDestroyHandle(object obj)
    {
        var msg = (MessageFx)obj;
        ParticleSystem fx1 = prFxDestroy.Spawn(transform);
        fx1.transform.position = msg.pos;
    }

}

public class MessageFx
{
    public TypePut typePut;
    public Vector3 pos;
}
public enum TypePut
{
    PutIn, Collect
}
