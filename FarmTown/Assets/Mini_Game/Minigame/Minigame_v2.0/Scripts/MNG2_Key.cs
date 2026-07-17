using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class MNG2_Key : MonoBehaviour
{
    public bool enable;
    [SerializeField] bool destroy;
    [SerializeField] Vector3 doMove;
    [SerializeField] GameObject[] khoas;
    [SerializeField] GameObject eventsNe;
    [SerializeField] string nameFunction;
    [SerializeField] float fieldFuntion;
    [SerializeField] GameObject tutorial;

    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject() || EventSystem.current.IsPointerOverGameObject(0)) return;
        transform.DOLocalMove(doMove, 0.5f);
        enable = true;
        StartCoroutine(SendEvent());

        if (tutorial != null)
        {
            tutorial.gameObject.SetActive(false);
        }
    }

    IEnumerator SendEvent()
    {
        yield return new WaitForSeconds(0.6f);
        //event
        if (eventsNe != null)
        {
            if (fieldFuntion != 0)
            {
                eventsNe.SendMessage(nameFunction, fieldFuntion);
            }
            else
            {
                eventsNe.SendMessage(nameFunction);
            }
        }
        GameManagerMiniGame.instance.PressKey();
        MNG2_Cua.instance.MoCua();
        //end
        for (int i = 0; i < khoas.Length; i++)
        {
            khoas[i].Recycle();
        }
       gameObject.Recycle();
    }
}
