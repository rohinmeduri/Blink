using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerMovement : NetworkBehaviour {

    // variables
    private int jumps;
    private bool canJump;
    private Vector2 currentNormal;
    private int stickyWallTimer;
    private int stunTimer;

    [SyncVar]
    public bool facingRight = true;

    private Rigidbody2D rb2D;
    //private Collider2D c2D;

    // constants
    public const float GROUND_RUN_FORCE = 2; // How fast player can attain intended velocity on ground
    public const float AIR_RUN_FORCE = 0.5F; // .... in air
    public const float MAX_SPEED = 10; // maximum horizontal speed
    public const float JUMP_SPEED = 10; // jump height
    public const int JUMP_NUM = 0; // number of midair jumps without touching ground
    public const float WALLJUMP_SPEED = 15; // horizontal speed gained from wall-jumps
    public const float FALL_SPEED = -10; // maximum fall speed
    public const float FALL_FORCE = 0.5F; // force of gravity
    public const float FALL_COEF = 2; // How much player can control fall speed. Smaller = more control (preferrably > 1 [see for yourself ;)])
    public const float MAX_WJABLE_ANGLE = -Mathf.PI / 18; // largest negative angle of a wall where counts as walljump
    public const float MIN_JUMP_RECOVERY_ANGLE = Mathf.PI / 18; // smallest angle of a wall where air jumps are recovered
    public const int STICKY_WJ_DURATION = 15; // amount of frames that player sticks to a wall after touching it
    public const int STUN_DURATION = 100; // amount of frames that a player stays stunned


    // Use this for initialization
    void Start()
    {
        jumps = 0;
        canJump = false;
        currentNormal = new Vector2(0, 0);
        stickyWallTimer = 0;
        stunTimer = 0;

        rb2D = gameObject.GetComponent<Rigidbody2D>();
        //c2D = gameObject.GetComponent<Collider2D>();
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

        if (stunTimer == 0)
        {
            run();
            jump();
        }
        else
        {
            DI();
            stunTimer--;
        }
        gravity();
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

    void jumpFlipSprite()
    {
        if (!hasAuthority || rb2D.velocity.x == 0)
        {
            return;
        }
        bool facingRightNow = rb2D.velocity.x > 0;
        if(facingRightNow != facingRight)
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
        float goalSpeed = sticky();
        float runForce;

        // determine whether should use ground acceleration or air acceleration
        if (currentNormal.Equals(new Vector2(0, 0))) runForce = AIR_RUN_FORCE;
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
     * Script for checking sticky walljump
     */
    float sticky()
    {
        float goalSpeed = 0;
        // If standing on flat enough ground or in air, reset sticky timer
        if (currentNormal.Equals(new Vector2(0, 0)) || currentNormal.y >= Mathf.Sin(MIN_JUMP_RECOVERY_ANGLE))
        {
            stickyWallTimer = 0;
        }
        // if touching a platform close enough to a walljumpable wall, start sticking
        else if (currentNormal.y < Mathf.Sin(MIN_JUMP_RECOVERY_ANGLE) && currentNormal.y > Mathf.Sin(MAX_WJABLE_ANGLE) && stickyWallTimer == 0)
        {
            stickyWallTimer = STICKY_WJ_DURATION;
        }

        // if still sticking, run into wall and decrease timer
        if (stickyWallTimer > 0)
        {
            goalSpeed = -MAX_SPEED * currentNormal.x;
            if (Mathf.Sign(currentNormal.x) == Mathf.Sign(Input.GetAxis("Horizontal")))
            {
                stickyWallTimer--;
            }
        }

        // once timer is out, resume normal movement
        if (stickyWallTimer == 0)
        {
            goalSpeed = MAX_SPEED * Input.GetAxis("Horizontal");
        }

        return goalSpeed;
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
            if (jumps > 0 || !(currentNormal.y < Mathf.Sin(MAX_WJABLE_ANGLE) || currentNormal.Equals(new Vector2(0, 0))))
            {
                rb2D.velocity = new Vector2(WALLJUMP_SPEED * currentNormal.x + rb2D.velocity.x, JUMP_SPEED);
            }
            // if jumping in midair or on a wall that's too steep
            if (jumps > 0 && (currentNormal.y < Mathf.Sin(MAX_WJABLE_ANGLE) || currentNormal.Equals(new Vector2(0, 0))))
            {
                jumps--;
            }
            jumpFlipSprite();
            // cannot jump until release jump key
            canJump = false;
        }

        // resets jump
        if (Input.GetAxis("Jump") == 0)
        {
            canJump = true;
        }
    }

    /**
     * Script for applying gravity
     */
    void gravity()
    {

        // set falling terminal velocity
        float fallSpeed = FALL_SPEED;
        if(stunTimer == 0)
        {
            fallSpeed *= (1 - Input.GetAxis("Vertical") / FALL_COEF);
        }

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
     * Script for Directional Influence
     */
    void DI()
    {

    }


    /**
     * Enter hit stun mode
     */
    public void hitStun()
    {
        //Debug.Log("hitstun");
        if (!hasAuthority)
        {
            return;
        }

        stunTimer = STUN_DURATION;
    }
    
    /**
     * Collision Detector
     */ 
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            Physics2D.IgnoreCollision(collision.collider, gameObject.GetComponent<Collider2D>());
        }

    }

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
        if (currentNormal.y > Mathf.Sin(MIN_JUMP_RECOVERY_ANGLE)) jumps = JUMP_NUM;
        
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
