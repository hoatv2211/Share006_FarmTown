using UnityEngine;
using Spine.Unity;
using System.Collections;

public class MNG2_Dragon : MonoBehaviour
{
    [SerializeField] Animator skeleton;
    [SerializeField] ParticleSystem effectLua;
    [SerializeField] ParticleSystem effectKhoiden;

    private void Start()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            this.PostEvent((int)EventID.OnDeath);
        }
        else if (collision.CompareTag("dan"))
        {
            skeleton.Play("Die", -1, 0);
            effectKhoiden.gameObject.SetActive(true);
            Destroy(gameObject, 0.4f);
        }
    }

    public void FunLua()
    {
        skeleton.Play("Idle", -1, 0);
        StartCoroutine(Cho1Ty());
    }

    IEnumerator Cho1Ty()
    {
        effectLua.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        effectLua.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.3f);
        effectLua.gameObject.SetActive(true);
        this.PostEvent((int)EventID.OnDeath);
        effectLua.gameObject.SetActive(false);
        skeleton.Play("Phunlua", -1, 0);
    }
}
