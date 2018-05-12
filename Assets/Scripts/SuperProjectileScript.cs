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

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag != "Player" && collision.gameObject.tag != "PlayerAI")
        {
            Destroy(gameObject);
            Debug.Log("destroyed");
        }
        else
        {
            Debug.Log(sender == null);
            sender.GetComponent<LocalPlayerScript>().killPlayer(collision.gameObject);
        }
    }
}
