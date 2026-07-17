using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MG3_Rotate : MonoBehaviour
{
    private Camera myCam;
    private Vector3 screenPos;
    private Vector3 mousePosOnMouseDown;
    private Vector3 clickOffset;
    private float angleOffset;
    private Collider2D col;

    private Quaternion originalRotation;
    private float startAngle = 0;

    RotateDirection rotateDirection = RotateDirection.none;
    private void Start()
    {
        myCam = GameManagerMiniGame.Camera;
        col = GetComponent<Collider2D>();
        originalRotation = this.transform.rotation;
    }

    private void LateUpdate()
    {

        Vector3 mousePos = myCam.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetMouseButtonDown(0))
        {
            screenPos = myCam.WorldToScreenPoint(transform.position);
            Vector3 v3 = Input.mousePosition - screenPos;
            angleOffset = (Mathf.Atan2(transform.right.y, transform.right.x) - Mathf.Atan2(v3.y, v3.x)) * Mathf.Rad2Deg;
        }
        //This fires while the button is pressed down
        if (Input.GetMouseButton(0))
        {
            Vector3 v3 = Input.mousePosition - screenPos;
            float angle = Mathf.Atan2(v3.y, v3.x) * Mathf.Rad2Deg;
            transform.eulerAngles = new Vector3(0, 0, angle + angleOffset);
        }


        //if (Input.GetMouseButtonDown(0))
        //{
        //    if (col == Physics2D.OverlapPoint(mousePos))
        //    {
        //        screenPos = myCam.WorldToScreenPoint(transform.position);
        //        Vector3 vec3 = Input.mousePosition - screenPos;
        //        angleOffset = (Mathf.Atan2(transform.right.y, transform.right.x) - Mathf.Atan2(vec3.y, vec3.x)) * Mathf.Rad2Deg;
        //    }
        //}
        //if (Input.GetMouseButton(0))
        //{
        //    if (col == Physics2D.OverlapPoint(mousePos))
        //    {
        //        Vector3 vec3 = Input.mousePosition - screenPos;
        //        float angle = Mathf.Atan2(vec3.y, vec3.x) * Mathf.Rad2Deg;
        //        transform.eulerAngles = new Vector3(0, 0, angle + angleOffset);
        //    }
        //}

        if (Input.GetMouseButtonUp(0))
        {
            if (col == Physics2D.OverlapPoint(mousePos))
            {
                rotateDirection = RotateDirection.none;
                if (transform.eulerAngles.z > 255 && transform.eulerAngles.z < 285)
                    rotateDirection = RotateDirection.right;
                if (transform.eulerAngles.z > 170 && transform.eulerAngles.z < 190)
                    rotateDirection = RotateDirection.down;
                if ((transform.eulerAngles.z > 345 && transform.eulerAngles.z <= 360) || (transform.eulerAngles.z > 0 && transform.eulerAngles.z <= 10))
                    rotateDirection = RotateDirection.up;
                if (transform.eulerAngles.z > 70 && transform.eulerAngles.z < 97)
                    rotateDirection = RotateDirection.left;

                Debug.Log("=> Rotate = " + transform.eulerAngles.z);
                this.PostEvent((int)EventID.OnCompleteRotate, rotateDirection);
            }
        }
    }
}

public enum RotateDirection
{
    none, right, left, down, up
}
