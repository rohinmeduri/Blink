using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class setPlayerID : MonoBehaviour {

	// Use this for initialization
	void Start () {
        gameObject.GetComponent<LocalPlayerScript>().setPlayerID(1);
	}
	
}
