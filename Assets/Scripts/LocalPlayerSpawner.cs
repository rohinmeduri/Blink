using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalPlayerSpawner : MonoBehaviour {
    private int numPlayers;
    private int numAIs;
    public GameObject playerPrefab;
    public GameObject AIPrefab;

	// Use this for initialization
	void Start () {
        numPlayers = GetComponent<ChangePlayerNumber>().getNumberOfPlayers();
        numAIs = GetComponent<ChangePlayerNumber>().getNumberOfAI();

        for(int i = 1; i <= numPlayers + numAIs; i++)
        {
            GameObject player;
            if (i <= numPlayers)
            {
                player = Instantiate(playerPrefab);
            }
            else
            {
                player = Instantiate(AIPrefab);
                player.GetComponent<PlayerAIScript>().setPlayerDifficulty(2);
            }
            player.GetComponent<LocalPlayerScript>().setPlayerID(i);
        }
	}
}
