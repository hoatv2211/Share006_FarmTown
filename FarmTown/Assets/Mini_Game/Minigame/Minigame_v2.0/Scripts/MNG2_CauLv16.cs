using UnityEngine;
using System.Collections;

public class MNG2_CauLv16 : MonoBehaviour
{
    [SerializeField] ParticleSystem khoi;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("da1"))
        {
            StartCoroutine(DelayDestroyDa());
        }
    }

    IEnumerator DelayDestroyDa()
    {
        yield return new WaitForSeconds(0.4f);
        khoi.gameObject.SetActive(true);
        GetComponent<BoxCollider2D>().isTrigger = false;
    }
}
