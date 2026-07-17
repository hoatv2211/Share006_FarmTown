using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class OrderBoard : MonoBehaviour {
    public bool check;
    private void Start()
    {
        Vector3 temp = transform.position;
        gameObject.GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt((13 - temp.y) * 100f);
        transform.GetChild(1).GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt((13 - temp.y) * 100f) + 1;
        transform.GetChild(2).GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt((13 - temp.y) * 100f) + 1;
        transform.GetChild(3).GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt((13 - temp.y) * 100f) + 1;
        transform.GetChild(4).GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt((13 - temp.y) * 100f) + 1;
        transform.GetChild(5).GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt((13 - temp.y) * 100f) + 1;
        transform.GetChild(6).GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt((13 - temp.y) * 100f) + 1;
        transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt((13 - temp.y) * 100f) + 2;
        transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt((13 - temp.y) * 100f) + 2;
    }
    private void OnMouseUpAsButton()
    {
        if (!EventSystem.current.IsPointerOverGameObject(0))
        {
            if (PlayerPrefs.GetInt("dangdichuyen") == 0)
            {
                OrderPanel.instance.Open();
            }
        }
    }
}
