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
    public const float MAX_SPEED = 15; // maximum horizontal speed
    public const float JUMP_SPEED = 15; // jump height
    public const int JUMP_NUM = 1; // number of jumps without touching ground
    public const float WALLJUMP_SPEED = 15; // horizontal speed gained from wall-jumps
    public const float FALL_SPEED = -20; // maximum fall speed
    public const float FALL_FORCE = 1; // force of gravity
    public const float FALL_COEF = 2; // How much player can control fall speed. Smaller = more control (preferrably > 1 [see for yourself ;)])

    public Rigidbody2D rb2D;
    public Collider2D c2D;
    
    private bool touchingGround = false;
    private int touchingWall = 0; // -1: right wall. 1: left wall. 0: no wall.

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

        Debug.Log(touchingGround + ", " + touchingWall);
    }

    /**
     * Script for Running
     */
    void run()
    {
        facingRight = Input.GetAxis("Horizontal") > 0 || (Input.GetAxis("Horizontal") == 0 && facingRight);
        gameObject.GetComponent<SpriteRenderer>().flipX = !facingRight;

        // changes velocity gradually to a goal velocity determined by controls
        float goalSpeed = MAX_SPEED * Input.GetAxis("Horizontal");
        float runForce;

        if (touchingGround) runForce = GROUND_RUN_FORCE;
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
        if (Input.GetAxis("Jump") > 0 && canJump)
        {
            if (touchingWall != 0 && !touchingGround) // walljump
            {
                rb2D.velocity = new Vector2(touchingWall * WALLJUMP_SPEED, JUMP_SPEED);
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
     * Collision Detector
     * Need to find a reliable way to figure out if touching ground and if touching wall
     * Possibly use coll.GetContacts()?
     */ 
    private void OnCollisionEnter2D(Collision2D coll)
    {
        GameObject collGO = coll.gameObject;

        // check if touching a wall
        if (Mathf.Abs(c2D.gameObject.transform.position.y - collGO.gameObject.transform.position.y) < (c2D.gameObject.GetComponent<Collider2D>().bounds.size.y + collGO.gameObject.GetComponent<Collider2D>().bounds.size.y) / 2 - 0.1F)
        {
            touchingWall = (int)Mathf.Sign(c2D.gameObject.transform.position.x - collGO.gameObject.transform.position.x);
        }
        else
        {
            // check if touching ground from above
            if (c2D.gameObject.transform.position.y > collGO.gameObject.transform.position.y)
            {
                touchingGround = true;

                // resets jumps
                jumps = JUMP_NUM;
            }
        }
        
    }

    private void OnCollisionExit2D(Collision2D coll)
    {
        GameObject collGO = coll.gameObject;

        // check if leaving wall
        if(Mathf.Abs(c2D.gameObject.transform.position.y - collGO.gameObject.transform.position.y) < (c2D.gameObject.GetComponent<Collider2D>().bounds.size.y + collGO.gameObject.GetComponent<Collider2D>().bounds.size.y) / 2 - 0.1F)
        {
            touchingWall = 0;
        }
        else // leaving ground
        {
            touchingGround = false;
        }
    }
}
