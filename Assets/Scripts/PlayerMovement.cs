using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerMovement : NetworkBehaviour {

    private int jumps = 0;
    private bool canJump = false;

    [SyncVar]
    public bool facingRight = true;

    public const float GROUND_RUN_FORCE = 5; // How fast player can attain intended velocity on ground
    public const float AIR_RUN_FORCE = 1; // .... in air
    public const float MAX_SPEED = 10; // maximum horizontal speed
    public const float JUMP_SPEED = 15; // jump height
    public const int JUMP_NUM = 1; // number of midair jumps without touching ground
    public const float WALLJUMP_SPEED = 15; // horizontal speed gained from wall-jumps
    public const float FALL_SPEED = -20; // maximum fall speed
    public const float FALL_FORCE = 1; // force of gravity
    public const float FALL_COEF = 2; // How much player can control fall speed. Smaller = more control (preferrably > 1 [see for yourself ;)])
    public const float MAX_WJABLE_ANGLE = -Mathf.PI / 18; // largest negative angle of a wall where counts as walljump
    public const float MIN_WJ_RECOVERY_ANGLE = Mathf.PI / 18; // smallest angle of a wall where air jumps are recovered

    private Rigidbody2D rb2D;
    //private Collider2D c2D;
    
    private Vector2 currentNormal;
    

    // Use this for initialization
    void Start () {
        rb2D = gameObject.GetComponent<Rigidbody2D>();
        //c2D = gameObject.GetComponent<Collider2D>();
        currentNormal = new Vector2(0, 0);
    }

    public void setLayer()
    {
        if (hasAuthority)
        {
            gameObject.layer = 2;
        }
    }

    // Update is called once per frame
    void Update()
    {
        flipSprite();
        if (!hasAuthority)
        {
            return;
        }
    }

    void FixedUpdate()
    {

        if (!hasAuthority)
        {
            return;
        }

        run();
        jump();
    }


    /**
     * Scripts for flipping the sprite
     */
    [Command]
    void CmdFlipSprite(bool fr)
    {
        facingRight = fr;
    }

    void flipSprite()
    {
        gameObject.GetComponent<SpriteRenderer>().flipX = !facingRight;
        if (!hasAuthority)
        {
            return;
        }
        bool facingRightNow = Input.GetAxis("Horizontal") > 0 || (Input.GetAxis("Horizontal") == 0 && facingRight);
        if (facingRightNow != facingRight)
        {
            CmdFlipSprite(facingRightNow);
        }
        facingRight = facingRightNow;
    }

    /**
     * Script for Running
     */
    void run()
    {
        

        // changes velocity gradually to a goal velocity determined by controls
        float goalSpeed = MAX_SPEED * Input.GetAxis("Horizontal");
        float runForce;

        // determine whether should use ground acceleration or air acceleration
        if (currentNormal.Equals(new Vector2(0,0))) runForce = AIR_RUN_FORCE;
        else runForce = GROUND_RUN_FORCE;
        
        // move to goal speed if possible; otherwise, approach it by "runForce"
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
            // if have midair jumps or attempted jump isn't midair or on a wall that's too steep
            if(jumps > 0 || !(currentNormal.y < Mathf.Sin(MAX_WJABLE_ANGLE) || currentNormal.Equals(new Vector2(0, 0))))
            {
                rb2D.velocity = new Vector2(WALLJUMP_SPEED * currentNormal.x + rb2D.velocity.x, JUMP_SPEED);
            }
            // if jumping in midair or on a wall that's too steep
            if (jumps > 0 && (currentNormal.y < Mathf.Sin(MAX_WJABLE_ANGLE) || currentNormal.Equals(new Vector2(0, 0))))
            {
                jumps--;
            }

            // cannot jump until release jump key
            canJump = false;
        }
        
        // resets jump
        if (Input.GetAxis("Jump") == 0)
        {
            canJump = true;
        }

        // set falling terminal velocity
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

    public void hitStun()
    {
        Debug.Log("hitstun");
        if (!hasAuthority)
        {
            return;
        }
    }
    
    /**
     * Collision Detector
     */ 
   /*private void OnCollisionEnter2D(Collision2D collision)
    {
        
    }*/

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!hasAuthority)
        {
            return;
        }
        // get points of contact with platforms
        ContactPoint2D[] cps = new ContactPoint2D[2];
        collision.GetContacts(cps);
        ContactPoint2D cp = cps[0];

        // set currentNormal to be the normal of the flattest ground currently touching
        if (Mathf.Abs(cp.normal.y) >= Mathf.Abs(currentNormal.y))
        {
            currentNormal = cp.normal;
        }

        // resets jump if the flattest ground is flat enough
        if (currentNormal.y > Mathf.Sin(MIN_WJ_RECOVERY_ANGLE)) jumps = JUMP_NUM;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!hasAuthority)
        {
            return;
        }
        // set currentNormal to zero vector when leave a ground
        currentNormal = new Vector2(0, 0);
    }
}
