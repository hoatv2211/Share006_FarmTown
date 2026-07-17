using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MG3_FollowPlayer : MonoBehaviour
{
    [SerializeField] float speed = 5;
    [SerializeField] float distanceStop;
    [SerializeField] Transform target;
    [SerializeField] SkeletonAnimation skeleton;
    [SerializeField] ParticleSystem smoke;
    private void Start()
    {
        skeleton.AnimationName = "idle";
    }
    private void OnEnable()
    {
        this.RegisterListener((int)EventID.OnCompleteKeyHandle, OnCompleteKeyHandle);
    }
    private void OnDisable()
    {
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnCompleteKeyHandle, OnCompleteKeyHandle);
    }

    private void OnCompleteKeyHandle(object obj)
    {
        var msg = (MessagerKeyHandle)obj;
        if (msg.nameObjectAction.Equals(name))
            SetTarget();
    }

    public void SetTarget()
    {
        target = GameManagerMiniGame.Player.transform;
        skeleton.AnimationName = "go";
    }
    void Update()
    {
        if(target != null)
        {
            if (Vector2.Distance(transform.position, target.position) >= distanceStop)
            {
                if (skeleton.AnimationName != "go")
                {
                    skeleton.AnimationName = "go";
                    PlayEffectSmoke();
                }
                transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
            }
            else
            if (Vector2.Distance(transform.position, target.position) < distanceStop)
            {
                if (skeleton.AnimationName != "idle")
                {
                    skeleton.AnimationName = "idle";
                    PlayEffectSmoke();
                }
            }
        }
    }
    void PlayEffectSmoke()
    {
        if (skeleton.AnimationName == "go")
            smoke.Play();
        else smoke.Stop();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "enemy":
                this.PostEvent((int)EventID.OnGameLost);
                break;
        }

    }
}
