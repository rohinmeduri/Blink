using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GetComponent<SpriteRenderer>().sprite = GameObject.Find("Stage Data Holder").GetComponent<StageDataScript>().stage;
	}
}
