using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int heathCurrent;
    [SerializeField] private Animator skeletonAnimation;
    [SerializeField] private Text textHeath;
    [SerializeField] private Transform checkpoint;
    [SerializeField] private GameObject roundHitRed;
    [SerializeField] private GameObject soulExplosionOrange;

    public Vector3 GetCheckpoint()
    {
        return checkpoint.transform.position;
    }

    private void Start()
    {
        textHeath.text = heathCurrent.ToString();
        skeletonAnimation.speed = Random.Range(0.8f, 1.2f);
        transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = Mathf.Abs((int)((100 - transform.position.x) * 100));
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (Player.instance.canAttack)
        {
            Player.instance.canAttack = false;
            collision.GetComponent<IBusy>()?.SetBusy(true);
            StartCoroutine(Attack());
        }
    }

    IEnumerator Attack()
    {
        yield return new WaitForSeconds(0.5f);
        skeletonAnimation.Play("Attack", -1, 0);
        Player.instance.skeletonAnimation.Play("Attack", -1, 0);
        Player.instance.roundHitBlue.gameObject.SetActive(true);
        roundHitRed.SetActive(true);
        yield return new WaitForSeconds(2);
        Player.instance.skeletonAnimation.Play("Idle", -1, 0);
        Player.instance.roundHitBlue.gameObject.SetActive(false);
        roundHitRed.SetActive(false);
        if (Player.instance.GetHeathCurrent() > heathCurrent)
        {
            Player.instance.AddHeathCurrent(heathCurrent);
            transform.GetComponentInParent<Floor>().DestroyEnemy(this);
            Player.instance.khoi.gameObject.SetActive(true);
            soulExplosionOrange.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            Player.instance.SetBusy(false);
            Player.instance.khoi.gameObject.SetActive(false);
            Destroy(gameObject);
        }
        else
        {
            skeletonAnimation.Play("Idle", -1, 0);
            Player.instance.soulExplosionOrange.gameObject.SetActive(true);
            heathCurrent += Player.instance.GetHeathCurrent();
            textHeath.text = heathCurrent.ToString();
            Player.instance.AddHeathCurrent(-Player.instance.GetHeathCurrent());
            yield return new WaitForSeconds(1);
            this.PostEvent((int)EventID.OnDeath);
        }
    }

    [ContextMenu("UpdateHeath")]
    public void Tesst()
    {
        textHeath.text = heathCurrent.ToString();
    }
}
