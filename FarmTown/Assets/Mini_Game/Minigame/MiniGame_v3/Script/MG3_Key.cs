using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class MG3_Key : MonoBehaviour
{
    [SerializeField] SpriteRenderer[] spr;
    [SerializeField] BoxCollider2D box;
    [SerializeField] Transform posTo;
    [SerializeField] float timeTo;
    [SerializeField] Transform child;
    [SerializeField] string nameObjectAction;
    [SerializeField] Transform posPlayerMoveTo;
    Sequence sequence;
    float mouseTime;
    Vector3 vitricu;
    private void Start()
    {
        sequence = DOTween.Sequence();
    }
    private void OnEnable()
    {
        this.RegisterListener((int)EventID.OnConfinement, OnConfinementHandle);
    }
    private void OnDisable()
    {
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnConfinement, OnConfinementHandle);
    }
    private void OnMouseDown()
    {
        if (!Util.IsMouseOverUI)
        {
            mouseTime = Time.time;
            vitricu = GameManagerMiniGame.Camera.ScreenToWorldPoint(Input.mousePosition);
        }
    }
    private void OnMouseUp()
    {
        if (!Util.IsMouseOverUI)
        {
            if (EventSystem.current.IsPointerOverGameObject() == false && Vector3.Distance(vitricu, GameManagerMiniGame.Camera.ScreenToWorldPoint(Input.mousePosition)) < 0.2f
                && Time.time - mouseTime < 0.2f)
            {
                GameManagerMiniGame.instance.ShowObjTutorial(false);
                sequence.Append(child.DOMove(posTo.position, timeTo).OnComplete(() =>
                {
                    if (box)
                        box.enabled = false;
                    HideKey();
                    this.PostEvent((int)EventID.OnCompleteKeyHandle, new MessagerKeyHandle { nameObjectAction = nameObjectAction, posPlayerMoveTo = posPlayerMoveTo });
                }));
            }

        }
    }
    private void OnConfinementHandle(object obj)
    {

        sequence.Append(child.DOMove(posTo.position, timeTo).OnComplete(() =>
           {
               if (box)
                   box.enabled = false;
               HideKey();
               this.PostEvent((int)EventID.OnCompleteKeyHandle, new MessagerKeyHandle { nameObjectAction = nameObjectAction, posPlayerMoveTo = posPlayerMoveTo });
           }));

    }
    void HideKey()
    {
        for (int i = 0; i < spr.Length; i++)
        {
            sequence.Append(spr[i].DOColor(new Color(1, 1, 1, 0), .5f));
        }
    }
    private void OnDestroy()
    {
        sequence.Kill();
    }
}

public class MessagerKeyHandle
{
    public string nameObjectAction;
    public Transform posPlayerMoveTo;
}
