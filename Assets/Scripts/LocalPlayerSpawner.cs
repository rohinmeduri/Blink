using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LocalPlayerSpawner : MonoBehaviour{
    private int numPlayers;
    private int numAIs;
    private int[] types;
    private int[] characters;
    private string[] characterStrings = { "Mage", "Rebel", "Saidon" };
    private int[] AIDifficulties;
    public GameObject playerPrefab;
    public GameObject AIPrefab;
    private bool mageChosen = false;
    private bool rebelChosen = false;
    private bool saidonChosen = false;
    private int numColored = 0;

	// Use this for initialization
	void Start () {

        //0 is player, 1 is AI, 2 is none;
        types = GetComponent<CharacterDataObject>().getTypesArray();

        //0 is easy, 1 is medium, 2 is hard
        AIDifficulties = GetComponent<CharacterDataObject>().getAIDifficultiesArray();

        //0 is mage, 1 is rebel, 2 is saidon
        characters = GetComponent<CharacterDataObject>().getCharactersArray();
        
        for(int i = 0; i < 4; i++)
        {
            if (types[i] != 2)
            {
                GameObject player;
                if (types[i] == 0)
                {
                    player = Instantiate(playerPrefab);
                }
                else
                {
                    player = Instantiate(AIPrefab);
                    player.GetComponent<PlayerAIScript>().setPlayerDifficulty(AIDifficulties[i]);
                }

                bool repeat = false;
                if(characters[i] == 0)
                {
                    if (mageChosen)
                    {
                        repeat = true;
                    }
                    mageChosen = true;
                }
                else if(characters[i] == 1)
                {
                    if (rebelChosen)
                    {
                        repeat = true;
                    }
                    rebelChosen = true;
                }
                else
                {
                    if (saidonChosen)
                    {
                        repeat = true;
                    }
                    saidonChosen = true;
                }

                if (repeat)
                {
                    numColored++;
                    player.GetComponent<LocalPlayerScript>().setColor(numColored);
                }
                else
                {
                    player.GetComponent<LocalPlayerScript>().setColor(0);
                }

                player.GetComponent<LocalPlayerScript>().setPlayerType(characterStrings[characters[i]]);
                player.GetComponent<LocalPlayerScript>().setPlayerID(i + 1);
            }
        }
	}
}
