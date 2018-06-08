using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class setPlayerID : MonoBehaviour {

	// Use this for initialization
	void Start () {
        gameObject.GetComponent<LocalPlayerScript>().setColor(0); //sets player color
        gameObject.GetComponent<LocalPlayerScript>().setPlayerID(1); //sets player ID
        gameObject.GetComponent<LocalPlayerScript>().setPlayerType("Mage"); //sets player selection type
	}
	
}
