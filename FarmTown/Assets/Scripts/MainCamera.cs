using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.EventSystems;
public class MainCamera : MonoBehaviour
{
    public static MainCamera Instance { get; private set; }
    Camera mcam;
    bool bl_2touch = false;
    Vector3 oldPos, panOrigin;
    float speed = 20f;
    Vector2?[] oldTouchPositions =
    {
        null,
        null
    };
    [SerializeField] float maxZoom, minZoom, speedScreen = 5f;
    Vector2 oldTouchVector;
    float oldTouchDistance;
    [SerializeField] float dragSpeed, outerLeft, outerRight;
    public bool camLock, bDragging;
    [SerializeField] bool cameraDragging, camDrag;
    Vector3 dragOrigin;
    [FormerlySerializedAs("mocRight")] [SerializeField] Transform rightBoundary;
    [FormerlySerializedAs("mocLeft")] [SerializeField] Transform leftBoundary;
    [FormerlySerializedAs("mocUp")] [SerializeField] Transform topBoundary;
    [FormerlySerializedAs("mocDown")] [SerializeField] Transform bottomBoundary;
    private Vector3 m_prevPos;
    public bool move;
    public Vector3 posend;
    private bool b_collider = false;
    private bool b_lock = false;
    private bool b_down = false;
    private bool b_autoMove = false;
    private bool b_drag = false;
    private bool b_actionTouch = false;
    private bool b_actionCamera = false;
    bool checkDialog;
    void Awake()
    {
        Instance = this;
        mcam = GetComponent<Camera>();
    }

    void Start()
    {
        posend = transform.localPosition;
    }

    void Update()
    {
        if (!EventSystem.current.IsPointerOverGameObject(0))
        {
            if (camLock == false)
            {
                if (Input.touchCount == 0)
                {
                    oldTouchPositions[0] = null;
                    oldTouchPositions[1] = null;
                    if (Input.GetMouseButtonDown(0)) OnTouchDown();
                    if (Input.GetMouseButton(0)) OnTouchMove();
                    if (Input.GetMouseButtonUp(0)) OnTouchUp();
                }
                else
                    if (Input.touchCount == 1)
                {
                    if (Input.GetTouch(0).phase == TouchPhase.Began)
                    {

                        OnTouchDown();
                    }
                    if (Input.GetTouch(0).phase == TouchPhase.Moved) OnTouchMove();
                    if (Input.GetTouch(0).phase == TouchPhase.Ended) OnTouchUp();

                }
                else
                    if (Input.touchCount == 2)
                {
                    if (oldTouchPositions[1] == null)
                    {
                        oldTouchPositions[0] = Input.GetTouch(0).position;
                        oldTouchPositions[1] = Input.GetTouch(1).position;
                        oldTouchVector = (Vector2)(oldTouchPositions[0] - oldTouchPositions[1]);
                        oldTouchDistance = oldTouchVector.magnitude;
                    }
                    else
                    {
                        Vector2 firstTouchPosition = Input.GetTouch(0).position;
                        Vector2 secondTouchPosition = Input.GetTouch(1).position;
                        Vector2 newTouchVector = firstTouchPosition - secondTouchPosition;
                        float newTouchDistance = newTouchVector.magnitude;

                        mcam.orthographicSize *= oldTouchDistance / newTouchDistance;
                        mcam.orthographicSize = Mathf.Clamp(mcam.orthographicSize, minZoom, maxZoom);

                        oldTouchPositions[0] = firstTouchPosition;
                        oldTouchPositions[1] = secondTouchPosition;
                        oldTouchVector = newTouchVector;
                        oldTouchDistance = newTouchDistance;
                    }
                }
                if (move == false)
                    transform.localPosition = Vector3.MoveTowards(transform.localPosition, posend, speed * Time.deltaTime);
                if (move == true)
                    transform.localPosition = Vector3.MoveTowards(transform.localPosition, posend, speedScreen * Time.deltaTime);
                if (transform.position.x + (mcam.orthographicSize * ((float)Screen.width / Screen.height))
                    - rightBoundary.position.x >= 0)
                {
                    float oldSize = mcam.orthographicSize;
                    float distanceSize = mcam.orthographicSize * ((float)Screen.width / Screen.height) + (mcam.orthographicSize - oldSize) * ((float)Screen.width / Screen.height);
                    transform.position = new Vector3(rightBoundary.position.x - distanceSize, transform.position.y, -10);
                    move = false;
                }

                if (transform.position.x + (mcam.orthographicSize * ((float)Screen.width / Screen.height))
                    - rightBoundary.position.x >= 0)
                {
                    float oldSize = mcam.orthographicSize;
                    float distanceSize = mcam.orthographicSize * ((float)Screen.width / Screen.height) + (mcam.orthographicSize - oldSize) * ((float)Screen.width / Screen.height);
                    transform.position = new Vector3(rightBoundary.position.x - distanceSize, transform.position.y, -10);
                    move = false;
                }

                if ((transform.position.x - (mcam.orthographicSize * Screen.width / Screen.height))
                    - leftBoundary.position.x <= 0)
                {
                    float oldSize = mcam.orthographicSize;
                    float distanceSize = mcam.orthographicSize * ((float)Screen.width / Screen.height) + (mcam.orthographicSize - oldSize) * ((float)Screen.width / Screen.height);
                    transform.position = new Vector3(leftBoundary.position.x + distanceSize, transform.position.y, -10);
                    move = false;
                }

                if ((transform.position.y + mcam.orthographicSize) - topBoundary.position.y >= 0)
                {
                    float oldSize = mcam.orthographicSize;
                    float distanceSize = mcam.orthographicSize + (mcam.orthographicSize - oldSize) * mcam.orthographicSize;
                    transform.position = new Vector3(transform.position.x, topBoundary.position.y - distanceSize, -10);
                    move = false;
                }

                if ((transform.position.y - mcam.orthographicSize) - bottomBoundary.position.y <= 0)
                {
                    float oldSize = mcam.orthographicSize;
                    float distanceSize = mcam.orthographicSize + (mcam.orthographicSize - oldSize) * mcam.orthographicSize;
                    transform.position = new Vector3(transform.position.x, bottomBoundary.position.y + distanceSize, -10);
                    move = false;
                }
            }
            else
            //if (camLock == true)
            {

            }
            if (move)
            {

                // transform.position = Vector3.MoveTowards(transform.position, posend, speedScreen * Time.deltaTime);
                if (Vector3.Distance(transform.position, posend) <= 0.05f)
                {
                    move = false;
                    if (checkDialog)
                    {
                        camLock = true;
                    }
                }
            }

        }

        if (moveNM)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, posend, speedScreen * Time.deltaTime);
            if (Vector3.Distance(transform.position, posend) <= 0.05f)
            {
                moveNM = false;
            }
        }
    }
    bool moveNM;
    public void moveKhiCamLock(Vector3 pos)
    {
        camLock = true;
        moveNM = true;
        pos.z = -10;
        posend = pos;

    }
    public void DisableAll()
    {
        SeedSelectionPanel.instance.close();
        DialogShop.instance.close();
        SickleTool.instance.close();
        InventoryController.instance.Close();
        CropTimerPanel.instance.Close();
        AnimalFeedingPanel.instance.close();
        AnimalTimerPanel.instance.Close();
        MagicTreePanel.instance.Close();
        EnvironmentToolPanel.instance.Close();

    }
    public void CloseSmallDialogs()
    {
        SickleTool.instance.close();
        CropTimerPanel.instance.Close();
        SeedSelectionPanel.instance.close();
        AnimalFeedingPanel.instance.close();
        AnimalTimerPanel.instance.Close();
        MagicTreePanel.instance.Close();
        EnvironmentToolPanel.instance.Close();

    }
    public void lockCam()
    {
        camLock = true;
        if (!PlayerPrefs.HasKey("huongdan"))
            DisableAll();
    }

    public void unLockCam()
    {
        camLock = false;
    }
    public void MoveCamera(Vector3 posEnd, bool check)
    {
        move = true;
        posEnd.z = -10;
        posend = posEnd;
        checkDialog = check;
    }

    private void OnTouchDown()
    {
        bool blOnUI = MouseOnUI();
        if (!blOnUI)
        {
            Vector3 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            m_prevPos = transform.InverseTransformPoint(target);
            RaycastHit2D hit = Physics2D.Raycast(target, Vector2.zero);
            if (hit.collider) b_collider = true;
            b_down = true;
        }
        else if (blOnUI)
        {
            bDragging = true;
            Vector3 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            m_prevPos = transform.InverseTransformPoint(target);
        }
    }
    private void OnTouchMove()
    {
        if (!b_lock)
        {
            if (!PlayerPrefs.HasKey("huongdan"))
                DisableAll();
            Vector3 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 targetLocal = transform.InverseTransformPoint(target);
            if (!b_drag)
            {
                if (Vector3.Distance(m_prevPos, targetLocal) > 0.1f)
                {
                    b_drag = true;
                    m_prevPos = targetLocal;
                    if (b_actionTouch)
                    {
                        // ActionTouchHandle();
                    }
                }
            }
            else if (b_drag)
            {
                Vector3 distane = targetLocal - m_prevPos;
                posend = transform.localPosition - distane;
                m_prevPos = targetLocal;
            }
        }
    }
    private void OnTouchUp()
    {
        if (b_down)
        {
            b_down = false;
            bDragging = false;
            if (!b_collider)
            {
                if (!b_drag)
                {
                    Vector3 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    target.z = 0;
                }
                else if (b_drag)
                {
                    if (!PlayerPrefs.HasKey("huongdan"))
                        DisableAll();
                    b_drag = false;
                    Vector3 target = transform.InverseTransformPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                    Vector3 distane = (target - m_prevPos) * 5;
                    posend = transform.localPosition - distane;
                }
            }
            else
            {

                b_collider = false;
            }
        }
    }
    public bool MouseOnUI()
    {
        PointerEventData pointerEvent = new PointerEventData(EventSystem.current);
        pointerEvent.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEvent, results);
        return results.Count > 0;
    }
    //public void ActionTouchHandle()
    //{
    //    if (b_actionTouch == true)
    //    {
    //        actionTouch();
    //        actionTouch = null;
    //        b_actionTouch = false;
    //    }
    //}
}
