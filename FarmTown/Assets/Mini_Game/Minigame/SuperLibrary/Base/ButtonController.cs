using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class ButtonController : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    [Header("Scale click")]
    [SerializeField] bool sacleWhenClick;
    [SerializeField] Transform childScale;
    [SerializeField] Vector2 vtScale= new Vector2(.85f, .85f);
    [SerializeField] float scaleTime=.3f;
    public string soundButton = "sfxOpenPopup";
    Transform obj;
    private void Start()
    {
        obj = gameObject.transform;
        if (childScale != null)
            obj = childScale;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (sacleWhenClick)
        {
            obj.DOScale(vtScale, scaleTime/2);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (sacleWhenClick)
        {
            obj.DOScale(Vector2.one, scaleTime);
        }
    }
}
