using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialDummyScript : LocalPlayerScript {

    float time = 0;
    bool canAttack = true;

	// Use this for initialization
	override protected void Start () {
        setPlayerID(2);
        base.Start();
	}
    protected override void Update()
    {
        time += Time.deltaTime;
        if(time > 3 && canAttack)
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
        gravity();
        jump();
        attack();
    }


    protected override void assignInputs()
    {
        deadInputs();
    }
    
}
