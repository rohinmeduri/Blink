using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Text.RegularExpressions;

public class TutorialStepManager : MonoBehaviour
{

    public GameObject localPlayer;
    public GameObject tutorialDummy;

    private int tutorialStep; //current step tutorial is on
    public Text instructions;

    CanvasGroup endScreen;


    private int line = 0;

    private bool moveRight = false;
    private bool moveLeft = false;
    private bool canJump = true;
    private string[] tutorialText; //will contain the tutorial instructions

    void Start()
    {
        tutorialStep = 0;
        localPlayer = GameObject.Find("Local Player");
        tutorialDummy = GameObject.Find("TutorialDummy");

        tutorialText = read();

        for (int i = 0; i < tutorialText.Length; i++)
        {
            Debug.Log(tutorialText[i]);
        }

        tutorialDummy.SetActive(false);

        endScreen = GameObject.Find("End Screen").GetComponent<CanvasGroup>();
    }

    void Update()
    {
        switch (tutorialStep) //runs different 
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

        FileStream fStream = new FileStream("Assets/Resources/TutorialText.txt", FileMode.Open);
        string instructions = "";
        using (StreamReader fReader = new StreamReader(fStream, true))
        {
            instructions = fReader.ReadToEnd();
        }

        string[] result = Regex.Split(instructions, "\r\n?|\n", RegexOptions.Singleline); //checks for "enter" in the text file, and splits string accordingly

        return result;
    }
    void checkInputA()
    {
        localPlayer.GetComponent<LocalPlayerScript>().disableJump();
        if (Input.GetAxisRaw("Jump1") != 0 && canJump)
        {
            canJump = false;
            instructions.text = tutorialText[line];
            line++;
            tutorialStep++;
        }
        if (Input.GetAxisRaw("Jump1") == 0) canJump = true;
    }

    void movement()
    {
        instructions.text = tutorialText[line];
        if (Input.GetAxisRaw("Horizontal1") > 0)
        {
            moveRight = true;
        }
        if (Input.GetAxisRaw("Horizontal1") < 0)
        {
            moveLeft = true;
        }
        if (moveLeft && moveRight)
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
        Debug.Log("jump" + tutorialStep + ", " + line);
    }

    void attack()
    {
        instructions.text = tutorialText[line];
        if (Input.GetAxisRaw("Attack1") != 0)
        {
            tutorialStep++;
            line++;
        }
    }

    void blink()
    {
        Debug.Log(tutorialStep);
        instructions.text = tutorialText[line];
        if (Input.GetAxisRaw("Blink1") != 0)
        {
            tutorialStep++;
            line++;
        }
    }

    void reversal()
    {
        tutorialDummy.SetActive(true);
        instructions.text = tutorialText[line];
        if (localPlayer.GetComponent<LocalPlayerScript>().reversaled())
        {

            tutorialStep++;
            line++;
            instructions.text = tutorialText[line];
        }
        Debug.Log(tutorialStep);
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
        if (localPlayer.GetComponent<LocalPlayerScript>().canSuper()) {
            tutorialStep++;
            line++;
        }
    }

    void endScreenDisplay() {
        endScreen.alpha = 1;
        setInteractable(endScreen, true);
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
