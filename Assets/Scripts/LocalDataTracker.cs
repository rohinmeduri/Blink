﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocalDataTracker : MonoBehaviour {

    public Text place1;
    public Text place2;
    public Text place3;
    public Text place4;

    //private ChangePlayerNumber cpn;
    private static int numberOfPlayers;
    private static int[][] placing;
    private static int numberAlive;
    private static int sum = 0;

    private void Start()
    {
        //cpn = new ChangePlayerNumber();
        //numberOfPlayers = cpn.getNumberOfAI() + cpn.getNumberOfPlayers();
        Invoke("findPlayers", 2);
    }

    private void findPlayers()
    {
        numberOfPlayers = GameObject.FindGameObjectsWithTag("Player").Length + GameObject.FindGameObjectsWithTag("PlayerAI").Length;
        placing = new int[numberOfPlayers][];
        for (int i = 0; i < numberOfPlayers; i++)
        {
            placing[i] = new int[5];
        }
        numberAlive = placing.Length;
    }


    public void playerDeath(GameObject lostPlayer, GameObject wonPlayer)
    {
        LocalPlayerScript lostPlayerScript = lostPlayer.GetComponent<LocalPlayerScript>();
        LocalPlayerScript wonPlayerScript = wonPlayer.GetComponent<LocalPlayerScript>();

        placing[numberAlive - 1] = compileData(lostPlayerScript);
        placing[0] = compileData(wonPlayerScript);

        int playerIndex = lostPlayerScript.getPlayerID();
        sum += playerIndex;
        
        numberAlive--;

        
        // insert code for updating other player stats

        if(numberAlive == 1)
        {
            Debug.Log(numberAlive);
            displayResults();
        }
    }

    private int[] compileData(LocalPlayerScript lps)
    {
        int[] output = lps.compileData();

       return output;
    }

    private void displayResults()
    {
        CanvasGroup endScreen = GameObject.Find("End Screen").GetComponent<CanvasGroup>();

        endScreen.alpha = 1;
        endScreen.interactable = true;
        endScreen.GetComponent<RectTransform>().SetAsLastSibling();

        place1.text = "1st\n" + formatStats(placing[0]);
        place2.text = "2nd\n" + formatStats(placing[1]);
        place3.text = "3rd\n" + formatStats(placing[2]);
        place4.text = "4th\n" + formatStats(placing[3]);

    }

    private string formatStats(int[] arr)
    {
        string output = "";
        for(int i = 0; i < arr.Length; i++)
        {
            output += arr[i] + "\n";
        }
        return output;
    }

    public void  replaceStats(int mc, int hn, int hp, int k)
    {
        Debug.Log("replacing stats pt 2");
        placing[numberAlive][1] = mc;
        placing[numberAlive][2] = hn;
        placing[numberAlive][3] = hp;
        placing[numberAlive][4] = k;
    }
}
