using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LocalPlayerSpawner : MonoBehaviour{
    private int numPlayers;
    private int numAIs;
    private int[] AIDifficulties;
    public GameObject playerPrefab;
    public GameObject AIPrefab;

	// Use this for initialization
	void Start () {
            
        numPlayers = GetComponent<ChangePlayerNumber>().getNumberOfPlayers();
        numAIs = GetComponent<ChangePlayerNumber>().getNumberOfAI();
        AIDifficulties = GetComponent<ChangePlayerNumber>().getAIDifficulties();
        Debug.Log("numplayers" + numPlayers);
        Debug.Log("numAI" + numAIs);


        for (int i = 1; i <= numPlayers + numAIs; i++)
        {
            Debug.Log("start loop:" + i);
            GameObject player;
            if (i <= numPlayers)
            {
                player = Instantiate(playerPrefab);
                Debug.Log("player made");
            }
            else
            {
                player = Instantiate(AIPrefab);
                Debug.Log("AI attempt");
                player.GetComponent<PlayerAIScript>().setPlayerDifficulty(AIDifficulties[i - 1 - numPlayers]);
                Debug.Log("AI made");
            }
            player.GetComponent<LocalPlayerScript>().setPlayerID(i);
            Debug.Log(i);
        }
	}
}
