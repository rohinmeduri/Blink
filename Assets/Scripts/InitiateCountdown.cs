using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitiateCountdown : MonoBehaviour {

    /**
     * Play Countdown sound effect
     */
	void Start () {
        GetComponent<AudioSource>().PlayOneShot(GetComponent<AudioSource>().clip);
	}
}
