using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerMovement : NetworkBehaviour {

    public int jumps = 0;
    public bool canJump = false;
    private bool facingRight = true;

    public const float GROUND_RUN_FORCE = 3; // How fast player can attain intended velocity on ground
    public const float AIR_RUN_FORCE = 1; // .... in air
    public const float MAX_SPEED = 10; // maximum horizontal speed
    public const float JUMP_SPEED = 15; // jump height
    public const int JUMP_NUM = 2; // number of jumps without touching ground
    public const float WALLJUMP_SPEED = 15; // horizontal speed gained from wall-jumps
    public const float FALL_SPEED = -10; // maximum fall speed
    public const float FALL_FORCE = 1; // force of gravity
    public const float FALL_COEF = 2; // How much player can control fall speed. Smaller = more control (preferrably > 1 [see for yourself ;)])

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

    /**
     * Script for Running
     */
    void run()
    {
        facingRight = Input.GetAxis("Horizontal") > 0 || (Input.GetAxis("Horizontal") == 0 && facingRight);
        // changes velocity gradually to a goal velocity determined by controls
        float goalSpeed = MAX_SPEED * Input.GetAxis("Horizontal");
        float runForce;

        if (c2D.IsTouching(ground)) runForce = GROUND_RUN_FORCE;
        else runForce = AIR_RUN_FORCE;

        if (Mathf.Abs(goalSpeed - rb2D.velocity.x) < runForce)
        {
            rb2D.velocity = new Vector2(goalSpeed, rb2D.velocity.y);
        }
        else
        {
            rb2D.velocity = new Vector2(rb2D.velocity.x + runForce * Mathf.Sign(goalSpeed - rb2D.velocity.x), rb2D.velocity.y);
        }
        gameObject.GetComponent<SpriteRenderer>().flipX = !facingRight;
    }

    /**
     * Script for Jumping
     */
    void jump()
    {
        // checks if touching walls
        int whichWall = touchingCollider();

        if (Input.GetAxis("Jump") > 0 && canJump)
        {
            if (whichWall != 0) // walljump
            {
                rb2D.velocity = new Vector2(whichWall * WALLJUMP_SPEED, JUMP_SPEED);
            }
            else if (jumps > 0) // normal jump
            {
                rb2D.velocity = new Vector2(rb2D.velocity.x, JUMP_SPEED);
                jumps--;
            }

            canJump = false;
        }

        // resets jump
        if (Input.GetAxis("Jump") == 0)
        {
            canJump = true;
        }

        float fallSpeed = FALL_SPEED * (1 - Input.GetAxis("Vertical") / FALL_COEF);

        // simulate gravity
        if (Mathf.Abs(fallSpeed - rb2D.velocity.y) < FALL_FORCE)
        {
            rb2D.velocity = new Vector2(rb2D.velocity.x, fallSpeed);
        }
        else
        {
            rb2D.velocity = new Vector2(rb2D.velocity.x, rb2D.velocity.y + FALL_FORCE * Mathf.Sign(fallSpeed - rb2D.velocity.y));
        }
    }

     /**
      * Checks for touching walls
      * Will need to edit later to accomodate walljumping on platforms
      */
    int touchingCollider() // 1: left wall. -1: right wall. 0: not touching walls
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
    
    /**
     * Collision Detector
     */ 
    void OnCollisionEnter2D(Collision2D col)
    {   
        // Resets jump counter when touching ground
        if (col.gameObject.name == "Ground") // possibly can glitch in the future if player lands perfectly on ground without colliding with it
        {
            jumps = JUMP_NUM;
        }
    }
}
