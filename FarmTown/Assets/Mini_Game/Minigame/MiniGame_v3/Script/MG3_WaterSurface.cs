using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MG3_WaterSurface : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] Transform tfMove;
    float step;
    private void LateUpdate()
    {
        step = speed * Time.deltaTime;
        tfMove.localPosition = new Vector2(tfMove.localPosition.x + step, tfMove.localPosition.y) ;
    }
}
