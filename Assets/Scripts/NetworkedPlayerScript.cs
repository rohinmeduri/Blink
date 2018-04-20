using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkedPlayerScript : LocalPlayerScript {

    private NetworkAnimator networkAnimator;
    private int gloryWaitFrames = 2;
    private int gloryWaitedFrames = 0;

    // Use this for initialization
    protected override void Start () {
        base.Start();
        networkAnimator = GetComponent<NetworkAnimator>();
    }

    public void setLayer()
    {
        if (hasAuthority)
        {
            setPlayerID(1);
            gameObject.layer = 2;
        }
        else
        {
            setPlayerID(2);
        }
    }

    /*public override void createMeter()
    {
        if (gloryWaitedFrames == gloryWaitFrames)
        {
            base.createMeter();
            RectTransform gloryTransform = glory.GetComponent<RectTransform>();
            if (!hasAuthority)
            {
                gloryTransform.anchorMin = new Vector2(1, 1);
                gloryTransform.anchorMax = new Vector2(1, 1);
                gloryTransform.pivot = new Vector2(1, 1);
                gloryTransform.anchoredPosition = new Vector3(-100, 0, 0);
            }
        }
    }*/

    /*public override void removeMeter()
    {
        CmdRemoveMeter();
    }

    [Command]
    public void CmdRemoveMeter()
    {
        RpcRemoveMeter();
    }

    [ClientRpc]
    public void RpcRemoveMeter()
    {
        Debug.Log("remove meter called");
        base.removeMeter();
    }*/

    // Update is called once per frame
    protected override void Update()
    {
        //flips sprite if necessary (on all clients)
        gameObject.GetComponent<SpriteRenderer>().flipX = !facingRight;

        gloryWaitedFrames++;
        if (gloryWaitedFrames == gloryWaitFrames)
        {
            createMeter();
        }

        if (!hasAuthority)
        {
            return;
        }

        base.Update();
    }

    //no pausing in online multiplayer
    protected override void pauseGame()
    {

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

        if (NetworkServer.active)
            animator.ResetTrigger("reversaling");
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
        networkAnimator.SetTrigger("Hitstun");
        if (NetworkServer.active)
            animator.ResetTrigger("Hitstun");
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

    protected override void killPlayer(GameObject go)
    {
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

    /**
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
