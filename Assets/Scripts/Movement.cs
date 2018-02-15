using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Movement : NetworkBehaviour {

    public bool jumpable = true;
    private bool facingRight = true;

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

        facingRight = (Input.GetAxis("Horizontal") > 0 || (Input.GetAxis("Horizontal") == 0 && facingRight));

        gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(10 * Input.GetAxis("Horizontal"), gameObject.GetComponent<Rigidbody2D>().velocity.y);
        if (Input.GetAxis("Jump") > 0 && jumpable)
        {
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(gameObject.GetComponent<Rigidbody2D>().velocity.x, 10);
            jumpable = false;
        }


        gameObject.GetComponent<SpriteRenderer>().flipX = !facingRight;

    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.name == "Ground")
        {
            jumpable = true;
        }
    }
}
