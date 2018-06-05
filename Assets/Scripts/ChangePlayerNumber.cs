using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
    public GameObject AIGroup1;
    public GameObject AIGroup2;
    public GameObject AIGroup3;
    public GameObject DecreaseAI;
    public GameObject IncreaseAI;
    public GameObject DecreasePlayers;
    public GameObject IncreasePlayers;
    private static string[] Difficulties = { "Easy", "Medium", "Hard" };

    private void Start()
    {
        if (SceneManager.GetActiveScene().name.Equals("Local Lobby Menu"))
        {
            UpdateAIGroups();
            updateCountGroups();
            updateNumberElements();
        }
            
    }

    //set limit in unity editor on AI
    /*
    public void changeNumberOfPlayersBy(int n)      //used by playerCountGroup buttons
    {
        numberOfPlayers += n;
        if (numberOfPlayers + numberOfAI > 4)
        {
            numberOfAI = 4 - numberOfPlayers;
        }
        IncreasePlayers.SetActive(true);
        DecreasePlayers.SetActive(true);
        if (numberOfPlayers == 4)
        {
            IncreasePlayers.SetActive(false);
        }
        if (numberOfPlayers == 1)
        {
            DecreasePlayers.SetActive(false);
        }
        updateNumberElements();
    }
    public void changeNumberOfAIBy(int n)           //used by AICountGroup
    {
        numberOfAI += n;
        if (numberOfPlayers + numberOfAI > 4)
        {
            numberOfPlayers = 4 - numberOfAI;
        }
        IncreaseAI.SetActive(true);
        DecreaseAI.SetActive(true);
        if (numberOfAI == 3)
        {
            IncreaseAI.SetActive(false);
        }
        if(numberOfAI == 0)
        {
            DecreaseAI.SetActive(false);
        }
        updateNumberElements();
        UpdateAIGroups();
        
    }
    */
    public void increaseAINumber()
    {
        numberOfAI++;
        if (numberOfPlayers + numberOfAI > 4)
        {
            numberOfPlayers = 4 - numberOfAI;
        }
        updateCountGroups();
    }
    public void decreaseAINumber()
    {
        numberOfAI--;
        if (numberOfPlayers + numberOfAI > 4)
        {
            numberOfPlayers = 4 - numberOfAI;
        }
        updateCountGroups();
    }
    public void increasePlayerNumber()
    {
        numberOfPlayers++;
        if (numberOfPlayers + numberOfAI > 4)
        {
            numberOfAI = 4 - numberOfPlayers;
        }
        updateCountGroups();
    }
    public void decreasePlayerNumber()
    {
        numberOfPlayers--;
        if (numberOfPlayers + numberOfAI > 4)
        {
            numberOfAI = 4 - numberOfPlayers;
        }
        updateCountGroups();
    }
    public void updateCountGroups()
    {
        //disable/enable AI count buttons
        if (numberOfAI < 1)
        {
            DecreaseAI.SetActive(false);
        }
        else
        {
            DecreaseAI.SetActive(true);
        }
        if(numberOfAI > 2)
        {
            IncreaseAI.SetActive(false);
        }
        else
        {
            IncreaseAI.SetActive(true);
        }
        //disable/enable Player count buttons
        if(numberOfPlayers < 2)
        {
            DecreasePlayers.SetActive(false);
        }
        else
        {
            DecreasePlayers.SetActive(true);
        }
        if(numberOfPlayers > 3)
        {
            IncreasePlayers.SetActive(false);
        }
        else
        {
            IncreasePlayers.SetActive(true);
        }
        updateNumberElements();
        UpdateAIGroups();
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
        Debug.Log(numberOfAI);
        return Dank;
    }
    public void UpdateAIGroups()
    {
        GameObject[] Groups = { AIGroup1, AIGroup2, AIGroup3 };
        for(int i = 0; i <= 2; i++)
        {
            Groups[i].SetActive(false);
        }
        for(int i = 0; i < numberOfAI; i++)
        {
            Groups[i].SetActive(true);
        }
    }
}
