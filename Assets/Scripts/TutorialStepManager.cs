using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text.RegularExpressions;

public class TutorialStepManager : MonoBehaviour {

    public GameObject airDummy;
    public GameObject reversalAI;
    public GameObject localPlayer = GameObject.Find("Local Player");

    public int tutorialStep = 0; //current step tutorial is on
    public int line = 0;

    private bool nextStep = false; //is the tutorial moving on to the next step
    private string text; 
    private string[] tutorialText; //will contain the tutorial instructions
	
	void Start () {

        tutorialText = read();

        for (int i = 0; i < tutorialText.Length; i++){
            Debug.Log(tutorialText[i]);
        }
        intro();
	}

    void Update(){
        if (nextStep)
        {
            switch (tutorialStep)
            {
                case 0:
                    intro();
                    break;
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

    public bool getNextStep() {
        return nextStep;
    }

    public void setNextStep(bool next)
    {
        nextStep = next;
    }

    public int getTutorialStep()
    {
        return tutorialStep;
    }

    void intro() {
        while (Input.GetAxisRaw(("Jump" + gameObject.GetComponent<LocalPlayerScript>().getControllerID())) == 0) {
            displayLine(line);
        }
        while (Input.GetAxisRaw(("Jump" + gameObject.GetComponent<LocalPlayerScript>().getControllerID())) == 0)
        {
            displayLine(line);
        }
        while (Input.GetAxisRaw(("Jump" + gameObject.GetComponent<LocalPlayerScript>().getControllerID())) == 0)
        {
            displayLine(line);
        }
    }

    void uAir()
    {
        
        if (Input.GetAxisRaw(("Attack" + gameObject.GetComponent<LocalPlayerScript>().getControllerID())) != 0 && Input.GetAxisRaw(("Vertical" + gameObject.GetComponent<LocalPlayerScript>().getControllerID())) != 0)
        {
            nextStep = true;
        }
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

    void displayLine(int i) {
        text = tutorialText[i];
        line++;
    }
}
