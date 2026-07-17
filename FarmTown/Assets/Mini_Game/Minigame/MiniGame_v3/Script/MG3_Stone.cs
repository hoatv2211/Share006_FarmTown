using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MG3_Stone : MonoBehaviour
{
    [SerializeField] Rigidbody2D rg;
    [SerializeField] string nameObjectAction;
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
        if (name.Equals("MaxMa"))
        {
            if (rg != null && msg.nameObjectAction.Equals(name))
                rg.simulated = true;
            return;
        }

        if(rg!= null)
            rg.simulated = true;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "enemy":
                collision.Recycle();
                gameObject.Recycle();
                this.PostEvent((int)EventID.OnShowFxDestroy, new MessageFx { pos = transform.position });
                this.PostEvent((int)EventID.OnCompleteKeyHandle, new MessagerKeyHandle { nameObjectAction = nameObjectAction });
                break;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("muikhoan"))
        {
            Invoke("Delay",3.5f);
        }

    }
    void Delay()
    {
        this.PostEvent((int)EventID.OnContiniueMove);
        this.PostEvent((int)EventID.OnShowFxDestroy, new MessageFx { pos = transform.position });
        gameObject.Recycle();
    }
}
