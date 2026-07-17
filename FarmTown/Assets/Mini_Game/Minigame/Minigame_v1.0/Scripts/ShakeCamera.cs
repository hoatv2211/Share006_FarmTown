using UnityEngine;
using System.Collections;

public class ShakeCamera : MonoBehaviour
{
    public static ShakeCamera instance;

    [SerializeField] Animator anim;

    private void Awake()
    {
        instance = this;
    }

    public void Rung()
    {
        StartCoroutine(OffRung());
    }

    IEnumerator OffRung()
    {
        anim.SetBool("rung", true);
        yield return new WaitForSeconds(1);
        anim.SetBool("rung", false);
    }
}
