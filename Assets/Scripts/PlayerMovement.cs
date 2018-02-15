using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerMovement : NetworkBehaviour {

    public int jumps = 0;
    public bool canJump = false;

    public const int RUN_SPEED = 10;
    public const int JUMP_SPEED = 12;
    public const int JUMP_NUM = 2;

    public Rigidbody2D rb2D;

	// Use this for initialization
	void Start () {
        rb2D = gameObject.GetComponent<Rigidbody2D>();

    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        rb2D.velocity = new Vector2(RUN_SPEED * Input.GetAxis("Horizontal"), rb2D.velocity.y);
        if (Input.GetAxis("Jump") > 0 && jumps > 0 && canJump)
        {
            rb2D.velocity = new Vector2(rb2D.velocity.x, JUMP_SPEED);
            jumps--;
            canJump = false;
        }

        if(Input.GetAxis("Jump") == 0)
        {
            canJump = true;
        }

    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.name == "Ground")
        {
            jumps = JUMP_NUM;
        }
    }
}
