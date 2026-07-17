using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class MapBoundary : MonoBehaviour
{


    private void OnMouseUpAsButton()
    {
        if (!EventSystem.current.IsPointerOverGameObject(0))
        {
            Vector2 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            NotificationController.Instance.ShowWorldNotification("Vùng cấm!", "Prohibited zone", target);
        }
    }
}
