using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageSelectScript : MonoBehaviour {
    public Sprite stage;
    public Image preview;

	// Use this for initialization
    public void stageClick()
    {
        preview.GetComponent<Image>().sprite = stage;
        GameObject.Find("Stage Data Holder").GetComponent<StageDataScript>().stage = stage;
    }
}
