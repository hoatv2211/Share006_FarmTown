using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class DecorationItem : MonoBehaviour
{
    public int id;
    [FormerlySerializedAs("stt")] public int instanceIndex;
    private int checkDragMovement;
    private float downtime;
    Coroutine moveCoroutine;
    Vector2 pointerStart, pointerEnd;
    private void Start()
    {
        UpdateSortingOrder();
        if (PlayerPrefs.HasKey("quay" + gameObject.name))
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y);
    }

    private void UpdateSortingOrder()
    {
        Vector2 target = transform.position;
        transform.GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt((13 - target.y) * 100f);
    }

    private void OnMouseDown()
    {
        if (!EventSystem.current.IsPointerOverGameObject(0))
        {
            if (!PlayerPrefs.HasKey("huongdan"))
                if (PlayerPrefs.GetInt("dangdichuyen") == 0)
                {
                    if (moveCoroutine != null)
                        StopCoroutine(moveCoroutine);
                    moveCoroutine = StartCoroutine(BeginMoveAfterHold());
                }
        }
    }
    IEnumerator BeginMoveAfterHold()
    {
        pointerStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        yield return new WaitForSeconds(.8f);
        pointerEnd = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Vector2.Distance(pointerStart, pointerEnd) < .2f)
        {
            checkDragMovement = 1;
            MovementDialog.instance.ShowMovementIndicator(transform.position);
            yield return new WaitForSeconds(.3f);
            MovementDialog.instance.originalPosition = transform.position;
            MainCamera.Instance.lockCam();
            checkDragMovement = 2;
            gameObject.AddComponent<GroundPlacementCollisionChecker>();
            gameObject.AddComponent<Rigidbody2D>();
            gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
            gameObject.GetComponent<PolygonCollider2D>().isTrigger = true;
            MainCamera.Instance.unLockCam();
            MovementDialog.instance.Open();
            MovementDialog.instance.confirmButton.onClick.AddListener(ConfirmMove);
            MovementDialog.instance.cancelButton.onClick.AddListener(CancelMove);
            MovementDialog.instance.rotateButton.onClick.AddListener(Rotate);
            PlayerPrefs.SetInt("dangdichuyen", 1);
        }


    }
    private void OnMouseDrag()
    {
        if (!EventSystem.current.IsPointerOverGameObject(0))
        {

            if (checkDragMovement == 2)
            {
                Vector2 cursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 posCam = new Vector2(cursorPosition.x, cursorPosition.y);
                Vector3 target = new Vector3(posCam.x, posCam.y, 0);
                target = new Vector3(((int)(posCam.x / 0.62f)) * 0.62f, ((int)(posCam.y / 0.32f)) * 0.32f, 0);
                transform.position = target;
            }
        }
    }
    private void OnMouseUp()
    {
        if (!EventSystem.current.IsPointerOverGameObject(0))
        {
            if (moveCoroutine != null)
                StopCoroutine(moveCoroutine);
            if (checkDragMovement == 1)
                checkDragMovement = 0;
            if (checkDragMovement == 2)
            {


            }
        }
    }
    public void ConfirmMove()
    {

        MovementDialog.instance.newPosition = transform.position;
        if (gameObject.GetComponent<GroundPlacementCollisionChecker>().hasCollision == true)
            transform.position = MovementDialog.instance.originalPosition;
        else
        {
            PlayerPrefs.SetFloat("x" + gameObject.name, gameObject.transform.position.x);
            PlayerPrefs.SetFloat("y" + gameObject.name, gameObject.transform.position.y);
        }
        Destroy(gameObject.GetComponent<Rigidbody2D>());
        Destroy(gameObject.GetComponent<GroundPlacementCollisionChecker>());
        gameObject.GetComponent<PolygonCollider2D>().isTrigger = false;
        checkDragMovement = 0;
        UpdateSortingOrder();
        MainCamera.Instance.unLockCam();
        MovementDialog.instance.Close();
        PlayerPrefs.SetInt("dangdichuyen", 0);
    }
    public void CancelMove()
    {
        gameObject.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255);
        Destroy(gameObject.GetComponent<Rigidbody2D>());
        Destroy(gameObject.GetComponent<GroundPlacementCollisionChecker>());
        gameObject.GetComponent<PolygonCollider2D>().isTrigger = false;
        MainCamera.Instance.unLockCam();
        transform.position = MovementDialog.instance.originalPosition;
        MovementDialog.instance.Close();
        checkDragMovement = 0;
        PlayerPrefs.SetInt("dangdichuyen", 0);
    }
    public void Rotate()
    {

        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y);
        if (!PlayerPrefs.HasKey("quay" + gameObject.name))
        {
            PlayerPrefs.SetInt("quay" + gameObject.name, 1);
        }
        else
        {
            PlayerPrefs.DeleteKey("quay" + gameObject.name);
        }
    }
}
