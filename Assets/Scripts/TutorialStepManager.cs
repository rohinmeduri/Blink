using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Text.RegularExpressions;

public class TutorialStepManager : MonoBehaviour
{

    public GameObject localPlayer; //Local Player Object
    public GameObject tutorialDummy; //Tutorial Dummy Question

    private int tutorialStep; //current step tutorial is on
    public Text instructions; //Text box being displayed 

    CanvasGroup endScreen;


    private int line = 0; 

    private bool moveRight = false; //moveRight and moveLeft are used to determine  
    private bool moveLeft = false; //if the player has properly tested out horizaontal movement
    private bool canJump = true; //variable used to prevemt jump from being held  
    private string[] tutorialText; //will contain the tutorial instructions

    void Start()
    {
        tutorialStep = 0;
        localPlayer = GameObject.Find("Local Player"); //reference to player-controlled sprite
        tutorialDummy = GameObject.Find("TutorialDummy"); //reference to tutorial dummy

        tutorialText = read(); //parses out each line of text file into a string array

        tutorialDummy.SetActive(false); //temporarily deactivates tutorial dummy

        endScreen = GameObject.Find("End Screen").GetComponent<CanvasGroup>(); //defines endScreen
    }

    void Update()
    {
        switch (tutorialStep) //runs different tutorial logic depending on what step the player is currently on
        {
            case 0:
                checkInputA();
                break;
            case 1:
                checkInputA();
                break;
            case 2:
                movement();
                break;
            case 3:
                jump();
                break;
            case 4:
                attack();
                break;
            case 5:
                blink();
                break;
            case 6:
                reversal();
                break;
            case 7:
                glory();
                break;
            case 8:
                attackDummy();
                break;
            case 9:
                notice();
                break;
            case 10:
                combo();
                break;
            case 11:
                super();
                break;
            case 12:
                endScreenDisplay();
                break;
            default:
                break;
        }

    }

    string[] read()
    { //reads text file and organizes each line into a string array

        FileStream fStream = new FileStream("Assets/Resources/TutorialText.txt", FileMode.Open); //loads file Tutorial.txt
        string instructions = "";
        using (StreamReader fReader = new StreamReader(fStream, true))
        {
            instructions = fReader.ReadToEnd(); //puts contents of Tutorial.txt into one string
        }

        string[] result = Regex.Split(instructions, "\r\n?|\n", RegexOptions.Singleline); //checks for "enter" in the text fil
                                                                                          //and splits string accordingly

        return result;
    }
    void checkInputA()
    {
        localPlayer.GetComponent<LocalPlayerScript>().disableJump(); //disables jump
        if (Input.GetAxisRaw("Jump1") != 0 && canJump)
        {
            canJump = false;
            instructions.text = tutorialText[line]; //displays text from array
            line++; //advances "line" used to determine place in tutorialText array
            tutorialStep++; //advances tutorialStep to move on in the switch-case statement
        }
        if (Input.GetAxisRaw("Jump1") == 0) canJump = true; //allows jumpInput
    }

    void movement()
    {
        instructions.text = tutorialText[line];
        if (Input.GetAxisRaw("Horizontal1") > 0) //checks moving right
        {
            moveRight = true;
        }
        if (Input.GetAxisRaw("Horizontal1") < 0) //checks moving left
        {
            moveLeft = true;
        }
        if (moveLeft && moveRight) //checks movement practice
        {
            tutorialStep++;
            line++;
        }
    }
    void jump()
    {
        instructions.text = tutorialText[line];
        if (Input.GetAxisRaw("Jump1") != 0)
        {
            line++;
            tutorialStep++;
        }
    }

    void attack()
    {
        instructions.text = tutorialText[line];
        if (Input.GetAxisRaw("Attack1") != 0) //looks for attack input
        {
            tutorialStep++;
            line++;
        }
    }

    void blink()
    {
        instructions.text = tutorialText[line];
        if (Input.GetAxisRaw("Blink1") != 0) //looks for blink input
        {
            tutorialStep++;
            line++;
        }
    }

    void reversal()
    {
        tutorialDummy.SetActive(true);
        instructions.text = tutorialText[line];
        if (localPlayer.GetComponent<LocalPlayerScript>().reversaled()) // looks for successful Blink
        {
            tutorialStep++;
            line++;
            instructions.text = tutorialText[line];
        }
    }

    void super()
    {
            instructions.text = tutorialText[line];
            if (localPlayer.GetComponent<LocalPlayerScript>().getKills() > 0) {
                tutorialStep++;
            } 
    }

    void glory()
    {
        checkInputA();
    }

    void notice()
    {
        checkInputA();
        instructions.text = tutorialText[line];
    }

    void attackDummy()
    {
        attack();
        instructions.text = tutorialText[line];
    }

    void combo()
    {
        if (localPlayer.GetComponent<LocalPlayerScript>().canSuper()) {//checks for filled glory meter
            tutorialStep++;
            line++;
        }
    }

    void endScreenDisplay() {
        endScreen.alpha = 1; //sets transparency of window to opague 
        setInteractable(endScreen, true); //allows interaction with end window
        endScreen.GetComponent<RectTransform>().SetAsLastSibling();
    }

    private void setInteractable(CanvasGroup go, bool interactable)
    {
        go.interactable = interactable;
        foreach (Button child in go.GetComponentsInChildren<Button>())
        {
            child.interactable = interactable;
        }
    }
}
