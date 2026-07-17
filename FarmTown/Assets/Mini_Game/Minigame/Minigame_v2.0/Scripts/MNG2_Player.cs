using UnityEngine;
using Spine.Unity;
using System.Collections;
using DG.Tweening;
using System;

public class MNG2_Player : MonoBehaviour
{
    public static MNG2_Player instance;

    [SerializeField] Animator skeleton;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] bool go;
    [SerializeField] float distanceRay;
    [SerializeField] ParticleSystem effectTienBay;

    public ParticleSystem effectChet;
    public int keyToMove;

    private void Awake()
    {
        instance = this;
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
        effectChet.gameObject.SetActive(true);
        Stop();
        Invoke("Delay", 1f);
    }
    void Delay()
    {
        gameObject.Recycle();
        this.PostEvent((int)EventID.OnGameLost);
    }
    private void Start()
    {
        Invoke("DelayPlay", 1f);
    }
    void DelayPlay()
    {
        StartCoroutine(Chay1Xiu());
    }
    private void FixedUpdate()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position - new Vector3(0.5f, 0.1f, 0), Vector2.down, distanceRay);
        if (go)
        {
            if (hit.collider != null)
            {
                rb.linearVelocity = Vector2.right * 4;
            }
            else
            {
                rb.linearVelocity = Vector2.down * 4;
            }
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    public void GoGo()
    {
        skeleton.Play("Run", -1, 0);
        go = true;
    }

    public void Stop()
    {
        skeleton.Play("Idle", -1, 0);
        go = false;
    }

    IEnumerator Chay1Xiu()
    {
        GoGo();
        yield return new WaitForSeconds(1);
        Stop();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "coin":
                DOTween.Kill(collision.transform);
                Destroy(collision.gameObject);
                collision.transform.GetComponent<CircleCollider2D>().enabled = false;
                break;
            case "dan":
            case "da":
                this.PostEvent((int)EventID.OnDeath, this);
                break;
            case "duiga":
                Destroy(collision.gameObject);
                break;
        }
    }

    public void SetDistanceRay(float value)
    {
        distanceRay = value;
    }
    GameObject[] rootCoin;
    public void TienBay()
    {
        rootCoin = GameObject.FindGameObjectsWithTag("coin");

        for (int i = 0; i < rootCoin.Length; i++)
        {
            rootCoin[i].transform.DOMove(transform.position, 0.5f).OnComplete(() =>
            {
                DestroyCoin();
            });
        }
        effectTienBay.gameObject.SetActive(true);
    }
    void DestroyCoin()
    {
        for (int i = 0; i < rootCoin.Length; i++)
        {
            if (rootCoin[i] != null)
                rootCoin[i].Recycle();
        }
    }
}
