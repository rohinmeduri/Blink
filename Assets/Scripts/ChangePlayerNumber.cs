using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangePlayerNumber : MonoBehaviour {
    private static int numberOfPlayers = 1;     //keep track of number of players
    private static int numberOfAI = 1;          //keep track of number of AI
    public Text players;
    public Text AI;

    //set limit in unity editor on AI
    public void changeNumberOfPlayersBy(int n)      //used by playerCountGroup buttons
    {
        numberOfPlayers += n;
        if(numberOfPlayers < 0)         //
        {                               //keep Players from going below 0
            numberOfPlayers = 0;        //
        }
        if (numberOfPlayers > 4)
        {
            numberOfPlayers = 4;
        }
        if (numberOfPlayers + numberOfAI > 4)
        {
            numberOfAI = 4 - numberOfPlayers;
        }
        updateTextElements();
    }
    public void changeNumberOfAIBy(int n)           //used by AICountGroup
    {
        numberOfAI += n;
        if(numberOfAI < 0)          //
        {                           //keep AI from going below 0
            numberOfAI = 0;         //
        }
        if (numberOfAI > 3)        //
        {                         //can't go over 3 AI
            numberOfAI = 3;       //
        }
        if (numberOfPlayers + numberOfAI > 4)
        {
            numberOfPlayers = 4 - numberOfAI;
        }
        updateTextElements();
    }
    public int getNumberOfPlayers()
    {
        return numberOfPlayers;
    }
    public int getNumberOfAI()
    {
        return numberOfAI;
    }
	public void updateTextElements()
    {
        players.text = numberOfPlayers.ToString();
        AI.text = numberOfAI.ToString();
    }
}
