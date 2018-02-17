using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerConnectionScript : NetworkBehaviour {
    public GameObject PlayerPrefab;

	// Use this for initialization
	void Start () {
        if (!isLocalPlayer)
        {
            return;
        }
        CmdSpawnPlayer();
	}

    [Command]
    void CmdSpawnPlayer()
    {
        GameObject player = Instantiate(PlayerPrefab);
        NetworkServer.SpawnWithClientAuthority(player, connectionToClient);
        RpcSetLayer(player);
    }

    [ClientRpc]
    void RpcSetLayer(GameObject player)
    {
        player.GetComponent<PlayerMovement>().setLayer();
    }
}
