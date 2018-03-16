using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAIScript : LocalPlayerScript {
    private float directionInputX = 0;
    private float directionInputY = 0;
    private GameObject enemy;

    protected override void Update()
    {
        //find enemy player
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Player");
        for (var i = 0; i < enemies.Length; i++)
        {
            if (enemies[i] != gameObject)
            {
                enemy = enemies[i];
                break;
            }
        }

        base.Update();
    }

    public override void createMeter()
    {
        base.createMeter();

        RectTransform gloryTransform = glory.GetComponent<RectTransform>();
        gloryTransform.anchorMin = new Vector2(1, 1);
        gloryTransform.anchorMax = new Vector2(1, 1);
        gloryTransform.pivot = new Vector2(1, 1);
        gloryTransform.anchoredPosition = new Vector3(-100, 0, 0);
    }

    protected override void assignInputs()
    {
        //determine movement and input based on enemy position
        Transform enemyTransform = enemy.GetComponent<Transform>();
        Transform myTransform = GetComponent<Transform>();
        Vector2 input = (enemyTransform.position - myTransform.position);
        Vector2 directionInput = input;
        directionInput.Normalize();
        directionInput.x += Random.Range(-0.25f, 0.25f);
        directionInput.y += Random.Range(-0.25f, 0.25f);
        directionInput.Normalize();
        directionInputX = directionInput.x;
        directionInputY = directionInput.y;

        blinkInput = input.magnitude > attackRadius * 0.8f;
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
        }

        inputX = input.x;
        inputY = input.y;
    }

    protected override Vector2 getDirection()
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
}
