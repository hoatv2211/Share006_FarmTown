using UnityEngine;
using System.Collections;

public class MNG2_Da : MonoBehaviour
{
    [SerializeField] string nameTag = "";
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(nameTag))
        {
            ShakeCamera.instance.Rung();
            GetComponent<Rigidbody2D>().AddTorque(1000);
        }
    }
}
