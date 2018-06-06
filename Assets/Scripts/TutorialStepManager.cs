using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text.RegularExpressions;

public class TutorialStepManager : MonoBehaviour {

    public GameObject airDummy;
    public GameObject reversalAI;

    public int tutorialStep = 0; //current step tutorial is on

    private bool nextStep = false; //is the tutorial moving on to the next step

    protected string[] tutorialText; //will contain the tutorial instructions
	
	void Start () {

        tutorialText = read();

        /*for (int i = 0; i < tutorialText.Length; i++){
            Debug.Log(tutorialText[i]);
        }*/
	}

    void Update(){
        /*if(nextStep){
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
                case 4:
                    reversal();
                case 5:
                    super();
                default:
                    break;
            }
        }*/
    }

    string[] read() { //reads text file and organizes each line into a string array

        FileStream fStream = new FileStream("Assets/Resources/TutorialText.txt", FileMode.Open);
        string instructions = "";
        using (StreamReader fReader = new StreamReader(fStream, true))
        {
            instructions = fReader.ReadToEnd();
        }

        string [] result = Regex.Split(instructions, "\r\n?|\n", RegexOptions.Singleline); //checks for "enter" in the text file, and splits string accordingly

        return result; 
    }

    void uAir(){
        
    }

    void dAir(){
        
    }

    void blink(){
        
    }

    void reversal(){
        
    }

    void super(){
        
    }
}
