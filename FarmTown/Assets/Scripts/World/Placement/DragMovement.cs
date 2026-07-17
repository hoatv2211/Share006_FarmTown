using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragMovement : MonoBehaviour {

    
    private void OnMouseDrag()
    {
        Vector2 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = target;
    }
    private void OnMouseUp()
    {
        Debug.Log("Released drag");
    }

}
