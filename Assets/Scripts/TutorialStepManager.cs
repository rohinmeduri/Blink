using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class TutorialStepManager : MonoBehaviour {

    public int tutorialStep = 0;

    protected string[] tutorialText = new string[11];
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void read() {
        StreamReader textInput = new StreamReader("Assets\\Resources\\tutorialText.txt");
    }
}
