using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecorationItemBuilding : MonoBehaviour {
    [SerializeField] int id;
	// Use this for initialization
	void Start () {
        Vector3 temp = transform.TransformDirection(transform.position);
        if(id==0)
            gameObject.GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt((13 - temp.y) * 100f);
        if (id == 1)
        {
            
            gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt((13 - temp.y) * 100f);
            gameObject.transform.GetChild(1).GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt((13 - temp.y) * 100f) + 1;
        }
    }
	
	
}
