using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class DeliveryTruck : MonoBehaviour {

    private void OnMouseUpAsButton()
    {
        if (!EventSystem.current.IsPointerOverGameObject(0))
        {
            OrderPanel.instance.CollectDeliveryReward();
        }
    }
}
