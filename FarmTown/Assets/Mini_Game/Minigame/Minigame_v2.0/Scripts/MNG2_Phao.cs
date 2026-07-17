using System.Collections;
using UnityEngine;
using Spine.Unity;

public class MNG2_Phao : MonoBehaviour
{
    [SerializeField] Animator skeleton;
    [SerializeField] GameObject danPrefab;
    [SerializeField] Transform posGun;
    [SerializeField] ParticleSystem effectBan;
    [SerializeField] ParticleSystem khoiden;
    [SerializeField] float powerPhao;
    private bool allowBan;

    public void Ban()
    {
        if (!allowBan)
        {
            StartCoroutine(BanNha());
        }
    }

    IEnumerator BanNha()
    {
        yield return new WaitForSeconds(1f);
        effectBan.gameObject.SetActive(true);
        skeleton.Play("Ban", -1, 0);
        GameObject dan = Instantiate(danPrefab, posGun.position, Quaternion.identity);
        dan.GetComponent<Rigidbody2D>().AddForce(Vector2.right * powerPhao, ForceMode2D.Force);
        yield return new WaitForSeconds(0.4f);
        Destroy(dan);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("da"))
        {
            if (khoiden != null)
            {
                khoiden.gameObject.SetActive(true);
            }
            allowBan = true;
            Destroy(collision.GetComponent<BoxCollider2D>(), 0.3f);
            Destroy(collision.GetComponent<Rigidbody2D>(), 0.3f);
            Destroy(gameObject, 0.3f);
        }
    }
}
