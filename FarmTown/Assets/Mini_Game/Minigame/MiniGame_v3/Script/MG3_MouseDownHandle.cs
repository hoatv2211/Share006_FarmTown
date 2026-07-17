using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MG3_MouseDownHandle : MonoBehaviour
{
    [SerializeField] MG3_MouseHandle mG3_Mouse;
    [SerializeField] GameObject objHandle;
    [SerializeField] BoxCollider2D[] boxs;
    [SerializeField] string nameHandle;
    private void OnMouseDown()
    {
        objHandle?.SetActive(true);
        for (int i = 0; i < boxs.Length; i++)
        {
            boxs[i].enabled = false;
        }
        if (mG3_Mouse == MG3_MouseHandle.Fail)
        {
            Invoke("DelayContiniueMove", 2.5f);
            return;
        }
        if (mG3_Mouse == MG3_MouseHandle.Raft)
        {
            this.PostEvent((int)EventID.OnCompleteKeyHandle, new MessagerKeyHandle { nameObjectAction = nameHandle });
            return;
        }
        this.PostEvent((int)EventID.OnMouseDownHandle, mG3_Mouse);
    }

    void DelayContiniueMove()
    {
        this.PostEvent((int)EventID.OnCompleteKeyHandle, new MessagerKeyHandle { nameObjectAction = nameHandle});
    }
}
public enum MG3_MouseHandle
{
    Eat, Rock, Fail, Saw, Fire, Raft
}