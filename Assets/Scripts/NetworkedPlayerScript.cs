using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkedPlayerScript : LocalPlayerScript {

    private NetworkAnimator networkAnimator;
    private int IDCounter = 2;
    private GameObject IDAssigner;

    public override void OnStartAuthority()
    {
        if (hasAuthority)
        {
            networkAnimator = GetComponent<NetworkAnimator>();
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            IDAssigner = GameObject.Find("ID Assigner");
            foreach(var i in players)
            {
                if(i == gameObject)
                {
                    setPlayerID(1);
                    gameObject.layer = 2;
                    continue;
                }
                else
                {
                    i.GetComponent<NetworkedPlayerScript>().setPlayerID(IDAssigner.GetComponent<IDAssigner>().getID());
                    IDCounter++;
                }
            }
           CmdNewPlayer();
        }
    }

    [Command]
    void CmdNewPlayer()
    {
        RpcNewPlayer();
    }

    [ClientRpc]
    void RpcNewPlayer()
    {
        newPlayer();
    }


    //this function is used for non-local versions of this player
    public void newPlayer()
    {
        if (!hasAuthority)
        {
            IDAssigner = GameObject.Find("ID Assigner");
            setPlayerID(IDAssigner.GetComponent<IDAssigner>().getID());
        }
    }

    // Update is called once per frame
    protected override void Update()
    {
        //flips sprite if necessary (on all clients)
        gameObject.GetComponent<SpriteRenderer>().flipX = !facingRight;

        if (!hasAuthority)
        {
            return;
        }

        base.Update();
    }

    //'pausing' in online multiplayer doesn't actually stop game - only allows you to quit
    protected override void pauseGame()
    {
        base.pauseGame();
        Time.timeScale = 1;
    }

    protected override void FixedUpdate()
    {
        if (!hasAuthority)
        {
            return;
        }
        base.FixedUpdate();
    }

    protected override void flipSprite()
    {
        if (!hasAuthority)
        {
            return;
        }

        //flip sprite based on player input if they are not wall hugging
        if (stickyWallTimer == 0)
        {
            bool facingRightNow = Input.GetAxisRaw("Horizontal" + controllerID) > 0 || (Input.GetAxisRaw("Horizontal" + controllerID) == 0 && facingRight);
            if (facingRightNow != facingRight)
            {
                CmdFlipSprite(facingRightNow);
                facingRight = facingRightNow;
            }
        }
    }

    protected override void wallJumpFlipSprite()
    {
        if (!hasAuthority || rb2D.velocity.x == 0)
        {
            return;
        }
        bool facingRightNow = rb2D.velocity.x > 0;
        if (facingRightNow != facingRight)
        {
            CmdFlipSprite(facingRightNow);
        }
        facingRight = facingRightNow;
    }

    /**
     * Scripts for flipping the sprite
     */
    [Command]
    void CmdFlipSprite(bool fr)
    {
        facingRight = fr;
    }

    protected override void wallStickFlipSprite()
    {
        //flip sprite away from wall
        bool facingRightNow = currentNormal.x > 0;
        if (facingRightNow != facingRight)
        {
            CmdFlipSprite(facingRightNow);
            facingRight = facingRightNow;
        }
    }

    protected override void attackGloryUpdate(GameObject otherPlayer, int hits, bool trueHit)
    {
        base.attackGloryUpdate(otherPlayer, hits, trueHit);
        CmdGloryUpdate(numGlory, otherPlayer.GetComponent<LocalPlayerScript>().numGlory, otherPlayer);
    }

    protected override void reversalGloryUpdate(GameObject attacker, int hits)
    {
        base.reversalGloryUpdate(attacker, hits);
        CmdGloryUpdate(numGlory, attacker.GetComponent<LocalPlayerScript>().numGlory, attacker);
    }

    [Command]
    void CmdGloryUpdate(float myGlory, float otherGlory, GameObject otherPlayer)
    {
        numGlory = myGlory;
        otherPlayer.GetComponent<LocalPlayerScript>().numGlory = otherGlory;
    }

    protected override void reversalAnimation()
    {
        networkAnimator.SetTrigger("reversaling");
    }

    public override void startKnockback(GameObject defender, Vector2 dir, int hits)
    {
        CmdKnockback(defender, gameObject, dir, hits);
    }

    public override void knockback(GameObject attacker, Vector2 dir, int hits)
    {
        CmdKnockback(gameObject, attacker, dir, hits);
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
        if (hits > 1)
        {
            camera.GetComponent<CameraShake>().shake((1.0f + (hits / 4)) * 0.5f, defender);
        }
        if (defender.tag == "Player")
        {
            defender.GetComponent<NetworkedPlayerScript>().baseKnockback(attacker, dir, hits);
        }
    }
    
    void baseKnockback(GameObject attacker, Vector2 dir, int hits)
    {
        if (!hasAuthority)
        {
            return;
        }
        base.knockback(attacker, dir, hits);
    }

    protected override void hitstunAnimation()
    {
       // networkAnimator.SetTrigger("Hitstun");
        //if (NetworkServer.active)
          //  animator.ResetTrigger("Hitstun");

        CmdHitstun();
    }

    [Command]
    void CmdHitstun()
    {
        RpcHitstun();
    }

    [ClientRpc]
    void RpcHitstun()
    {
        animator.SetTrigger("Hitstun");
    }

    protected override void rotate(Vector3 rotation)
    {
        CmdSyncRotation(rotation);
    }

    /*
    * Script for syncing rotation on server
    */
    [Command]
    void CmdSyncRotation(Vector3 rotation)
    {
        RpcSyncRotation(rotation);
    }

    /*
     * Script for syncing rotation on clients
     */
    [ClientRpc]
    void RpcSyncRotation(Vector3 rotation)
    {
        GetComponent<Transform>().eulerAngles = rotation;
    }

    public override void killPlayer(GameObject go)
    {
        go.GetComponent<LocalPlayerScript>().removeMeter();
        CmdKillPlayer(go);
    }

    protected override void updateComboHits(int hits)
    {
        CmdUpdateComboHits(hits);
    }

    [Command]
    void CmdUpdateComboHits(int hits)
    {
        comboHits = hits;
    }

    protected override void spawnProjectile(Vector3 position, Vector2 velocity, Vector3 rotation)
    {
        CmdSpawnSuperProjectile(position, velocity, rotation);
    }
    

    [Command]
    void CmdSpawnSuperProjectile(Vector3 position, Vector2 velocity, Vector3 rotation)
    {
        base.spawnProjectile(position, velocity, rotation);
        NetworkServer.Spawn(projectile);
    }

    /*
    * Script that tells server to kill player on clients
    */
    [Command]
    void CmdKillPlayer(GameObject player)
    {
        RpcKillPlayer(player);
    }

    /**
     * Script that kills a player on all clients
     */
    [ClientRpc]
    void RpcKillPlayer(GameObject player)
    {
        base.killPlayer(player);
    }
}
