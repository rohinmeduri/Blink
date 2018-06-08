using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LocalPlayerSpawner : MonoBehaviour{
    private int numPlayers;
    private int numAIs;
    private int[] types;
    private int[] characters;
    private int[] AIDifficulties;
    public GameObject playerPrefab;
    public GameObject AIPrefab;

	// Use this for initialization
	void Start () {

        //0 is player, 1 is AI, 2 is none;
        types = GetComponent<CharacterDataObject>().getTypeArray();
        numAIs = GetComponent<ChangePlayerNumber>().getNumberOfAI();
        AIDifficulties = GetComponent<ChangePlayerNumber>().getAIDifficulties();

        //0 is easy, 1 is medium, 2 is hard
        //0 is mage, 1 is rebel, 2 is saidon


        for (int i = 1; i <= numPlayers + numAIs; i++)
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
        }
	}
}
