using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialDummyScript : LocalPlayerScript {

    float time = 0;
    bool canAttack = true;

	// Use this for initialization
	override protected void Start () {
        setColor(0);    //sets standard color fill of character
        setPlayerID(2); //sets player ID and controller
        setPlayerType("Rebel"); //sets character type
 
        base.Start(); //inherits Start() from LocalPlayerScript
	}
    protected override void Update() //basic attack and phyiscs functions of the dummy
    {
        time += Time.deltaTime;
        if(time > 3 && canAttack) //attacks periodically
        {
            time = 0;
            attackInput = true;
            canAttack = false;
        }
        else
        {
            canAttack = true;
            attackInput = false;
        }
        gravity();   //gravity logic and physics
        jump(); // jump input logic and physics
        attack(); //attacking logic and physics  
    }


    protected override void assignInputs()
    {
        deadInputs(); //overrides assignInputs to set all of the inputs to false and effectively freeze the dummy
    }
    
}
