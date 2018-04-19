using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangePlayerNumber : MonoBehaviour {
    private static int numberOfPlayers = 1;     //keep track of number of players
    private static int numberOfAI = 1;          //keep track of number of AI
    public static int playerLimit = 4;              //set limit in unity editor on players
    public static int AILimit = 4;
    public Text players;
    public Text AI;
    //set limit in unity editor on AI
    void start()
    {
        players = gameObject.GetComponent<Text>();
        AI = gameObject.GetComponent<Text>();
    }
    public void changeNumberOfPlayersBy(int n)      //used by playerCountGroup buttons
    {
        numberOfPlayers += n;
        if(numberOfPlayers < 0)         //
        {                               //keep Players from going below 0
            numberOfPlayers = 0;        //
        }
        if(numberOfPlayers > playerLimit)       //
        {                                       //set a limit in unity that number cannot exceed
            numberOfPlayers = playerLimit;      //
        }
        players.text = numberOfPlayers.ToString();
    }
    public void changeNumberOfAIBy(int n)           //used by AICountGroup
    {
        numberOfAI += n;
        if(numberOfAI < 0)          //
        {                           //keep AI from going below 0
            numberOfAI = 0;         //
        }
        if(numberOfAI > AILimit)        //
        {                               //keep AI from exceeding limit set in unity;
            numberOfAI = AILimit;       //
        }
        AI.text = numberOfAI.ToString();
    }
    public int getNumberOfPlayers()
    {
        return numberOfPlayers;
    }
    public int getNumberOfAI()
    {
        return numberOfAI;
    }
	
}
