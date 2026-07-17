using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
public class SickleTool : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public static SickleTool instance;
    [FormerlySerializedAs("obsinh")] public GameObject sicklePrefab;
    [FormerlySerializedAs("obj")] public GameObject spawnedSickle;
    private void Start()
    {
        instance = this;
    }
    public void open(Vector2 pos)
    {
        // transform.parent.localScale = Vector3.one;
        transform.parent.parent.position = pos;
        transform.parent.GetComponent<Animator>().enabled = true;
        transform.parent.GetComponent<Animator>().Play(AnimatorParameters.SicklePanelOpen, -1, 0);
        if (PlayerPrefs.GetInt("huongdan") == 4)
        {
            TutorialController.instance.secondaryUiDragHand.SetActive(false);
            PlayerPrefs.SetInt("huongdan", 5);
        }
    }
    public void close()
    {
        transform.parent.GetComponent<Animator>().enabled = false;
        transform.parent.localScale = Vector3.zero;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Vector2 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        spawnedSickle = Instantiate(sicklePrefab, target, Quaternion.identity);
        close();
        MainCamera.Instance.lockCam();
        if (PlayerPrefs.HasKey("huongdan"))
            TutorialController.instance.secondaryUiDragHand.SetActive(false);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        spawnedSickle.transform.position = target;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Destroy(spawnedSickle);
        MainCamera.Instance.unLockCam();
    }
}
