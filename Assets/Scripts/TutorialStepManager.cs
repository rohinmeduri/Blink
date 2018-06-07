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

    private bool moveRight = false;
    private bool moveLeft = false;
    private bool nextStep = true; //is the tutorial moving on to the next step
    private string[] tutorialText; //will contain the tutorial instructions
	
	void Start () {
        localPlayer = GameObject.Find("Local Player");

        tutorialText = read();

        for (int i = 0; i < tutorialText.Length; i++){
            Debug.Log(tutorialText[i]);
        }
	}

    void Update() { 
            switch (tutorialStep)
            {
                case 1:
                    uAir();
                    break;
                case 2:
                    uAir();
                    break;
                case 3:
                    uAir();
                    break;
                case 4:
                    movement();
                    break;
                case 5:
                    jump();
                    break;
                case 6:
                    attack();
                    break;
                case 7:
                    uAir();
                    break;
                case 8:
                    dAir();
                    break;
                case 9:
                    blink();
                    break;
                case 10:
                    reversal();
                    break;
                case 11:
                    super();
                    break;
                default:
                    break;
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
    void checkInput() {
       localPlayer.GetComponent<LocalPlayerScript>().
       if(Input.GetAxisRaw()) 
    }

    void jump() {

    }

    void attack() {

    }
    IEnumerator introCoroutine(System.Action<bool> callback, bool wait, string action)
    {
        while (wait)
        {
            if (Input.GetAxisRaw(action + 1) != 0)
            {
                displayLine(line);
                wait = false;
                line += 1;
                yield return new WaitForSeconds(1.5f);
                callback(true);
            }
            yield return null;
        }
    }


    void uAir()
    {
        StartCoroutine(waitInput(true, "Jump"));
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