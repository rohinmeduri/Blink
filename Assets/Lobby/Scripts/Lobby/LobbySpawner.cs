using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype.NetworkLobby
{
    public class LobbySpawner : MonoBehaviour
    {

        LobbyManager lobbyManager;

        // Use this for initialization
        void Start()
        {
            lobbyManager = new LobbyManager();
            Debug.Log("blah1");
            //lobbyManager.Restart();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
