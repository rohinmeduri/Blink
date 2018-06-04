using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Prototype.NetworkLobby;

public class NetworkLobbyHook : LobbyHook {
    public override void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, GameObject lobbyPlayer, GameObject gamePlayer)
    {
        Debug.Log("player number: " + lobbyPlayer.GetComponent<LobbyPlayer>().connectionNumber);
        gamePlayer.GetComponent<NetworkedPlayerScript>().setPlayerNumber(lobbyPlayer.GetComponent<LobbyPlayer>().connectionNumber);
    }
}
