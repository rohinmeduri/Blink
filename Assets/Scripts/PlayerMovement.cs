using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerMovement : NetworkBehaviour {

    public int jumps = 0;
    public bool canJump = false;
    public int onWall = 0; // 1: left wall. -1: right wall

    public const int GROUND_RUN_SPEED = 10;
    public const int AIR_RUN_FORCE = 100;
    public const int AIR_MAX_SPEED = 10;
    public const int JUMP_SPEED = 12;
    public const int JUMP_NUM = 2;
    public const int WALLJUMP_SPEED = 10;

    public Rigidbody2D rb2D;
    public Collider2D c2D;

    private Collider2D leftWall;
    private Collider2D rightWall;
    private Collider2D ground;

    // Use this for initialization
    void Start () {
        rb2D = gameObject.GetComponent<Rigidbody2D>();
        c2D = gameObject.GetComponent<Collider2D>();
        leftWall = GameObject.Find("Left Wall").GetComponent<Collider2D>();
        rightWall = GameObject.Find("Right Wall").GetComponent<Collider2D>();
        ground = GameObject.Find("Ground").GetComponent<Collider2D>();
}

    // Update is called once per frame
    void Update()
    {

        if (!isLocalPlayer)
        {
            return;
        }

        run();
        jump();

    }

    void run()
    {
        // replace with same method for ground vs air: forces with capped velocity

        if (c2D.IsTouching(ground))
        {
            rb2D.velocity = new Vector2(GROUND_RUN_SPEED * Input.GetAxis("Horizontal"), rb2D.velocity.y);
        }
        else
        {
            rb2D.AddForce(new Vector2((rb2D.velocity.x < AIR_MAX_SPEED) ? AIR_RUN_FORCE * Input.GetAxis("Horizontal") : 0, 0));
        }
    }

    void jump()
    {

        int whichWall = touchingCollider();

        if (Input.GetAxis("Jump") > 0 && jumps > 0 && canJump)
        {
            if (whichWall == 0)
            {
                rb2D.velocity = new Vector2(rb2D.velocity.x, JUMP_SPEED);
                jumps--;
            }
            else
            {
                rb2D.velocity = new Vector2(whichWall * WALLJUMP_SPEED, JUMP_SPEED);
            }

            canJump = false;
        }

        if (Input.GetAxis("Jump") == 0)
        {
            canJump = true;
        }
    }

    int touchingCollider()
    {
        if (c2D.IsTouching(leftWall))
        {
            return 1;
        }
        else if (c2D.IsTouching(rightWall))
        {
            return -1;
        }
        return 0;
    }




    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.name == "Ground") // possibly can glitch in the future if player lands perfectly on ground without colliding with it
        {
            jumps = JUMP_NUM;
        }
    }
}
