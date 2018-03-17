﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class LocalPlayerScript : NetworkBehaviour
{

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
    public float superRadius;
    public LayerMask mask;
    public GameObject gloryPrefab;
    public Slider glorySlider;
    public Text comboText;
    public float baseGloryGain;
    public float gloryLostOnHit;
    public float reversalGloryGain;
    public float lastGloryIncrease = 0;

    // private variables
    private int characterSelection = 2;
    private int jumps;
    private bool canJump;
    protected Vector2 currentNormal;
    protected float stickyWallTimer;
    private float stunTimer;
    protected Rigidbody2D rb2D;
    private Collider2D c2D;
    private bool actionLock = false;
    private float actionWaitFrames;
    private float actionWaitedFrames = 0;
    private bool attackButtonHeld = false;
    private int attackFrozeFrames = 0;
    private float blinkFrames = 0;
    private float blinkTimer = 0;
    private bool teleported = false;
    private bool reversalEffective = false;
    private Vector2 reversalDirection;
    protected GameObject glory;
    [SyncVar]
    protected bool hasSuper = false;
    private bool startedSuper = false;
    protected Animator animator;
    private List<GameObject> touchingObjects = new List<GameObject>();
    private List<Vector2> touchingNormals = new List<Vector2>();
    private float[] xVelTracker;
    private float[] xInputTracker;
    private bool boosting = false;
    [SyncVar(hook = "OnChangeComboHits")]
    protected int comboHits = 0;
    private float comboHitInterval = 0;
    protected float inputX = 0;
    protected float inputY = 0;
    protected bool jumpInput = false;
    protected bool reversalInput = false;
    protected bool attackInput = false;
    protected bool blinkInput = false;
    protected bool superInput = false;

    // constants
    public const float GROUND_RUN_FORCE = 2; // How fast player can attain intended velocity on ground
    public const float AIR_RUN_FORCE = 0.5f; // .... in air
    public const float MAX_SPEED = 10; // maximum horizontal speed
    public const float JUMP_SPEED = 10; // jump height
    public const int JUMP_NUM = 1; // number of midair jumps without touching ground
    public const float WALLJUMP_SPEED = 15; // horizontal speed gained from wall-jumps
    public const float WALL_FALL_SPEED = -5; // maximum fall speed when on wall
    public const float FALL_SPEED = -10; // maximum fall speed
    public const float FALL_FORCE = 0.5f; // force of gravity
    public const float FALL_COEF = 2; // How much player can control fall speed. Smaller = more control (preferrably > 1 [see for yourself ;)])
    public const float MAX_WJABLE_ANGLE = -Mathf.PI / 18; // largest negative angle of a wall where counts as walljump
    public const float MIN_JUMP_RECOVERY_ANGLE = Mathf.PI / 4; // smallest angle of a wall where air jumps are recovered
    public const float STICKY_WJ_DURATION = 0.375f; // amount of seconds that player sticks to a wall after touching it
    public const float ATTACK_WAIT_FRAMES = 0.625f; // number of seconds a player must wait between attacks
    public const float ATTACK_FREEZE_FRAMES = 0.3f; //number of seconds a player freezes while attacking [DEPRECIATED]
    public const float COMBO_HIT_TIMER = 2.5f; //number of seconds a player must land the next attack within to continue a combo
    public const float TRUE_HIT_MULTIPLIER = 1.5f; //multiplier for glory increase for true hits 
    public const float STUN_DURATION = 1.25f; // amount of seconds that a player stays stunned
    public const float GROUND_KNOCKBACK_MODIFICATION = 0f; //amount increase to the y component of knockback velocity if player is on ground
    public const float KNOCKBACK_DAMPENING_COEF = 0.98f; // factor that knockback speed slows every second
    public const float DI_FORCE = 0.1f; // amount of influence of DI
    public const float REVERSAL_EFFECTIVE_TIME = 1.625f; //number of seconds in which a reversal is effective
    public const float REVERSAL_DURATION = 1.625f; //number of seconds a reversal lasts (effective time + end lag)
    public const float REVERSAL_SUCCESS_ANGLE = 90; //minimum angle between reversal and attack for reversal to be successful
    public const float SUPER_LOSS_GLORY = 85; //glory at which super is lost if player falls below

    public const float GLORY_ON_SUPER_MISS = 75; //glory player drops to for losing super
    public const float BLINK_VELOCITY = 0.75f; //target blink speed
    public const float BLINK_TIME = 0.125f; //how long the velocity blink lasts
    public const float BLINK_FRAMES = 1.3f; //how long the player needs to wait until velocity blinking again
    public const int TELEPORT_DISTANCE = 6; //teleport distance
    public const float TELEPORT_FRAMES = 2.5f; //frames until teleportation can happen again
    public const float TELEPORT_TIME = 0.25f; //frames until player can move again
    public const float SUPER_CHARGE_FRAMES = 1.5f; //number of frames a super takes to charge
    public const float SUPER_END_LAG = 1.25f; //number of frames player stalls without doing anything after a super
    // if turn speed to 1 or -1 with a change of at least the threshold in at most timelimit number of frames, boost applied
    public const int BOOST_TIMELIMIT = 2;
    public const float BOOST_THRESHOLD = 0.75f;
    public const float BOOST_SPEED = 20; // speed of boost


    // Use this for initialization
    protected virtual void Start()
    {
        jumps = 0;
        canJump = false;
        currentNormal = Vector2.zero;
        stickyWallTimer = 0;
        stunTimer = 0;
        xVelTracker = new float[BOOST_TIMELIMIT + 1];
        xInputTracker = new float[BOOST_TIMELIMIT + 1];

        rb2D = gameObject.GetComponent<Rigidbody2D>();
        c2D = gameObject.GetComponent<Collider2D>();

        animator = GetComponent<Animator>();

        createMeter();
    }


    /**
     * Script for creating Glory meters
     */
    public virtual void createMeter()
    {
        //instantiate UI element and place on canvas
        glory = Instantiate(gloryPrefab);
        var canvas = GameObject.Find("Canvas");
        RectTransform gloryTransform = glory.GetComponent<RectTransform>();
        gloryTransform.SetParent(canvas.transform);

        //position meter appropriately depending on if it corresponds to local player or enemy player
        /*if (!hasAuthority)
        {
            gloryTransform.anchorMin = new Vector2(1, 1);
            gloryTransform.anchorMax = new Vector2(1, 1);
            gloryTransform.pivot = new Vector2(1, 1);
            gloryTransform.anchoredPosition = new Vector3(-100, 0, 0);
        }
        else
        {*/
        gloryTransform.anchoredPosition = new Vector3(0, 0, 0);
        //}

        //assign slider and comboText to variables so they can be modified easily
        glorySlider = glory.transform.Find("Slider").gameObject.GetComponent<Slider>();
        comboText = glory.transform.Find("Combo Text").gameObject.GetComponent<Text>();
        glorySlider.value = numGlory;
    }

    // Update is called once per frame
    virtual protected void Update()
    {
        assignInputs();

        //flips sprite if necessary (on all clients)
        gameObject.GetComponent<SpriteRenderer>().flipX = !facingRight;

        // Updates Animator variables
        animator.SetFloat("xDir", inputX);
        animator.SetFloat("yDir", inputY);
        animator.SetFloat("yVel", rb2D.velocity.y / -FALL_SPEED);
        animator.SetBool("isMoving", Mathf.Abs(Input.GetAxis("Horizontal")) > 0);
        animator.SetBool("isAirborn", isAirborn());
        animator.SetBool("onWall", isWall());
        animator.SetInteger("StunTimer", (int)stunTimer);
        int attackNum = 0;
        if (Mathf.Abs(inputY) > Mathf.Abs(inputX))
        {
            attackNum = (int)Mathf.Sign(inputY);
        }
        animator.SetInteger("attackNum", attackNum);

        // Decreases stun timer
        if (stunTimer > 0) stunTimer -= Time.deltaTime;

        //keep a timer for between hits in a combo
        if (comboHits > 0)
        {
            comboHitInterval += Time.deltaTime;
            if (comboHitInterval >= COMBO_HIT_TIMER)
            {
                updateComboHits(0);
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

            //check to see if player has started super and charge time has elapsed
            if (startedSuper && actionWaitedFrames >= SUPER_CHARGE_FRAMES)
            {
                launchSuper();
            }

            //see if action lock duration has expired - if so, escape action lock
            actionWaitedFrames += Time.deltaTime;
            if (actionWaitedFrames >= actionWaitFrames)
            {
                actionLock = false;
                reversalEffective = false;
                actionWaitedFrames = 0;
            }

        }

        //freeze player if they are mid-attack
        /*if (attacking)
        {
            rb2D.velocity = Vector2.zero;
            attackFrozeFrames++;
            if (attackFrozeFrames >= ATTACK_FREEZE_FRAMES)
            {
                attackFrozeFrames = 0;
                attacking = false;
            }
        }*/
    }

    //set input values (done this way because different inputs are used in derrived classes)
    protected virtual void assignInputs()
    {
        inputX = Input.GetAxisRaw("Horizontal");
        inputY = Input.GetAxisRaw("Vertical");
        jumpInput = Input.GetAxisRaw("Jump") != 0;
        attackInput = Input.GetAxisRaw("Attack") != 0;
        reversalInput = Input.GetAxisRaw("Reversal") != 0;
        blinkInput = Input.GetAxisRaw("Blink") != 0;
        superInput = Input.GetAxisRaw("Super") != 0;
    }

    void FixedUpdate()
    {
        if (stunTimer <= 0)
        {
            rb2D.sharedMaterial = regularMaterial;
            rotate(Vector3.zero);

            if (blinkTimer <= 0)
            { //blinkTimer is the amount of time the blink takes to complete, not the amount of frames until next blink
                if (!actionLock)
                {
                    startBlink();
                    gravity();
                    jump();
                    run();
                    boost();
                    flipSprite();
                    attack();
                    reversal();
                    StartSuper();
                }
                else if (actionWaitedFrames >= 20)
                {
                    gravity();
                }
            }
            else
            {
                blink();
            }
        }
        else
        {
            rb2D.sharedMaterial = stunMaterial;
            DI();
        }


    }

    protected virtual void flipSprite()
    {
        //flip sprite based on player input if they are not wall hugging
        if (stickyWallTimer <= 0)
        {
            facingRight = inputX > 0 || (inputX == 0 && facingRight);
        }
    }

    protected virtual void wallJumpFlipSprite()
    {
        if (rb2D.velocity.x == 0)
        {
            return;
        }
        facingRight = rb2D.velocity.x > 0;
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
    protected virtual float sticky()
    {
        float goalSpeed = 0;
        // If standing on flat enough ground or in air, reset sticky timer
        if (isAirborn() || isGround())
        {
            stickyWallTimer = 0;
        }
        // if touching a platform close enough to a walljumpable wall, start sticking
        else if (isWall() && stickyWallTimer <= 0)
        {
            stickyWallTimer = STICKY_WJ_DURATION;
        }

        // if still sticking, flip sprite appropriately, run into wall and decrease timer
        if (stickyWallTimer > 0)
        {
            //flip sprite away from wall
            facingRight = currentNormal.x > 0;

            goalSpeed = -MAX_SPEED * currentNormal.x;
            if (Mathf.Sign(currentNormal.x) == Mathf.Sign(inputX))
            {
                stickyWallTimer -= Time.deltaTime;
            }
        }

        // once timer is out, resume normal movement
        if (stickyWallTimer <= 0)
        {
            goalSpeed = MAX_SPEED * inputX;
        }

        return goalSpeed;
    }

    /**
     * Script for boosting
     */
    void boost()
    {
        for (int i = xVelTracker.Length - 1; i >= 1; i--)
        {
            xVelTracker[i] = xVelTracker[i - 1];
            xInputTracker[i] = xInputTracker[i - 1];
        }
        xVelTracker[0] = rb2D.velocity.x;
        xInputTracker[0] = inputX;

        if (Mathf.Abs(xInputTracker[0]) >= 0.8f && Mathf.Abs(xInputTracker[0] - xInputTracker[xInputTracker.Length - 1]) > BOOST_THRESHOLD)
        {
            boosting = true;
        }
        else if (Mathf.Abs(xInputTracker[0]) != 1)
        {
            boosting = false;
        }
        if (!isGround())
        {
            boosting = false;
        }
        if (xVelTracker[xVelTracker.Length - 1] * xVelTracker[xVelTracker.Length - 2] <= 0 && boosting)
        {
            rb2D.velocity = new Vector2(xInputTracker[0] * BOOST_SPEED, rb2D.velocity.y);
            // goalSpeed = rb2D.velocity.x; (maybe not necessary)
            boosting = false;
            Debug.Log("boosting");
        }
    }

    /**
     * Script for Jumping
     */
    void jump()
    {
        // checks if touching walls
        if (jumpInput && canJump)
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
            if (isWall())
            {
                wallJumpFlipSprite();
            }

            // cannot jump until release jump key
            canJump = false;
        }

        // resets jump
        if (!jumpInput)
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
        if (stickyWallTimer <= 0)
        {
            fallSpeed = FALL_SPEED;
        }
        else
        {
            fallSpeed = WALL_FALL_SPEED;
        }
        if (stunTimer <= 0)
        {
            fallSpeed *= (1 - inputY / FALL_COEF);
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
        if (attackInput)
        {
            //cancel attacker's momentum
            rb2D.velocity = Vector2.zero;

            //raycast to see if someone is hit with the attack - mask out attacker's layer
            Vector2 direction = getDirection();
            direction.Normalize();
            Vector2 origin = new Vector2(player.GetComponent<Transform>().position.x, player.GetComponent<Transform>().position.y);
            Debug.DrawRay(origin, direction * attackRadius, Color.blue, 1f);
            gameObject.layer = 2;
            RaycastHit2D hit = Physics2D.Raycast(origin: origin, direction: direction, distance: attackRadius);
            gameObject.layer = 0;

            //if attack is successful:
            if (hit.rigidbody != null)
            {
                comboHits++;
                var trueHit = (comboHitInterval <= STUN_DURATION) && (comboHits > 1);
                attackGloryUpdate(hit.rigidbody.gameObject, comboHits, trueHit);
                startKnockback(hit.rigidbody.gameObject, direction, comboHits);
                comboHitInterval = 0;
                blinkFrames = 0;
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

    /**
     * Script for intiaiting all blink processes
     */
    void startBlink()
    {
        switch (characterSelection)
        {
            case 1:
                if (blinkInput && blinkFrames <= 0)
                { //currently set to 'b'
                    blinkTimer = BLINK_TIME;
                    blinkFrames = BLINK_FRAMES;
                }
                break;
            case 2:
                if (blinkInput && blinkFrames <= 0)
                { //currently set to 'b'
                    blinkTimer = TELEPORT_TIME;
                    blinkFrames = TELEPORT_FRAMES;
                }
                break;
            default:
                break;
        }

        if (blinkFrames > 0)
        {
            blinkFrames -= Time.deltaTime;
        }
    }

	/**
     * Script for blinking
     */
	void blink(){

        blinkTimer -= Time.deltaTime;

        switch (characterSelection)
        {
            case 1:
                if (blinkInput)
                { //currently set to 'b'
                    rb2D.velocity = BLINK_VELOCITY * getDirection();
                }
                else
                {
                    rb2D.velocity = new Vector2(0, 0);
                    blinkTimer = 0;
                }
                break;
            case 2:

                //bool teleported is to prevent the user from simply hold down the button
                if (blinkInput && !teleported)
                {//currently set to 'b'
                    float distance = TELEPORT_DISTANCE;

                    int layerMask = LayerMask.GetMask("Ignore Raycast");
                    Vector2 direction = getDirection();
                    Vector2 origin = new Vector2(player.GetComponent<Transform>().position.x, player.GetComponent<Transform>().position.y);
                    RaycastHit2D hit = Physics2D.Raycast(origin: origin, direction: direction, distance: TELEPORT_DISTANCE, layerMask: layerMask);
                    if (hit.collider != null)
                    {
                        distance = hit.distance;
                    }

                    rb2D.position = new Vector2(rb2D.position.x + distance * getDirection().x,
                        rb2D.position.y + distance * getDirection().y);

                    teleported = true;
                }
                else
                {
                    rb2D.velocity = new Vector2(0, 0);
                    blinkTimer = 0;
                    teleported = false;
                }
                break;
            default:
                break;
        }
    }

    /**
     * Script to see if player is reversaling (actual impact is handled in knockback)
     */
    void reversal()
    {
        //check if player is pushing reversal button and can reversal
        if (reversalInput)
        {
            //cancel momentum
            rb2D.velocity = Vector2.zero;

            reversalDirection = getDirection();
            actionLock = true;
            reversalEffective = true;
            actionWaitFrames = REVERSAL_DURATION;

            reversalAnimation();
        }
    }

    protected virtual void reversalAnimation()
    {
        //trigger animation
        animator.SetTrigger("reversaling");
    }

    /**
     * Script for StartinSuper
     */
    void StartSuper()
    {
        //check if can super and is super-ing
        if (hasSuper && superInput)
        {
            Debug.Log("super");
            //cancel momentum
            rb2D.velocity = Vector2.zero;
            actionLock = true;
            actionWaitFrames = SUPER_CHARGE_FRAMES + SUPER_END_LAG;
            startedSuper = true;
        }
    }

    /**
     * Script for firing super
     */
    void launchSuper()
    {
        if (startedSuper && actionWaitedFrames >= SUPER_CHARGE_FRAMES)
        {
            //circlecast to see if someone is hit with the super - mask out attacker's layer
            Vector2 direction = getDirection();
            direction.Normalize();
            Vector2 origin = new Vector2(player.GetComponent<Transform>().position.x, player.GetComponent<Transform>().position.y);
            gameObject.layer = 2;
            RaycastHit2D hit = Physics2D.CircleCast(origin: origin, radius: superRadius, direction: direction, distance: Mathf.Infinity);
            gameObject.layer = 0;

            if (hit.rigidbody != null && hit.rigidbody.gameObject.tag == "Player" || hit.rigidbody.gameObject.tag == "PlayerAI")
            {
                killPlayer(hit.rigidbody.gameObject);
            }
            startedSuper = false;
        }
    }

    protected virtual void killPlayer(GameObject go)
    {
        Destroy(go);
    }

    /**
     * Script for determining direction of player actions (attacks, reversals, and blinks)
     */
    protected virtual Vector2 getDirection()
    {
        //determine horizontal component of attack's direction
        float horizontalDirection;
        //if attacker is not moving, attack direction is the direction they are facing
        if (inputX == 0 && inputY == 0)
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
            horizontalDirection = inputX;
        }
        Vector2 direction = new Vector2(horizontalDirection, inputY);
        return direction;
    }


    /*
     * Script for updating glory after an attack on the server
     */
    protected virtual void attackGloryUpdate(GameObject otherPlayer, int hits, bool trueHit)
    {
        //increase attacker glory
        updateComboHits(hits);
        float trueMultiplier = 1;
        if (trueHit)
        {
            trueMultiplier = TRUE_HIT_MULTIPLIER;
        }
        float gloryIncrease = baseGloryGain * (1.0f + comboHits / 10.0f) * trueMultiplier;
        if (numGlory + gloryIncrease >= 100)
        {
            lastGloryIncrease = (100 - numGlory);
            changeGlory(100);
        }
        else
        {
            changeGlory(numGlory + gloryIncrease);
            lastGloryIncrease = gloryIncrease;
        }

        //decrease hit person glory
        if (otherPlayer.GetComponent<LocalPlayerScript>().numGlory - gloryLostOnHit < 0)
        {
            otherPlayer.GetComponent<LocalPlayerScript>().changeGlory(0);
        }
        else
        {
            otherPlayer.GetComponent<LocalPlayerScript>().changeGlory(otherPlayer.GetComponent<LocalPlayerScript>().numGlory - gloryLostOnHit);
        }
    }

    /*
     * Script that updates glory
     */
    public virtual void changeGlory(float glory)
    {
        numGlory = glory;
        if (glorySlider != null)
        {
            glorySlider.value = glory;
        }

        //check if player has super or not
        if (numGlory == 100)
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
    protected virtual void updateComboHits(int hits)
    {
        if (comboText != null)
        {
            comboHits = hits;
            comboText.text = "Combo: " + hits;
        }
    }

    public virtual void startKnockback(GameObject defender, Vector2 dir, int hits)
    {
        defender.GetComponent<LocalPlayerScript>().knockback(player, dir, hits);
    }

    /**
     * Enter hit stun mode
     */
    public virtual void knockback(GameObject attacker, Vector2 dir, int hits)
    {
        //check if reversaling correctly
        if (reversalEffective && Vector2.Angle(reversalDirection, dir) > 90f)
        {
            updateComboHits(comboHits + 1);
            reversalGloryUpdate(attacker, comboHits);
            startKnockback(attacker, reversalDirection, comboHits);
            comboHitInterval = 0;
            actionWaitFrames = 0;
            blinkFrames = 0;
        }

        //otherwise, take the hit
        else
        {
            reversalEffective = false;
            startedSuper = false;

            //end combo if there is one
            updateComboHits(0);
            comboHitInterval = 0;

            //send player slighty more upwards if they are on the ground
            if (isGround() && dir.y < GROUND_KNOCKBACK_MODIFICATION)
            {
                dir.y += GROUND_KNOCKBACK_MODIFICATION;
                dir.Normalize();
            }

            //send player flying in direction of attack
            rb2D.velocity = dir * baseAttackForce * (1.0f + hits / 4.0f);

            //rotate player perpendicular to attack
            facingRight = dir.x < 0;
            if (!facingRight)
            {
                rotate(new Vector3(0, 0, Mathf.Rad2Deg * Mathf.Atan2(dir.y, dir.x)));
            }
            else
            {
                rotate(new Vector3(0, 0, Mathf.Rad2Deg * Mathf.Atan2(dir.y, dir.x) - 180));
            }

            //stun player
            stunTimer = STUN_DURATION;
            actionLock = true;
            actionWaitFrames = STUN_DURATION;

            //trigger animation
            animator.SetTrigger("Hitstun");
        }
    }

    /*
     * Script for syncing rotation on server
     */
    protected virtual void rotate(Vector3 rotation)
    {
        GetComponent<Transform>().eulerAngles = rotation;
    }

    /**
     * Script for updating glory on server after successful reversal
     */
    protected virtual void reversalGloryUpdate(GameObject attacker, int hits)
    {
        updateComboHits(hits);

        //increase reversal-er glory
        if (numGlory + reversalGloryGain + gloryLostOnHit >= 100)
        {
            changeGlory(100);
        }
        else
        {
            changeGlory(numGlory + reversalGloryGain + gloryLostOnHit);
        }

        //decrease attacker glory
        if (attacker.GetComponent<LocalPlayerScript>().numGlory - attacker.GetComponent<LocalPlayerScript>().lastGloryIncrease < 0)
        {
            attacker.GetComponent<LocalPlayerScript>().changeGlory(0);
        }
        else
        {
            attacker.GetComponent<LocalPlayerScript>().changeGlory(attacker.GetComponent<LocalPlayerScript>().numGlory - attacker.GetComponent<LocalPlayerScript>().lastGloryIncrease);
        }
    }

    /**
    * Script for Directional Influence
    */
    void DI()
    {
        rb2D.velocity = new Vector2(rb2D.velocity.x * KNOCKBACK_DAMPENING_COEF + DI_FORCE * inputX, rb2D.velocity.y * KNOCKBACK_DAMPENING_COEF + DI_FORCE * inputY);
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
        return currentNormal.Equals(Vector2.zero);
    }

    /**
     * Collision Detector
     */
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "PlayerAI")
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

            //rotate the player if in hitstun
            if (stunTimer > 0)
            {
                Debug.Log(Vector2.SignedAngle(rb2D.velocity, currentNormal));
            }
        }


    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "PlayerAI")
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
        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "PlayerAI")
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
            currentNormal = Vector2.zero;
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

    //Script used only for networked children to update comboHits on clients
    protected void OnChangeComboHits(int hits)
    {
        Debug.Log("comboHits changed");
        if (comboText != null)
        {
            comboHits = hits;
            comboText.text = "Combo: " + hits;
        }
    }

    //script used only for networked children to update glory on clients
    protected void OnChangeGlory(float glory)
    {
        Debug.Log("changed glory");
        numGlory = glory;
        if (glorySlider != null)
        {
            glorySlider.value = glory;
        }

        //check if player has super or not
        if (numGlory == 100)
        {
            hasSuper = true;
        }
        else if (hasSuper && numGlory < SUPER_LOSS_GLORY)
        {
            hasSuper = false;
        }
    }
}
