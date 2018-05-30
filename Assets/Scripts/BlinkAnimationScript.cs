using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkAnimationScript : MonoBehaviour {
    public const float ANIMATION_TIME = 0.188f;

    private float timeCounter = 0;
	
	// Update is called once per frame
	void Update () {
        timeCounter += Time.deltaTime;
        if(timeCounter >= ANIMATION_TIME)
        {
            Destroy(gameObject);
        }
    }
}
