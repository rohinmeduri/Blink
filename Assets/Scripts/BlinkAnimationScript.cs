using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkAnimationScript : MonoBehaviour {
    public const float ANIMATION_TIME = 0.250f;

    private float timeCounter = 0;
	
	/*
     *This script controls the blink gameObject that is left behind when a player blinks
     */
	void Update () {
        //destroy the gameObject once the blink animation is complete
        timeCounter += Time.deltaTime;
        if(timeCounter >= ANIMATION_TIME)
        {
            Destroy(gameObject);
        }
    }
}
