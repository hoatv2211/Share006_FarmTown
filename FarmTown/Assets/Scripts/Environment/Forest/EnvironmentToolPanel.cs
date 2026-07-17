using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
public class EnvironmentToolPanel : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    public static EnvironmentToolPanel instance;
    [FormerlySerializedAs("anim")] [SerializeField] Animator animator;
    private GameObject dragPreview;
    [FormerlySerializedAs("obsinh")] public GameObject dragPrefab;
    [FormerlySerializedAs("cua")] [SerializeField] Sprite sawSprite;
    [FormerlySerializedAs("riu")] [SerializeField] Sprite axeSprite;
    [FormerlySerializedAs("bom")] [SerializeField] Sprite bombSprite;
    int toolType;
    private void Start()
    {
        instance = this;
    }
    public void Open(Vector2 pos, int selectedToolType)
    {
        toolType = selectedToolType;
        if (selectedToolType == 0)
        {
            transform.parent.localPosition = new Vector3(0, 264, 0);
            gameObject.GetComponent<Image>().sprite = sawSprite;
            transform.GetChild(0).GetComponent<Text>().text = PlayerPrefs.GetInt("slvatpham49").ToString();
        }
        if (selectedToolType == 1)
        {
            transform.parent.localPosition = new Vector3(0, 100, 0);
            gameObject.GetComponent<Image>().sprite = axeSprite;
            transform.GetChild(0).GetComponent<Text>().text = PlayerPrefs.GetInt("slvatpham50").ToString();
        }
        if (selectedToolType == 2)
        {
            transform.parent.localPosition = new Vector3(0, 264, 0);
            gameObject.GetComponent<Image>().sprite = bombSprite;
            transform.GetChild(0).GetComponent<Text>().text = PlayerPrefs.GetInt("slvatpham51").ToString();
        }
        gameObject.GetComponent<Image>().SetNativeSize();
        transform.parent.parent.position = pos;
        animator.enabled = true;
        animator.Play("dialogLiem", -1, 0);
    }
    public void Close()
    {
        animator.enabled = false;
        transform.parent.localScale = Vector3.zero;
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        Vector2 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        dragPreview = Instantiate(dragPrefab, target, Quaternion.identity);
        if (toolType == 0)
        {
            dragPreview.GetComponent<SpriteRenderer>().sprite = sawSprite;
        }
        if (toolType == 1)
        {
            dragPreview.GetComponent<SpriteRenderer>().sprite = axeSprite;
        }
        if (toolType == 2)
        {
            dragPreview.GetComponent<SpriteRenderer>().sprite = bombSprite;
        }
        Close();
        MainCamera.Instance.lockCam();
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        dragPreview.transform.position = target;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Destroy(dragPreview);
        MainCamera.Instance.unLockCam();
    }
}
