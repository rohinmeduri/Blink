using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialSpawner : TutorialDummyScript {

    private int tutorialStep;
	// Use this for initialization

	void Start () {
        tutorialStep = 
	}
	
	// Update is called once per frame
	void Update () {
        if(nextStep){
            switch (tutorialStep)
            {
                case 1:
                    uAir();
                    break;
                case 2:
                    dAir();
                    break;
                case 3:
                    blink();
                    break;
                case 4:
                    reversal();
                    break;
                case 5:
                    super();
                    break;
                default:
                    break;
            }
        }

    }

    void uAir()
    {

    }

    void dAir()
    {

    }

    void blink()
    {

    }

    void reversal()
    {

    }

    void super()
    {

    }
}
