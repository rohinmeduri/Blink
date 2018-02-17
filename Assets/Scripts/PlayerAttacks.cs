﻿using UnityEngine;
using UnityEngine.Networking;

public class PlayerAttacks : NetworkBehaviour {
    // Update is called once per frame
    private bool canAttack = true;
    private bool attackButtonHeld = false;
    public GameObject player;
    public float attackRadius;
    public float baseAttackForce;
    public int waitFrames;
    private float waitedFrames = 0;
    public float playerWidth;
    public float playerHeight;
    
    void FixedUpdate () {
        if (!hasAuthority)
        {
            return;
        }

        if (!canAttack)
        {
            waitedFrames++;
            if(waitedFrames == waitFrames)
            {
                canAttack = true;
                waitedFrames = 0;
            }
        }

        if (Input.GetAxis("Fire1") != 0)
        {
            attackButtonHeld = true;
        }

        else
        {
            if (attackButtonHeld == true && canAttack)
            {
                player.GetComponent<Rigidbody2D>().velocity = new Vector2 (0, 0);
                float horizontalDirection;
                if (Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0)
                {
                    if (player.GetComponent<PlayerMovement>().facingRight)
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
                direction.Normalize();
                Vector2 origin = new Vector2(player.GetComponent<Transform>().position.x + playerWidth * direction.x, player.GetComponent<Transform>().position.y + playerHeight * direction.y);
                Debug.DrawRay(origin, direction * attackRadius, Color.blue, 1f);
                RaycastHit2D hit = Physics2D.Raycast(origin: origin, direction: direction, distance: attackRadius);
                if (hit.rigidbody != null && hit.rigidbody != player.GetComponent<Rigidbody2D>())
                {
                    Debug.Log("hit");
                    //hit.rigidbody.AddForce(direction * attackForce);
                    CmdKnockback(hit.rigidbody.gameObject, direction);
                }
                canAttack = false;
            }
            attackButtonHeld = false;
        }
	}

    [Command]
    void CmdKnockback(GameObject go, Vector2 dir)
    {
        go.GetComponent<Rigidbody2D>().velocity = dir * baseAttackForce;
        go.GetComponent<PlayerMovement>().hitStun();
        RpcKnockback(go, dir);
    }

    [ClientRpc]
    void RpcKnockback(GameObject go, Vector2 dir)
    {
        go.GetComponent<Rigidbody2D>().velocity = dir * baseAttackForce;
        go.GetComponent<PlayerMovement>().hitStun();
    }
}
