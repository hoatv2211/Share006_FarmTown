using UnityEngine;

public class MinigameChuong : MonoBehaviour
{
    [SerializeField] GameObject effectTym;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("mat_dat"))
        {
            effectTym.SetActive(true);
        }
    }
}
