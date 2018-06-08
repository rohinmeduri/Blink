using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//script used to assign the frame that was chosen based on the stage select screen
public class FrameScript : MonoBehaviour {
	void Start () {
        GetComponent<SpriteRenderer>().sprite = GameObject.Find("Stage Data Holder").GetComponent<StageDataScript>().frame;
	}
}
