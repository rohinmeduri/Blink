using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerAIScript : LocalPlayerScript {
    private float directionInputX = 0;
    private float directionInputY = 0;
    private GameObject enemy;
    private List<Vector3> positionTracker = new List<Vector3>();
    private List<float> timeTracker = new List<float>();
    private float timeCounter = 0;
    private int playerDifficulty;
    private bool attackCheck = false;
    private bool blinkCheck = false;
    private bool superCheck = false;
    private bool countBlink = false;

    public readonly float[] REACTION_TIME = {0.5f, 0.4f, 0.25f}; //how long AI takes to react to other player's movements
    public readonly float[] BLINK_DISTANCE_FACTOR = {4f, 2f, 1.2f};

    protected override void Update()
    {
        hasSuper = true;
        //find enemy player
        GameObject[] playerEnemies = GameObject.FindGameObjectsWithTag("Player");
        GameObject[] AIenemies = GameObject.FindGameObjectsWithTag("PlayerAI");
        GameObject[] enemies = playerEnemies.Concat(AIenemies).ToArray();
        float enemyDistance = Mathf.Infinity;

        for (var i = 0; i < enemies.Length; i++)
        {
            Vector2 myTransform = GetComponent<Transform>().position;

            if (enemies[i] != gameObject)
            {
                Vector2 enemyTransform = enemies[i].GetComponent<Transform>().position;
                Vector2 distanceVector = (enemyTransform - myTransform);
                if (distanceVector.magnitude < enemyDistance)
                {
                    enemy = enemies[i];
                    enemyDistance = distanceVector.magnitude;
                }
            }
        }

        if(stunTimer > 0)
        {
            blinkCheck = false;
            superCheck = false;
            attackCheck = false;
            countBlink = false;
        }

        base.Update();
    }

    

    public void setPlayerDifficulty(int difficulty)
    {
        if(difficulty > 2)
        {
            difficulty = 2;
        }
        else if(difficulty < 0)
        {
            difficulty = 0;
        }
        playerDifficulty = difficulty;
    }

    protected override void assignInputs()
    {
        //check that that there is still an enemy, otherwise no input
        if (enemy != null)
        {
            timeCounter += Time.deltaTime;
            positionTracker.Add((enemy.GetComponent<Transform>().position));
            timeTracker.Add(Time.deltaTime);

            //determine movement and input based on enemy position (in the past)
            if (timeCounter >= REACTION_TIME[playerDifficulty])
            {
                Vector2 enemyTransform = positionTracker[0];
                Vector2 myTransform = GetComponent<Transform>().position;
                Vector2 input = (enemyTransform - myTransform);
                Vector2 directionInput = input;
                directionInput.Normalize();
                if (playerDifficulty == 0)
                {
                    directionInput.x += Random.Range(-0.3f, 0.3f);
                    directionInput.y += Random.Range(-0.3f, 0.3f);
                }
                if (playerDifficulty == 1)
                {
                    directionInput.x += Random.Range(-0.2f, 0.2f);
                    directionInput.y += Random.Range(-0.2f, 0.2f);
                }
                if (playerDifficulty == 1)
                {
                    directionInput.x += Random.Range(-0.1f, 0.1f);
                    directionInput.y += Random.Range(-0.1f, 0.1f);
                }

                directionInput.Normalize();
                directionInputX = directionInput.x;
                directionInputY = directionInput.y;

                superInput = hasSuper;
                blinkInput = input.magnitude > attackRadius * BLINK_DISTANCE_FACTOR[playerDifficulty] && !superInput;

                jumpInput = input.y > attackRadius;
                if (input.magnitude < attackRadius * 0.8f)
                {
                    facingRight = input.x >= 0;
                    input = Vector2.zero;
                    reversalInput = comboHits == 0 && Mathf.CeilToInt(Random.value * 4.0f) == 4;
                    attackInput = !reversalInput;
                }
                else
                {
                    input.Normalize();
                    attackInput = false;
                    reversalInput = false;
                }

                inputX = input.x;
                inputY = input.y;

                while (timeCounter >= REACTION_TIME[playerDifficulty])
                {
                    timeCounter -= timeTracker[0];

                    positionTracker.RemoveAt(0);
                    timeTracker.RemoveAt(0);
                }
            }

            if (hasSuper && playerDifficulty > 0)
            {
                winCombo();
            }
        }
        else
        {
            superInput = false;
            blinkInput = false;
            jumpInput = false;
            attackInput = false;
            reversalInput = false;
            inputX = 0;
            inputY = 0;
        }
    }

    /*
     * script that makes AI do a hit + super combo for medium, and hit + blink + super combo for hard
     */
    void winCombo()
    {
        if (playerDifficulty == 2)
        {
            superInput = false;
            if (attackCheck)
            {
                countBlink = true;
                blinkInput = true;
            }
            if (attackLanded)
            {
                attackCheck = true;
            }
        }
        else
        {
            countBlink = true;
            blinkInput = true;
            attackCheck = true;
        }

        if (superCheck)
        {
            superInput = false;
            blinkCheck = false;
            countBlink = false;
            superCheck = false;
            attackCheck = false;
        }

        if (blinkCheck)
        {
            blinkInput = false;
            superInput = true;
        }
        else
        {
            superInput = false;
        }

        if (teleported && countBlink)
        {
            blinkCheck = true;
        }

        if (launchedSuper)
        {
            superCheck = true;
        }

        if (attackCheck & !blinkCheck)
        {
            Vector2 enemyTransform = positionTracker[0];
            Vector2 myTransform = GetComponent<Transform>().position;
            Vector2 input = (myTransform - enemyTransform);
            Vector2 directionInput = input;
            directionInput.y = 1;
            directionInput.Normalize();
            inputX = directionInput.x;
            inputY = directionInput.y;
            directionInputX = directionInput.x;
            directionInputY = directionInput.y;
            jumpInput = input.y <= 0;
        }
    }

    public override Vector2 getDirection()
    {
        //determine horizontal component of attack's direction
        float horizontalDirection;
        //if attacker is not moving, attack direction is the direction they are facing
        if (directionInputX == 0 && directionInputY == 0)
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
            horizontalDirection = directionInputX;
        }
        Vector2 direction = new Vector2(horizontalDirection, directionInputY);
        return direction;
    }

    protected override void DI()
    {
        if (playerDifficulty > 0)
        {
            //DI away from enemy
            Vector2 direction = new Vector2(inputX * -1, inputY * -1);
            direction.Normalize();
            Vector2 origin = new Vector2(gameObject.GetComponent<Transform>().position.x, gameObject.GetComponent<Transform>().position.y);

            //make sure not DI'ing into a wall
            int layerMask = LayerMask.GetMask("Ignore Raycast");
            RaycastHit2D hit = Physics2D.Raycast(origin: origin, direction: direction, distance: attackRadius * 2, layerMask: layerMask);
            if (hit.collider != null)
            {
                //DI perpendicular to direction if heading towards a wall
                direction = Quaternion.Euler(0, 0, 90) * direction;
                hit = Physics2D.Raycast(origin: origin, direction: direction, distance: attackRadius * 2, layerMask: layerMask);

                //if now heading toward another wall (i.e. in a corner), flip DI 180 degrees
                if (hit.collider != null)
                {
                    direction = Quaternion.Euler(0, 0, 180) * direction;
                }
            }

            inputX = direction.x;
            inputY = direction.y;
            base.DI();
        }
    }
}
