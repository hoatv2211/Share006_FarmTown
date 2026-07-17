using UnityEngine;

public class MNG2_CauLv4 : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("da"))
        {
            if (collision.GetComponent<CircleCollider2D>() != null)
            {
                collision.GetComponent<CircleCollider2D>().isTrigger = true;
            }
            GetComponent<BoxCollider2D>().isTrigger = false;
        }
    }
}
