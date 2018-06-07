using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitiateCountdown : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GetComponent<AudioSource>().PlayOneShot(GetComponent<AudioSource>().clip);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
