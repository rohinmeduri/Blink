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

	// Use this for initialization
	void Start () {

        //0 is player, 1 is AI, 2 is none;
        types = GetComponent<CharacterDataObject>().getTypesArray();
        Debug.Log("types");
        Debug.Log(types[0]);
        Debug.Log(types[1]);
        Debug.Log(types[2]);
        Debug.Log(types[3]);
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
                player.GetComponent<LocalPlayerScript>().setPlayerID(i + 1);
                player.GetComponent<LocalPlayerScript>().setPlayerType(characterStrings[characters[i]]);
            }
        }
        /*for (int i = 1; i <= numPlayers + numAIs; i++)
        {
            GameObject player;
            if (i <= numPlayers)
            {
                player = Instantiate(playerPrefab);
            }
            else
            {
                player = Instantiate(AIPrefab);
                player.GetComponent<PlayerAIScript>().setPlayerDifficulty(AIDifficulties[i - 1 - numPlayers]);
            }
            player.GetComponent<LocalPlayerScript>().setPlayerID(i);
        }*/
	}
}
