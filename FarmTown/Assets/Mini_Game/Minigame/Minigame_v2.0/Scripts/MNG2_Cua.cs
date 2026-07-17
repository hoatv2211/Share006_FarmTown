using UnityEngine;
using DG.Tweening;

public class MNG2_Cua : MonoBehaviour
{
    public static MNG2_Cua instance;

    [SerializeField] MNG2_Key[] keys;

    private void Awake()
    {
        instance = this;
    }

    public void MoCua()
    {
        bool allow = false;
        for (int i = 0; i < keys.Length; i++)
        {
            if (keys[i].enable)
            {
                allow = true;
            }
            else
            {
                allow = false;
                break;
            }
        }
        if (allow)
        {
            transform.DOMoveY(transform.position.y + 3, 0.5f);
            Destroy(gameObject.GetComponent<BoxCollider2D>());
        }
    }
}
