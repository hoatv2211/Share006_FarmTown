using UnityEngine;
using System.Collections;

public class MNG2_Chuong : MonoBehaviour
{
    [SerializeField] ParticleSystem effectTym;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            MNG2_Player.instance.Stop();
            effectTym.gameObject.SetActive(true);
            StartCoroutine(Win());
        }
    }

    IEnumerator Win()
    {
        yield return new WaitForSeconds(2);
        GameManagerMiniGame.instance.Win();
    }
}
