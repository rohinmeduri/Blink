using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Text.RegularExpressions;

public class TutorialStepManager : MonoBehaviour {

    public GameObject localPlayer;

    public int tutorialStep = 0; //current step tutorial is on
    public Text instructions;

    private int line = 0;

    private bool nextStep = true; //is the tutorial moving on to the next step
    private bool input = false;
    private string[] tutorialText; //will contain the tutorial instructions
	
	void Start () {
        localPlayer = GameObject.Find("Local Player");

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
    //Input.GetAxisRaw(("Jump" + localPlayer.GetComponent<LocalPlayerScript>().getControllerID())) == 0
    void intro() {
        StartCoroutine(waitInput(true, "Jump"));
        StopAllCoroutines();
        StartCoroutine(waitInput(true, "Attack"));
        StopAllCoroutines();
        tutorialStep++;
    }

    void uAir()
    {
        StartCoroutine(waitInput(true, "Jump"));
        StartCoroutine(waitInput(true, "Vertical"));
        StopAllCoroutines();

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
        instructions.text = tutorialText[i];
    }

    IEnumerator waitInput(bool wait, string action) {
        while (wait) {
            if (Input.GetAxisRaw(action + 1) != 0) {
                displayLine(line);
                wait = false;
                line += 1;
                yield return new WaitForSeconds(1.5f);
            }
            yield return null;
        }
    }
}
