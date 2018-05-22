using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SuperProjectileScript : NetworkBehaviour {
    private GameObject sender;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void setSender(GameObject s)
    {
        sender = s;
    }

    protected virtual void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.gameObject.tag != "Player" && collider.gameObject.tag != "PlayerAI")
        {
            Destroy(gameObject);
        }
        else
        {
            sender.GetComponent<LocalPlayerScript>().killPlayer(collider.gameObject);
        }
    }
}
