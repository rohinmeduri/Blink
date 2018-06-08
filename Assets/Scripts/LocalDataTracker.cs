using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

//script that compiles and displays stats of players in the battle scene
public class LocalDataTracker : NetworkBehaviour {

    public Text place1;
    public Text place2;
    public Text place3;
    public Text place4;

    public Sprite Mage;
    public Sprite Rebel;
    public Sprite Saidon;

    private string winner;

    CanvasGroup endScreen;

    private static int numberOfPlayers;
    private static int[][] placing;
    private static int numberAlive;
    private static int sum = 0;

    protected virtual void Start()
    {
        endScreen = GameObject.Find("End Screen").GetComponent<CanvasGroup>();
        setInteractable(endScreen, false);
        Invoke("findPlayers", 2);
    }

    //function that makes the end screen clickable
    private void setInteractable(CanvasGroup go, bool interactable)
    {
        go.interactable = interactable;
        foreach (Button child in go.GetComponentsInChildren<Button>())
        {
            child.interactable = interactable;
        }
    }

    //function that finds all the players that are in the game so they can be kept track of
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

    //function that updates stats once a player dies
    public virtual void playerDeath(GameObject lostPlayer, GameObject wonPlayer)
    {
        winner = wonPlayer.GetComponent<LocalPlayerScript>().getPlayerType();

        LocalPlayerScript lostPlayerScript = lostPlayer.GetComponent<LocalPlayerScript>();
        LocalPlayerScript wonPlayerScript = wonPlayer.GetComponent<LocalPlayerScript>();

        placing[numberAlive - 1] = compileData(lostPlayerScript);
        placing[0] = compileData(wonPlayerScript);

        int playerIndex = lostPlayerScript.getPlayerID();
        sum += playerIndex;

        numberAlive--;
        if(numberAlive == 1)
        {

            displayResults();
            GameObject.Find("MusicPlayer").GetComponent<MusicPlayerScript>().queueVictory();
        }
    }

    //function used for networking that compiles stats and destroys dead player objects
    public int reducePlayers(GameObject player)
    {
        numberAlive--;
        if(numberAlive > 0)
        {
            player.GetComponent<LocalPlayerScript>().removeMeter();
            player.GetComponent<LocalPlayerScript>().cancelSuperAnimation();
            Destroy(player);

            if (numberAlive == 1)
            {

                NetworkedPlayerScript winningPlayer = GameObject.FindGameObjectWithTag("Player").GetComponent<NetworkedPlayerScript>();

                winner = winningPlayer.getPlayerType();
                Debug.Log(winner);

                if (winningPlayer.getHasAuthority())
                {
                    winningPlayer.compileData();
                }
            }
        }

        else
        {
            displayResults();
        }
        return numberOfPlayers;
    }

    private int[] compileData(LocalPlayerScript lps)
    {
        int[] output = lps.compileData();

       return output;
    }

    //function that finds who the winner is and shows the stats in the end screen UI object
    private void displayResults()
    {
        endScreen.alpha = 1;
        setInteractable(endScreen, true);
        endScreen.GetComponent<RectTransform>().SetAsLastSibling();

        GameObject.Find("Canvas").transform.Find("End Screen").transform.Find("Cursor").GetComponent<CursorScript>().setVisible(true);

        if (winner.Equals("Mage"))
        {
            endScreen.transform.Find("WinnerName").GetComponent<Image>().sprite = Mage;
        }
        else if (winner.Equals("Rebel"))
        {
            endScreen.transform.Find("WinnerName").GetComponent<Image>().sprite = Rebel;

        }
        else if (winner.Equals("Saidon"))
        {
            endScreen.transform.Find("WinnerName").GetComponent<Image>().sprite = Saidon;

        }

        place1.text = "1st\n" + formatStats(placing[0]);
        place2.text = "2nd\n" + formatStats(placing[1]);
        if (numberOfPlayers > 2)
        {
            place3.text = "3rd\n" + formatStats(placing[2]);
        }
        if (numberOfPlayers > 3)
        {
            place4.text = "4th\n" + formatStats(placing[3]);
        }

    }

    //function that takes the arrays used to store stats and converts them into stirngs
    private string formatStats(int[] arr)
    {
        string output = "";
        for(int i = 0; i < arr.Length; i++)
        {
            output += arr[i] + "\n";
        }
        return output;
    }

    //function for networking that syncs a players stats across the network
    public void  replaceStats(int mc, int hn, int hp, int k, GameObject player)
    {
        Debug.Log("replacing stats pt 2");
        placing[numberAlive - 1][0] = player.GetComponent<LocalPlayerScript>().getPlayerID();
        placing[numberAlive - 1][1] = mc;
        placing[numberAlive - 1][2] = hn;
        placing[numberAlive - 1][3] = hp;
        placing[numberAlive - 1][4] = k;

        if(numberAlive == 1)
        {
            displayResults();
        }
        else
        {
            reducePlayers(player);
        }
    }
}
