using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class CloseProductionPanelHandler : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!PlayerPrefs.HasKey("huongdan"))
            ProductionPanel.instance.Close();
    }


}
