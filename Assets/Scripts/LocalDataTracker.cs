using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalDataTracker : MonoBehaviour {

    private static ChangePlayerNumber cpn;
    private static int numberOfPlayers;
    private static int[] placing;
    private static int counter;
    private static int sum = 0;

    private void Start()
    {
        cpn = new ChangePlayerNumber();
    }

    public void startGame()
    {
        numberOfPlayers = cpn.getNumberOfAI() + cpn.getNumberOfPlayers();
        placing = new int[numberOfPlayers];
        counter = placing.Length - 1;
    }

    public static void playerDeath(int playerIndex)
    {
        Debug.Log("Place " + (counter + 1) + ": Player " + playerIndex);

        placing[counter] = playerIndex;

        sum += playerIndex;
        counter--;

        
        // insert code for updating other player stats

        if(counter == 0)
        {
            placing[0] = numberOfPlayers * (numberOfPlayers - 1) / 2 - sum; // efficient method for finding winner index

            displayResults();
        }
    }

    private static void displayResults()
    {
        CanvasGroup endScreen = GameObject.Find("End Screen").GetComponent<CanvasGroup>();

        endScreen.alpha = 1;
        endScreen.interactable = true;
    }
}
