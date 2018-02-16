using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerMovement : NetworkBehaviour {

    public int jumps = 0;
    public bool canJump = false;

    [SyncVar]
    private bool facingRight = true;

    public const float GROUND_RUN_FORCE = 3; // How fast player can attain intended velocity on ground
    public const float AIR_RUN_FORCE = 1; // .... in air
    public const float MAX_SPEED = 15; // maximum horizontal speed
    public const float JUMP_SPEED = 15; // jump height
    public const int JUMP_NUM = 2; // number of jumps without touching ground
    public const float WALLJUMP_SPEED = 15; // horizontal speed gained from wall-jumps
    public const float FALL_SPEED = -20; // maximum fall speed
    public const float FALL_FORCE = 1; // force of gravity
    public const float FALL_COEF = 2; // How much player can control fall speed. Smaller = more control (preferrably > 1 [see for yourself ;)])
    public const float MIN_WJ_RECOVERY_ANGLE = -Mathf.PI / 4; // smallest angle of a wall where jump number can be recovered

    public Rigidbody2D rb2D;
    public Collider2D c2D;
    
    private Vector2 currentNormal;
    

    // Use this for initialization
    void Start () {
        rb2D = gameObject.GetComponent<Rigidbody2D>();
        c2D = gameObject.GetComponent<Collider2D>();
        currentNormal = new Vector2(0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.GetComponent<SpriteRenderer>().flipX = !facingRight;
       
        if (!hasAuthority)
        {
            return;
        }

        run();
        jump();
    }

    [Command]
    void CmdFlipSprite(bool fr)
    {
        facingRight = fr;
    }

    /**
     * Script for Running
     */
    void run()
    {
        bool facingRightNow = Input.GetAxis("Horizontal") > 0 || (Input.GetAxis("Horizontal") == 0 && facingRight);
        if(facingRightNow != facingRight)
        {
            CmdFlipSprite(facingRightNow);
        }
        facingRight = facingRightNow;

        // changes velocity gradually to a goal velocity determined by controls
        float goalSpeed = MAX_SPEED * Input.GetAxis("Horizontal");
        float runForce;

        if (currentNormal.Equals(new Vector2(0,0))) runForce = AIR_RUN_FORCE;
        else runForce = GROUND_RUN_FORCE;
        

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
        if (Input.GetAxis("Jump") > 0 && canJump && jumps > 0)
        {
            rb2D.velocity = new Vector2(WALLJUMP_SPEED * currentNormal.x + rb2D.velocity.x, JUMP_SPEED);
            jumps--;
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
     */ 
    private void OnCollisionEnter2D(Collision2D collision)
    {
        
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        
        ContactPoint2D[] cps = new ContactPoint2D[2];

        collision.GetContacts(cps);

        ContactPoint2D cp = cps[0];
        
        if(cp.normal.y > Mathf.Sin(MIN_WJ_RECOVERY_ANGLE)) jumps = JUMP_NUM;

        if (Mathf.Abs(cp.normal.y) >= Mathf.Abs(currentNormal.y))
        {
            currentNormal = cp.normal;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        Debug.Log(currentNormal.x + ", " + currentNormal.y);
        currentNormal = new Vector2(0, 0);
        jumps--;
    }
}
