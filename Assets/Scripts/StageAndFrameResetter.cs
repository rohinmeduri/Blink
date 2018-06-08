using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//script that resets the stage and frame in the data holder so that it corresponds to the colliseum which is the default stage that
//shows in the stage select scene
public class StageAndFrameResetter : MonoBehaviour {
    public Sprite defaultStage;
    public Sprite defaultFrame;

	// Use this for initialization
	void Start () {
        GameObject.Find("Stage Data Holder").GetComponent<StageDataScript>().stage = defaultStage;
        GameObject.Find("Stage Data Holder").GetComponent<StageDataScript>().frame = defaultFrame;
    }
}
