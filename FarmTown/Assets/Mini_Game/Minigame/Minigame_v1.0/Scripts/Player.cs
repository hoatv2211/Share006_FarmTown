using Spine.Unity;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour, IPreviousPosition, IBusy
{
    public static Player instance;
    private Vector3 prevPos;
    private bool busy;

    [SerializeField] private int heathCurrent;
    [SerializeField] private Text heathText;
    public Animator skeletonAnimation;
    public ParticleSystem roundHitBlue;
    public ParticleSystem khoi;
    public ParticleSystem soulExplosionOrange;

    private bool canDestroyFloor;
    private GameObject floorToDestroy;
    [SerializeField] private int indexFloor;
    [SerializeField] private Animator animatorFloor;
    public Animator animNhun;

    public bool canAttack;

    private void Awake()
    {
        instance = this;
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        Invoke("DelayPlay", 1f);
    }
    private void OnEnable()
    {
        this.RegisterListener((int)EventID.OnDeath, OnDeathHandle);
    }

    private void OnDisable()
    {
        EventDispatcher.Instance?.RemoveListener((int)EventID.OnDeath, OnDeathHandle);
    }
    private void OnDeathHandle(object obj)
    {
        Invoke("DelayDeath", 1);
    }
    void DelayDeath()
    {
        gameObject.Recycle();
        this.PostEvent((int)EventID.OnGameLost);
    }
    void DelayPlay()
    {
        busy = true;
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        heathText.text = heathCurrent.ToString();
        animatorFloor.SetFloat("floor", indexFloor);
    }
    private void OnMouseDrag()
    {
        if (!busy && !GameManagerMiniGame.IsWin && !GameManagerMiniGame.IsLost)
        {
            canAttack = false;
            Vector3 newPos = GameManagerMiniGame.Camera.ScreenToWorldPoint(Input.mousePosition);
            newPos.z = -1;
            transform.position = newPos;
            if (GameManagerMiniGame.instance.objTutorial != null)
            {
                GameManagerMiniGame.instance.objTutorial.SetActive(false);
                GameManagerMiniGame.instance.objTutorial = null;
            }
        }
    }

    private void OnMouseUp()
    {
        if (busy)
            return;
        canAttack = true;
        prevPos.z = -1;
        StartCoroutine(Nhun1Phat());
        if (canDestroyFloor && floorToDestroy != null)
        {
            if (floorToDestroy.transform != transform.parent)
            {
                StartCoroutine(WaitingForX00ms());
                indexFloor++;
                canDestroyFloor = false;
                this.PostEvent((int)EventID.OnFxFloorHouse, indexFloor);
                animatorFloor.SetFloat("floor", indexFloor);
            }
        }
        if(transform.parent.GetComponent<Floor>() != null)
        {
            if (transform.parent.GetComponent<Floor>().enemies.Count > 0)
            {
                Vector3 newPos = transform.parent.GetComponent<Floor>().enemies[0].GetCheckpoint();
                newPos.z = -1;
                transform.position = newPos;
                transform.parent.GetComponent<Floor>().select.SetActive(false);
            }
            else
            {
                transform.position = prevPos;
            }
        }
        else
        {
            transform.position = prevPos;
        }

    }

    IEnumerator WaitingForX00ms()
    {
        if (floorToDestroy.GetComponent<Floor>().floorWhere == FloorWhere.Middle)
        {
            floorToDestroy.GetComponent<Rigidbody2D>().gravityScale = 0;
        }
        floorToDestroy.GetComponent<BoxCollider2D>().enabled = false;
        yield return new WaitForSeconds(0.3f);
        Destroy(floorToDestroy);
    }

    IEnumerator Nhun1Phat()
    {
        animNhun.SetBool("nhun", true);
        yield return new WaitForSeconds(1);
        animNhun.SetBool("nhun", false);
    }

    public void SetPreviousPosition(Vector3 pos)
    {
        prevPos = pos;
    }

    public void SetBusy(bool busy)
    {
        this.busy = busy;
    }

    public void AddHeathCurrent(int heath)
    {
        heathCurrent += heath;
        heathText.text = heathCurrent.ToString();
    }

    public int GetHeathCurrent()
    {
        return heathCurrent;
    }

    public void SetCanDestroyFloor(GameObject floorToDestroy)
    {
        canDestroyFloor = true;
        this.floorToDestroy = floorToDestroy;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("mong_house"))
        {
            busy = false;
            ShakeCamera.instance.Rung();
            GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
            GetComponent<BoxCollider2D>().isTrigger = true;
            prevPos = transform.position;
            StartCoroutine(DeactiveKhoi());
        }
    }

    IEnumerator DeactiveKhoi()
    {
        khoi.gameObject.SetActive(true);
        animNhun.SetBool("nhun", true);
        yield return new WaitForSeconds(1f);
        khoi.gameObject.SetActive(false);
        animNhun.SetBool("nhun", false);
        if (GameManagerMiniGame.instance.objTutorial != null)
        {
            GameManagerMiniGame.instance.objTutorial.SetActive(true);
        }
    }

    [ContextMenu("UpdateHeath")]
    public void Tesst()
    {
        heathText.text = heathCurrent.ToString();
    }
}
