using UnityEngine;
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
    public LayerMask mask;
    
    void FixedUpdate () {
        Debug.Log(attackButtonHeld);

        if (!hasAuthority)
        {
            return;
        }

        //check to see if player can attack again (after "waitedFrames" num of frames 
        //have elapsed since previous attack)
        if (!canAttack)
        {
            waitedFrames++;
            if(waitedFrames == waitFrames)
            {
                canAttack = true;
                waitedFrames = 0;
            }
        }

        //check to see if attack button is held down - attack occurs once the button is released
        if (Input.GetAxis("Fire1") != 0)
        {
            attackButtonHeld = true;
        }
        else
        {
            //check that button was held in previous frame (meaning it was released this frame
            //so attack should initiate)
            if (attackButtonHeld && canAttack)
            {
                //cancel attacker's momentum
                player.GetComponent<Rigidbody2D>().velocity = new Vector2 (0, 0);

                //determine horizontal component of attack's direction
                float horizontalDirection;
                //if attacker is not moving, attack direction is the direction they are facing
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
                
                //raycast to see if someone is hit with the attack - mask out attacker's layer
                Vector2 direction = new Vector2(horizontalDirection, Input.GetAxis("Vertical"));
                direction.Normalize();
                Vector2 origin = new Vector2(player.GetComponent<Transform>().position.x, player.GetComponent<Transform>().position.y);
                Debug.DrawRay(origin, direction * attackRadius, Color.blue, 1f);
                RaycastHit2D hit = Physics2D.Raycast(origin: origin, direction: direction, distance: attackRadius, layerMask: mask.value);
                if (hit.rigidbody != null)
                {
                    CmdKnockback(hit.rigidbody.gameObject, direction);
                }

                //cannot attack immediately after launching an attack
                canAttack = false;
            }

            //keep track that attack button wasn't held during this frame
            attackButtonHeld = false;
        }
	}

    //apply knockback to attack recipient on the server
    [Command]
    void CmdKnockback(GameObject go, Vector2 dir)
    {
        go.GetComponent<Rigidbody2D>().velocity = dir * baseAttackForce;
        if (go.tag == "player")
        {
            go.GetComponent<PlayerMovement>().hitStun();
        }
        RpcKnockback(go, dir);

    }

    //apply knowckback to attack recipients on the clients
    [ClientRpc]
    void RpcKnockback(GameObject go, Vector2 dir)
    {
        go.GetComponent<Rigidbody2D>().velocity = dir * baseAttackForce;
        if (go.tag == "player")
        {
            go.GetComponent<PlayerMovement>().hitStun();
        }
    }
}
