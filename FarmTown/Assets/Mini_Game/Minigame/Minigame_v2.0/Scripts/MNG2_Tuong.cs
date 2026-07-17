using UnityEngine;

public class MNG2_Tuong : MonoBehaviour
{
    [SerializeField] ParticleSystem effectKhoiden;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("ground"))
        {
            ShakeCamera.instance.Rung();
            effectKhoiden.gameObject.SetActive(true);
            MNG2_Player.instance.TienBay();
            Destroy(GetComponent<BoxCollider2D>());
            Destroy(GetComponent<Rigidbody2D>());
        }
    }
}
