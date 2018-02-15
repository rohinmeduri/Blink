using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Movement : NetworkBehaviour {

    public bool jumpable = true;

	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(10 * Input.GetAxis("Horizontal"), gameObject.GetComponent<Rigidbody2D>().velocity.y);
        if (Input.GetAxis("Jump") > 0 && jumpable)
        {
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(gameObject.GetComponent<Rigidbody2D>().velocity.x, 10);
            jumpable = false;
        }

    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.name == "Ground")
        {
            jumpable = true;
        }
    }
}
