using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public static CameraFollow instance;
    [SerializeField]  Transform Target;
    [SerializeField] float smoothing;
    [SerializeField] float offsetX = -1;
    float tempOffsetX = 0;
    Vector3 tempPos;
    private void Awake()
    {
        instance = this;
        tempOffsetX = offsetX;
        tempPos = transform.position;
    }
    public void RestPosition()
    {
        transform.position = tempPos;
    }
    private void OnEnable()
    {
        //this.RegisterListener((int)EventID.OnGameWin, OnGameWinHandle);
        //this.RegisterListener((int)EventID.OnMG3Replay, OnMG3ReplayHandle);
    }

    private void OnDisable()
    {
        //EventDispatcher.Instance?.RemoveListener((int)EventID.OnGameWin, OnGameWinHandle);
        //EventDispatcher.Instance?.RemoveListener((int)EventID.OnMG3Replay, OnMG3ReplayHandle);
    }
    private void OnGameWinHandle(object obj)
    {
        offsetX = 0;
    }
    private void OnMG3ReplayHandle(object obj)
    {
        offsetX = tempOffsetX;
    }

    public void SetTarget(Transform tf)
    {
        Target = tf;
    }
    private void LateUpdate()
    {
        if (Target)
        {
            transform.position = new Vector3(Target.position.x+ offsetX, Target.position.y,-10);
        }
    }
}
