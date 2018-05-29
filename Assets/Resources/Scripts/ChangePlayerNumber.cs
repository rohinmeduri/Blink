using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangePlayerNumber : MonoBehaviour {
    private static int numberOfPlayers = 1;     //keep track of number of players
    private static int numberOfAI = 1;          //keep track of number of AI
    private static int AI1Difficulty = 1;
    private static int AI2Difficulty = 1;
    private static int AI3Difficulty = 1;
    public Text players;
    public Text AI;
    public Text AIDiff1;
    public Text AIDiff2;
    public Text AIDiff3;
    private static string[] Difficulties = { "Easy", "Medium", "Hard" };


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
        updateNumberElements();
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
        updateNumberElements();
    }
    public void changeAI1Difficulty(int n)
    {
        AI1Difficulty += n;
        if (AI1Difficulty < 0)          //
        {                           //
            AI1Difficulty = 0;         //
        }
        if (AI1Difficulty > 2)        //
        {                         //
            AI1Difficulty = 2;       //
        }
        AIDiff1.text = Difficulties[AI1Difficulty];
    }
    public void changeAI2Difficulty(int n)
    {
        AI2Difficulty += n;
        if (AI2Difficulty < 0)          //
        {                           //
            AI2Difficulty = 0;         //
        }
        if (AI2Difficulty > 2)        //
        {                         //
            AI2Difficulty = 2;       //
        }
        AIDiff2.text = Difficulties[AI2Difficulty];
    }
    public void changeAI3Difficulty(int n)
    {
        AI3Difficulty += n;
        if (AI3Difficulty < 0)          //
        {                           //
            AI3Difficulty = 0;         //
        }
        if (AI3Difficulty > 2)        //
        {                         //
            AI3Difficulty = 2;       //
        }
        AIDiff3.text = Difficulties[AI3Difficulty];
    }
    public int getNumberOfPlayers()
    {
        return numberOfPlayers;
    }
    public int getNumberOfAI()
    {
        return numberOfAI;
    }
	public void updateNumberElements()
    {
        players.text = numberOfPlayers.ToString();
        AI.text = numberOfAI.ToString();
    }
    public int[] getAIDifficulties()
    {
        int[] Fresh = {AI1Difficulty, AI2Difficulty, AI3Difficulty};
        int[] Dank = new int[numberOfAI];
        for(int i = 0; i < numberOfAI; i++)
        {
            Dank[i] = Fresh[i];
        }
        return Dank;
    }
}
