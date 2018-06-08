using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkFloater : MonoBehaviour {
    private float timer = 0f;
	// Update is called once per frame

    /*
     * This script creates the bobbing motion of the blink title on the main menu
     */ 
	void Update () {
        timer += Time.deltaTime;
        GetComponent<RectTransform>().position = new Vector3(GetComponent<RectTransform>().position.x, GetComponent<RectTransform>().position.y + 0.3f * Mathf.Sin(timer * 1.6f));
	}
}
