using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class DecorationItemBuildingClickHandler : MonoBehaviour
{

    private void OnMouseUpAsButton()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            NotificationController.Instance.ShowWorldNotification("Cập nhật trong phiên bản tiếp theo!!!", "Coming soon!!!", transform.position);
        }
    }
}
