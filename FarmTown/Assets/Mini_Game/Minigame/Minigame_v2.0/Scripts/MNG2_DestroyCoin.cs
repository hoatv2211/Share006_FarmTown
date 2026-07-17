using UnityEngine;

public class MNG2_DestroyCoin : MonoBehaviour
{
    [SerializeField] GameObject bridge;
    [SerializeField] ParticleSystem effectKhoiden;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("coin"))
        {
            Destroy(collision.gameObject);
        }
        else if(collision.gameObject.CompareTag("da"))
        {
            effectKhoiden.gameObject.SetActive(true);
            bridge.SetActive(true);
        }
    }
}
