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
        for(int i = 1; i <= numPlayers; i++)
        {
            Instantiate(playerPrefab);
        }
        for(int i = 1; i <= numAIs; i++)
        {
            Instantiate(AIPrefab);
        }
	}
}
