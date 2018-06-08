using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//this script handles user input in character selection so the UI elements remain robust/relevant
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

    //hide difficultySelection on start (default option is player, not AI. For some reason, disabling in editor breaks this script)
    void Start () {
        if (!disabled)
        {
            difficultyYPosition = DifficultySelection.GetComponent<RectTransform>().anchoredPosition.y;
        }
        DifficultySelection.GetComponent<RectTransform>().anchoredPosition = new Vector2(DifficultySelection.GetComponent<RectTransform>().anchoredPosition.x, difficultyYPosition + 2000);
        disabled = true;
    }

    //scripts that take user input and change the options available to the player accordingly (for example, show difficulties options if an AI is selected
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
    }
    public void decreaseCharacterButtonClick(int characterIndex)
    {
        data.changeCharacter(characterIndex, -1);
        preview.GetComponent<Image>().sprite = characters[data.getCharacter(characterIndex)];
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
