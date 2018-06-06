using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text.RegularExpressions;

public class TutorialStepManager : MonoBehaviour {

    public int tutorialStep = 0;

    protected string[] tutorialText;
	// Use this for initialization
	void Start () {
        tutorialText = read();
        for (int i = 0; i < tutorialText.Length; i++){
            Debug.Log(tutorialText[i]);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    string[] read() {

        FileStream fs = new FileStream("Assets\\Resources\\TutorialText.txt", FileMode.Open);
        string content = "";
        using (StreamReader fReader = new StreamReader(fs, true))
        {
            content = fReader.ReadToEnd();
        }

        string [] result = Regex.Split(content, "\r\n?|\n", RegexOptions.Singleline); //checks for "enter" in the text file, and splits string accordingly

        return result; 
    }
}
