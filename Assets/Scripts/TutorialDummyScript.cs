using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialDummyScript : LocalPlayerScript {

	// Use this for initialization
	override protected void Start () {
        setPlayerID(2);
        base.Start();
	}

    // Update is called once per frame
    protected override void assignInputs()
    {
        deadInputs();
    }
}
