using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MG3_LevelController : MonoBehaviour
{
    public Transform posStart;
    public MG3_SkinPlayer[] skinPlayers;
    public List<Transform> listMove;
    [SerializeField] GameObject activeHandle;
    private void Start()
    {
        if(activeHandle)
        activeHandle?.SetActive(false);
    }
    private void OnEnable()
    {
        this.RegisterListener((int)EventID.OnStopHandle, OnStopHandleHandle);
    }
    private void OnDisable()
    {
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnStopHandle, OnStopHandleHandle);
    }

    private void OnStopHandleHandle(object obj)
    {
        var msg = (bool)obj;
        if (activeHandle)
            activeHandle?.SetActive(true);
    }
}

public enum MG3_SkinPlayer
{
    raft=0,
}