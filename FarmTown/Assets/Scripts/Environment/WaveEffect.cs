using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveEffect : MonoBehaviour {

	// Use this for initialization
	void Start () {
        StartCoroutine(run());
	}
	
	IEnumerator run()
    {
        yield return new WaitForSeconds(Random.Range(0f, .9f));
        gameObject.GetComponent<Animator>().enabled = true;
    }
}

