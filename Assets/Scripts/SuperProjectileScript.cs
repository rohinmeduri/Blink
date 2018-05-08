using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperProjectileScript : MonoBehaviour {
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.layer == 2)
        {
            Destroy(gameObject);
        }
        else if(collision.gameObject.tag == "Player" || collision.rigidbody.gameObject.tag == "PlayerAI")
        {
            Debug.Log(sender == null);
            sender.GetComponent<LocalPlayerScript>().killPlayer(collision.gameObject);
        }
    }
}
