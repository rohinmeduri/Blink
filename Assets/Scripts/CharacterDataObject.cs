using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this script stores the player selections from the character selection screen
public class CharacterDataObject : MonoBehaviour {

    //variables needed for spawning players and AIs
    private static int[] types;
    private static int[] characters;
    private static int[] AIDifficulties;

    // Use this for initialization
    void Start () {
        types = new int[4];
        characters = new int[4];
        AIDifficulties = new int[4];
	}

    //scripts that respond to player input to change their choices
    public void changeType(int characterIndex, int delta) {
        types[characterIndex] += delta;
        if (types[characterIndex] > 2)
        {
            types[characterIndex] = 0;
        }

        if (types[characterIndex] < 0)
        {
            types[characterIndex] = 2;
        }
    }
    public void changeCharacter(int characterIndex, int delta)
    {
        characters[characterIndex] += delta;
        if (characters[characterIndex] > 2)
        {
            characters[characterIndex] = 0;
        }

        if (characters[characterIndex] < 0)
        {
            characters[characterIndex] = 2;
        }
    }
    public void changeAIDifficulty(int characterIndex, int delta)
    {
        AIDifficulties[characterIndex] += delta;
        if (AIDifficulties[characterIndex] > 2)
        {
            AIDifficulties[characterIndex] = 0;
        }

        if (AIDifficulties[characterIndex] < 0)
        {
            AIDifficulties[characterIndex] = 2;
        }
    }

    //get field methods
	public int getType(int characterIndex)
    {
        return types[characterIndex];
    }
    public int getCharacter(int characterIndex)
    {
        return characters[characterIndex];
    }
    public int getAIDifficulty(int characterIndex)
    {
        return AIDifficulties[characterIndex];
    }

    //ensure at least 1 player, 1 other
    public bool canProceed()
    {
        bool playerExists = false;
        int numberOfCharacters = 0;

        for (int i = 0; i < types.Length; i++)
        {
            if(types[i] != 2)
            {
                numberOfCharacters++;
                if(types[i] == 0)
                {
                    playerExists = true;
                }
            }
        }
        return playerExists && (numberOfCharacters > 1);
    }
    
    public int[] getTypesArray()
    {
        return types;
    }
    public int[] getCharactersArray()
    {
        return characters;
    }
    public int[] getAIDifficultiesArray()
    {
        return AIDifficulties;
    }
}
