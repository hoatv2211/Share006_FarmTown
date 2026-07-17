using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LocalizedText : MonoBehaviour {
    public string vni, eng;
	
	void Awake () {
        if (Application.systemLanguage == SystemLanguage.Vietnamese)
            gameObject.GetComponent<Text>().text = vni;
        else
            gameObject.GetComponent<Text>().text = eng;
    }
	
}

