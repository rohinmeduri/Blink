using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CSSUIObject: MonoBehaviour {

    //DataForCharacter
    public CharacterDataObject data;

    //UI Elements
    public GameObject DifficultySelection;
    public Text difficultyText;
    public Button increaseDifficultyButton;
    public Button decreaseDifficultyButton;

    public GameObject CharacterSelection;
    public Button increaseCharacterButton;
    public Button decreaseCharacterButton;

    public Text typeText;
    public Button increaseCharacterType;
    public Button decreaseCharacterType;
    public Sprite[] characters;
    public Image preview;

    //constants for use in UI
    private static string[] types = { "Player", "AI", "None" };
    private static string[] difficulties = { "Easy", "Normal", "Hard" };

    private static int frameCounter = 0;
    private static bool disabled = false;
    private static float difficultyYPosition;

    // Use this for initialization
    void Start () {
        if (!disabled)
        {
            difficultyYPosition = DifficultySelection.GetComponent<RectTransform>().anchoredPosition.y;
        }
        DifficultySelection.GetComponent<RectTransform>().anchoredPosition = new Vector2(DifficultySelection.GetComponent<RectTransform>().anchoredPosition.x, difficultyYPosition + 2000);
        disabled = true;
    }
    private void Update()
    {
        /*frameCounter++;
        if(frameCounter >= 2 && !disabled)
        {
            DifficultySelection.SetActive(false);
            disabled = true;
        }*/
    }

    private void OnEnable()
    {
        //DifficultySelection.SetActive(false);
    }

    public void increaseTypeButtonClick(int characterIndex)  // -1 is left, 1 is right
    {
        data.changeType(characterIndex, 1);
        
        //if AI, enable difficulty settings (but no other type has difficulty enabled)
        if (data.getType(characterIndex) == 1)
        {
            DifficultySelection.SetActive(true);
            DifficultySelection.GetComponent<RectTransform>().anchoredPosition = new Vector2(DifficultySelection.GetComponent<RectTransform>().anchoredPosition.x, difficultyYPosition);
        }
        else
        {
            DifficultySelection.SetActive(false);
        }

        //if none, disable sprite+buttons
        if (data.getType(characterIndex) == 2)
        {
            CharacterSelection.SetActive(false);
        }
        else
        {
            CharacterSelection.SetActive(true);
        }
        typeText.text = types[data.getType(characterIndex)];
    }
    public void decreaseTypeButtonClick(int characterIndex)  // -1 is left, 1 is right
    {
        data.changeType(characterIndex, -1);

        //if AI, enable difficulty settings (but no other type has difficulty enabled)
        if (data.getType(characterIndex) == 1)
        {
            DifficultySelection.SetActive(true);
            DifficultySelection.GetComponent<RectTransform>().anchoredPosition = new Vector2(DifficultySelection.GetComponent<RectTransform>().anchoredPosition.x, difficultyYPosition);
        }
        else
        {
            DifficultySelection.SetActive(false);
        }

        //if none, disable sprite+buttons
        if (data.getType(characterIndex) == 2)
        {
            CharacterSelection.SetActive(false);
        }
        else
        {
            CharacterSelection.SetActive(true);
        }
        typeText.text = types[data.getType(characterIndex)];
    }

    public void increaseCharacterButtonClick(int characterIndex)
    {
        data.changeCharacter(characterIndex, 1);
        preview.GetComponent<Image>().sprite = characters[data.getCharacter(characterIndex)];
        Debug.Log("characterIndex: " + characterIndex);
        //STILL NEED TO CHANGE SPRITE AND NAME SPRITE BASED ON CODE
    }
    public void decreaseCharacterButtonClick(int characterIndex)
    {
        data.changeCharacter(characterIndex, -1);
        preview.GetComponent<Image>().sprite = characters[data.getCharacter(characterIndex)];
        Debug.Log("characterIndex: " + characterIndex);
        //STILL NEED TO CHANGE SPRITE AND NAME SPRITE BASED ON CODE
    }

    public void increaseDifficultyButtonClick(int characterIndex)
    {
        data.changeAIDifficulty(characterIndex, 1);
        difficultyText.text = difficulties[data.getAIDifficulty(characterIndex)];
    }
    public void decreaseDifficultyButtonClick(int characterIndex)
    {
        data.changeAIDifficulty(characterIndex, -1);
        difficultyText.text = difficulties[data.getAIDifficulty(characterIndex)];
    }
}
