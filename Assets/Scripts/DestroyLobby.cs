using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyLobby : MonoBehaviour {

    /**
     * Used to destroy the lobby manager when going back to main lobby (because it is dontDestroyOnLoad
     */

	// Use this for initialization
	void Start () {
        Destroy(GameObject.Find("LobbyManager"));
    }
}
