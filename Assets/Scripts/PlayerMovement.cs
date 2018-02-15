using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerMovement : NetworkBehaviour {

    public int jumps = 0;
    public bool canJump = false;
    public int onWall = 0; // 1: left wall. -1: right wall

    public const int GROUND_RUN_FORCE = 3;
    public const int AIR_RUN_FORCE = 1;
    public const int MAX_SPEED = 10;
    public const int JUMP_SPEED = 15;
    public const int JUMP_NUM = 2;
    public const int WALLJUMP_SPEED = 15;
    public const float DRAG_COEF = 0.8F;

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
        // changes velocity gradually to a goal velocity determined by controls
        float goalSpeed = MAX_SPEED * Input.GetAxis("Horizontal");
        int runForce;

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
    }
    
    /**
     * Checks for touching walls
     * Will need to edit later to accomodate walljumping on platforms
     */ 
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
