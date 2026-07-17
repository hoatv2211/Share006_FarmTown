using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class TutorialController : MonoBehaviour
{
    public static TutorialController instance;

    [FormerlySerializedAs("bantaychi")] public GameObject pointerHand;
    [FormerlySerializedAs("bantaylen")] public GameObject upwardDragHand;
    [FormerlySerializedAs("bantaykeoxuong")] public GameObject downwardDragHand;
    [FormerlySerializedAs("taykeoUi")] public GameObject uiDragHand;
    [FormerlySerializedAs("taykeoui2")] public GameObject secondaryUiDragHand;
    [FormerlySerializedAs("bantaychi2")] public GameObject secondaryPointerHand;
    [FormerlySerializedAs("taychiShop")] public GameObject shopPointerHand;
    [FormerlySerializedAs("fxshop")] public GameObject shopHighlight;
    [FormerlySerializedAs("taykeoshop")] public GameObject shopDragHand;
    [FormerlySerializedAs("taykeoNm")] public GameObject productionDragHand;
    [FormerlySerializedAs("dialogend")] public GameObject completionDialog;
    [FormerlySerializedAs("taychikcnhamay")] public GameObject speedUpPointerHand;
    [FormerlySerializedAs("taykeochuong")] public GameObject penDragHand;
    [FormerlySerializedAs("taychiiconvn")] public GameObject animalIconPointerHand;
    [FormerlySerializedAs("vatnuoian")] public GameObject animalFeedDragHand;
    [FormerlySerializedAs("taychixehang")] public GameObject deliveryTruckPointerHand;
    [FormerlySerializedAs("thongBaoHuongDan")] public GameObject tutorialMessagePrefab;
    [FormerlySerializedAs("posThongBao")] public GameObject worldMessageAnchor;
    [FormerlySerializedAs("posgiua")] public GameObject canvasMessageAnchor;

    private void Start()
    {
        instance = this;
        if (!PlayerPrefs.HasKey("huongdan"))
        {
            pointerHand.SetActive(false);
            upwardDragHand.SetActive(false);
            downwardDragHand.SetActive(false);
        }
        else
        {
            MovePointerHand(CropPlot.FirstRuntimePlot.transform.position);
        }
    }

    public void MovePointerHand(Vector2 position)
    {
        pointerHand.transform.position = position;
    }

    public void MoveUpwardDragHand(Vector2 position)
    {
        Vector2 target = transform.InverseTransformPoint(position);
        upwardDragHand.transform.localPosition = target;
    }

    public void MoveDownwardDragHand(Vector2 position)
    {
        downwardDragHand.transform.position = position;
    }

    public void MoveUiDragHand(Vector2 position)
    {
        Vector2 target = transform.InverseTransformPoint(position);
        uiDragHand.GetComponent<RectTransform>().localPosition = target;
    }

    public void CloseCompletionDialog()
    {
        completionDialog.SetActive(false);
    }

    public void ShowCameraMessage(string vietnameseText, string englishText)
    {
        ShowMessage(vietnameseText, englishText, canvasMessageAnchor, false);
    }

    public void ShowWorldMessage(string vietnameseText, string englishText)
    {
        ShowMessage(vietnameseText, englishText, worldMessageAnchor, false);
    }

    public void ShowCanvasMessage(string vietnameseText, string englishText)
    {
        ShowMessage(vietnameseText, englishText, canvasMessageAnchor, true);
    }

    private void ShowMessage(string vietnameseText, string englishText, GameObject parent, bool useOverlaySorting)
    {
        GameObject message = Instantiate(tutorialMessagePrefab, parent.transform);
        message.transform.SetParent(parent.transform);
        if (useOverlaySorting)
            message.GetComponent<Canvas>().sortingOrder = 200;

        message.transform.GetChild(0).GetComponent<Text>().text =
            Application.systemLanguage == SystemLanguage.Vietnamese ? vietnameseText : englishText;
        Destroy(message, 2.5f);
    }
}
