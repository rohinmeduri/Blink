using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PlayerScript : NetworkBehaviour {
    
    // public variables
    [SyncVar]
    public bool facingRight = true;
    [SyncVar(hook = "OnChangeGlory")]
    public float numGlory = 0;
    public PhysicsMaterial2D regularMaterial;
    public PhysicsMaterial2D stunMaterial;
    public GameObject player;
    public float attackRadius;
    public float baseAttackForce;
    public LayerMask mask;
    public GameObject gloryPrefab;
    public Slider glorySlider;
    public Text comboText;
    public float baseGloryGain;
    public float gloryLostOnHit;
    public float reversalGloryGain;

    // private variables
    private int jumps;
    private bool canJump;
    private Vector2 currentNormal;
    private int stickyWallTimer;
    private int stunTimer;
    private Rigidbody2D rb2D;
    private Collider2D c2D;
    private bool actionLock = false;
    private int actionWaitFrames;
    private int actionWaitedFrames = 0;
    private bool attackButtonHeld = false;
    private int attackFrozeFrames = 0;
    private float lastGloryIncrease = 0;
    private bool reversalEffective = false;
    private Vector2 reversalDirection;
    private GameObject glory;
    private int gloryWaitFrames = 2;
    private int gloryWaitedFrames = 0;
    [SyncVar]
    private bool hasSuper = false;
    private Animator animator;
    private List<GameObject> touchingObjects = new List<GameObject>();
    private List<Vector2> touchingNormals = new List<Vector2>();

    [SyncVar (hook = "OnChangeComboHits")]
    private int comboHits = 0;
    private int comboHitInterval = 0;

    // constants
    public const float GROUND_RUN_FORCE = 2; // How fast player can attain intended velocity on ground
    public const float AIR_RUN_FORCE = 0.5F; // .... in air
    public const float MAX_SPEED = 10; // maximum horizontal speed
    public const float JUMP_SPEED = 10; // jump height
    public const int JUMP_NUM = 1; // number of midair jumps without touching ground
    public const float WALLJUMP_SPEED = 15; // horizontal speed gained from wall-jumps
    public const float WALL_FALL_SPEED = -5; // maximum fall speed when on wall
    public const float FALL_SPEED = -10; // maximum fall speed
    public const float FALL_FORCE = 0.5F; // force of gravity
    public const float FALL_COEF = 2; // How much player can control fall speed. Smaller = more control (preferrably > 1 [see for yourself ;)])
    public const float MAX_WJABLE_ANGLE = -Mathf.PI / 18; // largest negative angle of a wall where counts as walljump
    public const float MIN_JUMP_RECOVERY_ANGLE = Mathf.PI / 4; // smallest angle of a wall where air jumps are recovered
    public const int STICKY_WJ_DURATION = 15; // amount of frames that player sticks to a wall after touching it
    public const int ATTACK_WAIT_FRAMES = 20; // number of frames a player must wait between attacks
    public const int ATTACK_FREEZE_FRAMES = 12; //number of frames a player freezes while attacking
    public const int COMBO_HIT_TIMER = 100; //number of frames a player must land the next attack within to continue a combo
    public const float TRUE_HIT_MULTIPLIER = 1.5f; //multiplier for glory increase for true hits 
    public const int STUN_DURATION = 50; // amount of frames that a player stays stunned
    public const float GROUND_KNOCKBACK_MODIFICATION = 0f; //amount increase to the y component of knockback velocity if player is on ground
    public const float KNOCKBACK_DAMPENING_COEF = 0.98F; // factor that knockback speed slows every frame
    public const float DI_FORCE = 0.1F; // amount of influence of DI
    public const int REVERSAL_EFFECTIVE_TIME = 80; //number of frames in which a reversal is effective
    public const int REVERSAL_DURATION = 80; //number of frames a reversal lasts (effective time + end lag)
    public const float REVERSAL_SUCCESS_ANGLE = 90; //minimum angle between reversal and attack for reversal to be successful
    public const float SUPER_LOSS_GLORY = 85; //glory at which super is lost if player falls below
    public const float GLORY_ON_SUPER_MISS = 75; //glory player drops to for losing super

    // Use this for initialization
    void Start()
    {
        jumps = 0;
        canJump = false;
        currentNormal = new Vector2(0, 0);
        stickyWallTimer = 0;
        stunTimer = 0;

        rb2D = gameObject.GetComponent<Rigidbody2D>();
        c2D = gameObject.GetComponent<Collider2D>();

        animator = GetComponent<Animator>();
    }

    public void setLayer()
    {
        if (hasAuthority)
        {
            gameObject.layer = 2;
        }
    }

    /**
     * Script for creating Glory meters
     */
    public void createMeter()
    {
        //instantiate UI element and place on canvas
        glory = Instantiate(gloryPrefab);
        var canvas = GameObject.Find("Canvas");
        RectTransform gloryTransform = glory.GetComponent<RectTransform>();
        gloryTransform.SetParent(canvas.transform);

        //position meter appropriately depending on if it corresponds to local player or enemy player
        if (!hasAuthority)
        {
            gloryTransform.anchorMin = new Vector2(1, 1);
            gloryTransform.anchorMax = new Vector2(1, 1);
            gloryTransform.pivot = new Vector2(1, 1);
            gloryTransform.anchoredPosition = new Vector3(-100, 0, 0);
        }
        else
        {
            gloryTransform.anchoredPosition = new Vector3(0, 0, 0);
        }

        //assign slider and comboText to variables so they can be modified easily
        glorySlider = glory.transform.Find("Slider").gameObject.GetComponent<Slider>();
        comboText = glory.transform.Find("Combo Text").gameObject.GetComponent<Text>();
        glorySlider.value = numGlory;
    }

    // Update is called once per frame
    void Update()
    {
        // flips sprite
        flipSprite();
        
        //create glory meter after a couple frames so that client authority can be assigned
        gloryWaitedFrames++;
        if (gloryWaitedFrames == gloryWaitFrames)
        {
            createMeter();
        }

        if (!hasAuthority)
        {
            return;
        }

        // Updates Animator variables
        animator.SetFloat("xDir", Input.GetAxis("Horizontal"));
        animator.SetFloat("yDir", Input.GetAxis("Vertical"));
        animator.SetFloat("yVel", rb2D.velocity.normalized.y);
        animator.SetBool("isMoving", Mathf.Abs(Input.GetAxis("Horizontal")) > 0);
        animator.SetBool("isAirborn", isAirborn());
        animator.SetBool("onWall", isWall());
        int attackNum = 0;
        if (Mathf.Abs(Input.GetAxis("Vertical")) > Mathf.Abs(Input.GetAxis("Horizontal")))
        {
            attackNum = (int)Mathf.Sign(Input.GetAxis("Vertical"));
        }
        animator.SetInteger("attackNum", attackNum);

        // Decreases stun timer
        if (stunTimer > 0) stunTimer--;

        //keep a timer for between hits in a combo
        if (comboHits > 0)
        {
            comboHitInterval++;
            if(comboHitInterval == COMBO_HIT_TIMER)
            {
                comboHits = 0;
                CmdChangeComboHits(comboHits);
                comboHitInterval = 0;
            }
        }

        //check to see if player can perform an action again (blink, reversal, attack, super)
        if (actionLock)
        {
            //check to see if player's reversal is still effective
            if (reversalEffective && actionWaitedFrames >= REVERSAL_EFFECTIVE_TIME)
            {
                reversalEffective = false;
            }

            actionWaitedFrames++;
            if(actionWaitedFrames == actionWaitFrames)
            {
                actionLock = false;
                reversalEffective = false;
                actionWaitedFrames = 0;
            }
        }



        //freeze player if they are mid-attack
        /*if (attacking)
        {
            rb2D.velocity = new Vector2(0, 0);
            attackFrozeFrames++;
            if (attackFrozeFrames >= ATTACK_FREEZE_FRAMES)
            {
                attackFrozeFrames = 0;
                attacking = false;
            }
        }*/
    }
    
    void FixedUpdate()
    {

        if (!hasAuthority)
        {
            return;
        }

        if (stunTimer == 0)
        {
            rb2D.sharedMaterial = regularMaterial;
            run();
            jump();
            gravity();
            attack();
            reversal();
            super();
        }
        else
        {
            rb2D.sharedMaterial = stunMaterial;
            DI();
        }
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

        //flip sprite based on player input if they are not wall hugging
        if (stickyWallTimer == 0){
            bool facingRightNow = Input.GetAxis("Horizontal") > 0 || (Input.GetAxis("Horizontal") == 0 && facingRight);
            if (facingRightNow != facingRight)
            {
                CmdFlipSprite(facingRightNow);
                facingRight = facingRightNow;
            }
        }
    }

    void wallJumpFlipSprite()
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
        if (isAirborn()) runForce = AIR_RUN_FORCE;
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
        if (isAirborn() || isGround())
        {
            stickyWallTimer = 0;
        }
        // if touching a platform close enough to a walljumpable wall, start sticking
        else if (isWall() && stickyWallTimer == 0)
        {
            stickyWallTimer = STICKY_WJ_DURATION;
        }

        // if still sticking, flip sprite appropriately, run into wall and decrease timer
        if (stickyWallTimer > 0)
        {
            //flip sprite away from wall
            bool facingRightNow = currentNormal.x > 0;
            if (facingRightNow != facingRight)
            {
                CmdFlipSprite(facingRightNow);
                facingRight = facingRightNow;
            }

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
            if (jumps > 0 || !(currentNormal.y < Mathf.Sin(MAX_WJABLE_ANGLE) || isAirborn()))
            {
                rb2D.velocity = new Vector2(WALLJUMP_SPEED * currentNormal.x + rb2D.velocity.x, JUMP_SPEED);
            }
            // if jumping in midair or on a wall that's too steep
            if (jumps > 0 && (currentNormal.y < Mathf.Sin(MAX_WJABLE_ANGLE) || isAirborn()))
            {
                jumps--;
            }
            if (isWall()){
                wallJumpFlipSprite();
            }
            
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
        if(stickyWallTimer == 0)
        {
            fallSpeed = FALL_SPEED;
        }
        else
        {
            fallSpeed = WALL_FALL_SPEED;
        }
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
     * Script for attacking
     */
    void attack()
    {
        //check to see if attack button is held down - attack occurs once the button is released
        if (Input.GetAxis("Fire1") != 0)
        {
            attackButtonHeld = true;
        }
        else
        {
            //check that button was held in previous frame (meaning it was released this frame
            //so attack should initiate)
            if (attackButtonHeld && !actionLock)
            {
                //raycast to see if someone is hit with the attack - mask out attacker's layer
                Vector2 direction = getDirection();
                direction.Normalize();
                Vector2 origin = new Vector2(player.GetComponent<Transform>().position.x, player.GetComponent<Transform>().position.y);
                Debug.DrawRay(origin, direction * attackRadius, Color.blue, 1f);
                RaycastHit2D hit = Physics2D.Raycast(origin: origin, direction: direction, distance: attackRadius, layerMask: mask.value);
                
                //if attack is successful:
                if (hit.rigidbody != null)
                {
                    comboHits++;
                    var trueHit = (comboHitInterval <= STUN_DURATION) && (comboHits > 1);
                    CmdAttackGloryUpdate(hit.rigidbody.gameObject, comboHits, trueHit);
                    CmdKnockback(hit.rigidbody.gameObject, player, direction, comboHits);
                    comboHitInterval = 0;
                }

                //trigger animation
                animator.SetTrigger("attacking");

                //cannot attack immediately after launching an attack
                actionLock = true;
                actionWaitFrames = ATTACK_WAIT_FRAMES;
            }

            //keep track that attack button wasn't held during this frame
            attackButtonHeld = false;
        }
    }

	void blink{
		
	}

    /**
     * Script to see if player is reversaling (actual impact is handled in knockback)
     */
    void reversal()
    {
        //check if player is pushing reversal button and can reversal
        if (Input.GetAxis("Fire2") > 0 && !actionLock)
        {
            Debug.Log("reversal");
            reversalDirection = getDirection();
            actionLock = true;
            reversalEffective = true;
            actionWaitFrames = REVERSAL_DURATION;
        }
    }
    
    /**
     * 
     */
    void super()
    {
        //check if can super and is super-ing
        if (hasSuper && !actionLock && Input.GetAxis("Fire3") > 0)
        {
            CmdChangeHasSuper(false);
            CmdSetGlory(75);
            Debug.Log("super-ing");
        }
    }

    /**
     * Script to change hasSuper on server
     */ 
    [Command]
    void CmdChangeHasSuper(bool super)
    {
        hasSuper = super;
    }

    /**
     * Script for determining direction of player actions (attacks, reversals, and blinks)
     */
    private Vector2 getDirection()
    {
        //determine horizontal component of attack's direction
        float horizontalDirection;
        //if attacker is not moving, attack direction is the direction they are facing
        if (Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0)
        {
            if (facingRight)
            {
                horizontalDirection = 1;
            }
            else
            {
                horizontalDirection = -1;
            }
        }
        else
        {
            horizontalDirection = Input.GetAxis("Horizontal");
        }
        Vector2 direction = new Vector2(horizontalDirection, Input.GetAxis("Vertical"));
        return direction;
    }


    /*
     * Script for updating glory after an attack on the server
     */
    [Command]
    void CmdAttackGloryUpdate(GameObject otherPlayer, int hits, bool trueHit)
    {
        //increase attacker glory
        comboHits = hits;
        float trueMultiplier = 1;
        if (trueHit)
        {
            trueMultiplier = TRUE_HIT_MULTIPLIER;
        }
        float gloryIncrease = baseGloryGain * (1.0f + comboHits / 10.0f) * trueMultiplier;
        if (numGlory + gloryIncrease >= 100)
        {
            lastGloryIncrease = (100 - numGlory);
            numGlory = 100;
        }
        else
        {
            numGlory += gloryIncrease;
            lastGloryIncrease = gloryIncrease;
        }

        //decrease hit person glory
        if (otherPlayer.GetComponent<PlayerScript>().numGlory - gloryLostOnHit < 0)
        {
            otherPlayer.GetComponent<PlayerScript>().numGlory = 0;
        }
        else
        {
            otherPlayer.GetComponent<PlayerScript>().numGlory -= gloryLostOnHit;
        }
    }

    /*
     * Script for setting glory to specific value on server
     */
     void CmdSetGlory(float glory)
    {
        numGlory = glory;
    }

    /*
     * Script for updating ComboHits on the Server
     */
    [Command]
    void CmdChangeComboHits(int hits)
    {
        comboHits = hits;
    }


    /*
     * Script that updates glory on the clients
     */
    void OnChangeGlory(float glory)
    {
        numGlory = glory;
        if (glorySlider != null)
        {
            glorySlider.value = glory;
        }

        //check if player has super or not
        if(numGlory == 100)
        {
            hasSuper = true;
        }
        else if (hasSuper && numGlory < SUPER_LOSS_GLORY)
        {
            hasSuper = false;
        }
    }

    /*
     * Scipt that updates combo counter on clients
     */
    void OnChangeComboHits(int hits)
    {
        if (comboText != null)
        {
            comboHits = hits;
            comboText.text = "Combo: " + hits;
        }
    }

    /*
     * Script that applies knockback on the server
     */
    [Command]
    void CmdKnockback(GameObject defender, GameObject attacker, Vector2 dir, int hits)
    {
        RpcKnockback(defender, attacker, dir, hits);
    }

    /*
     * Script that applies knockback on the clients
     */
    [ClientRpc]
    void RpcKnockback(GameObject defender, GameObject attacker, Vector2 dir, int hits)
    {
        if (defender.tag == "Player")
        {
            defender.GetComponent<PlayerScript>().knockback(attacker, dir, hits);
        }
    }

    /**
     * Enter hit stun mode
     */
    public void knockback(GameObject attacker, Vector2 dir, int hits)
    {
        if (!hasAuthority)
        {
            return;
        }
        //check if reversaling correctly
        if (reversalEffective && Vector2.Angle(reversalDirection, dir) > 90f)
        {
            comboHits++;
            CmdReversalGloryUpdate(attacker, comboHits);
            CmdKnockback(attacker, player, reversalDirection, comboHits);
            comboHitInterval = 0;
        }

        //otherwise, take the hit
        else
        {
            reversalEffective = false;

            //end combo if there is one
            comboHits = 0;
            CmdChangeComboHits(comboHits);
            comboHitInterval = 0;

            //send player slighty more upwards if they are on the ground
            if (isGround() && dir.y < GROUND_KNOCKBACK_MODIFICATION)
            {
                dir.y += GROUND_KNOCKBACK_MODIFICATION;
                dir.Normalize();
            }

            //send player flying in direction of attack
            rb2D.velocity = dir * baseAttackForce * (1 + hits / 4);

            //stun player
            stunTimer = STUN_DURATION;
            actionLock = true;
            actionWaitFrames = STUN_DURATION;
        }
    }



    /**
     * Script for updating glory on server after successful reversal
     * 
     */
    [Command]
    void CmdReversalGloryUpdate(GameObject attacker, int hits)
    {
        comboHits = hits;

        //increase reversal-er glory
        if (numGlory + reversalGloryGain + gloryLostOnHit >= 100)
        {
            numGlory = 100;
        }
        else
        {
            numGlory += reversalGloryGain + gloryLostOnHit;
        }

        //decrease attacker glory
        if (attacker.GetComponent<PlayerScript>().numGlory - attacker.GetComponent<PlayerScript>().lastGloryIncrease < 0)
        {
            attacker.GetComponent<PlayerScript>().numGlory = 0;
        }
        else
        {
            attacker.GetComponent<PlayerScript>().numGlory -= attacker.GetComponent<PlayerScript>().lastGloryIncrease;
        }
    }

    /**
    * Script for Directional Influence
    */
    void DI()
    {
        rb2D.velocity = new Vector2(rb2D.velocity.x * KNOCKBACK_DAMPENING_COEF + DI_FORCE * Input.GetAxis("Horizontal"), rb2D.velocity.y * KNOCKBACK_DAMPENING_COEF + DI_FORCE * Input.GetAxis("Vertical"));
    }

    /**
     * returns if player is on ground
     */
    bool isGround()
    {
        return currentNormal.y >= Mathf.Sin(MIN_JUMP_RECOVERY_ANGLE);
    }
    // general check if any normal qualifies as on ground
    bool isGround(Vector2 normal)
    {
        return normal.y >= Mathf.Sin(MIN_JUMP_RECOVERY_ANGLE);
    }

    /**
     * returns if player is on a wall
     */
    bool isWall()
    {
        return currentNormal.y < Mathf.Sin(MIN_JUMP_RECOVERY_ANGLE) && currentNormal.y > Mathf.Sin(MAX_WJABLE_ANGLE) && !isAirborn();
    }
    // general check if any normal qualifies as on wall
    bool isWall(Vector2 normal)
    {
        return normal.y < Mathf.Sin(MIN_JUMP_RECOVERY_ANGLE) && normal.y > Mathf.Sin(MAX_WJABLE_ANGLE);
    }

    /**
     * returns if player is in air
     */
    bool isAirborn()
    {
        return currentNormal.Equals(new Vector2(0, 0));
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
        else
        {

            // add collided object to list
            touchingNormals.Add(getNormal(collision));
            touchingObjects.Add(collision.gameObject);


            // set player normal
            setPlayerNormal();
            
            // triggers landing animation if landed on ground
            if (isGround() && !isWall(getNormal(collision)))
            {
                animator.SetTrigger("hitGround");
            }
        }
        

    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!hasAuthority)
        {
            return;
        }

        // set player normal
        setPlayerNormal();


        // resets jump if the flattest ground is flat enough
        if (isGround()) jumps = JUMP_NUM;
        
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!hasAuthority)
        {
            return;
        }

        // remove object from list of objects touched
        touchingNormals.RemoveAt(touchingObjects.IndexOf(collision.gameObject));
        touchingObjects.RemoveAt(touchingObjects.IndexOf(collision.gameObject));

        // set player normal
        setPlayerNormal();


    }
    
    /**
     * Method to set the player's normal with any platforms it is touching
     */
    private void setPlayerNormal()
    {
        // set currentNormal to zero vector when leave a ground
        if (touchingNormals.Count == 0)
        {
            currentNormal = new Vector2(0, 0);
        }
        else
        {
            // set normal to most upwards-pointing (flattest ground)
            Vector2 newNormal = new Vector2(0, -1);
            foreach (Vector2 normal in touchingNormals)
            {
                if (newNormal.y < normal.y)
                {
                    newNormal = normal;
                }
            }
            currentNormal = newNormal;
        }
    }

    /**
     * Method to get the normal of a Collision2D object
     */
    private Vector2 getNormal(Collision2D collision)
    {
        // get points of contact with platforms
        ContactPoint2D[] cps = new ContactPoint2D[2];
        collision.GetContacts(cps);
        ContactPoint2D cp = cps[0];
        return cp.normal;
    }
}
