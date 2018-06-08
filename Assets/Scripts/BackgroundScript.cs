using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundScript : MonoBehaviour {

	/*
     *This script sets the stage background to the one selected in the stage selection screen
     * The stage choice is stored in the 'stage data holder' game object
     */
	void Start () {
        GetComponent<SpriteRenderer>().sprite = GameObject.Find("Stage Data Holder").GetComponent<StageDataScript>().stage;
	}
}
