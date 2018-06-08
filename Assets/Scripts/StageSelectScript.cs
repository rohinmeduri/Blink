using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageSelectScript : MonoBehaviour {
    public Sprite stage;
    public Image preview;
    public Sprite frame;

	// Use this for initialization
    public void stageClick()
    {
        preview.GetComponent<Image>().sprite = stage; // establishes stage sprite
        GameObject.Find("Stage Data Holder").GetComponent<StageDataScript>().stage = stage; //sets StageDataScript.stage = to current stage
        GameObject.Find("Stage Data Holder").GetComponent<StageDataScript>().frame = frame; // sets StageDataScript.stage = to current frame
    }
}
