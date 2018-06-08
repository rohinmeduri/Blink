using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageAndFrameResetter : MonoBehaviour {
    public Sprite defaultStage;
    public Sprite defaultFrame;

	// Use this for initialization
	void Start () {
        GameObject.Find("Stage Data Holder").GetComponent<StageDataScript>().stage = defaultStage;
        GameObject.Find("Stage Data Holder").GetComponent<StageDataScript>().frame = defaultFrame;
    }
}
