using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkedPlayerScript : LocalPlayerScript {

    private NetworkAnimator networkAnimator;
    private GameObject IDAssigner;
    private GameObject dataTracker;
    [SyncVar(hook = "OnPlayerNumber")]
    private int playerNumber;
    private string[] characterStrings = { "Mage", "Rebel", "Saidon" };

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
                    setPlayerType(characterStrings[GetComponent<CharacterDataObject>().getCharacter(0)]);
                    gameObject.layer = 2;
                }
                else
                {
                    i.GetComponent<NetworkedPlayerScript>().setPlayerID(IDAssigner.GetComponent<IDAssigner>().getID());
                    Random rnd = new Random();
                    int playerType = (Random.Range(0,3));
                    if (playerType == 0)
                    {
                        i.GetComponent<NetworkedPlayerScript>().setPlayerType("Saidon");
                    }
                    else if(playerType == 1)
                    {
                        i.GetComponent<NetworkedPlayerScript>().setPlayerType("Rebel");
                    }
                    else
                    {
                        i.GetComponent<NetworkedPlayerScript>().setPlayerType("Saidon");
                    }
                }
            }
            CmdNewPlayer();
        }
    }

    public void setPlayerNumber(int number)
    {
        playerNumber = number;
    }

    public void OnPlayerNumber(int num)
    {
        playerNumber = num;
        callSetPlayerPosition(num); 
    }

    protected override void setPlayerPosition(int posNum){
        return;
    }

    void callSetPlayerPosition(int num)
    {
        base.setPlayerPosition(num);
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
        if (!hasAuthority && (getPlayerID() == 0))
        {
            IDAssigner = GameObject.Find("ID Assigner");
            setPlayerID(IDAssigner.GetComponent<IDAssigner>().getID());
            Random rnd = new Random();
            int playerType = (Random.Range(0, 3));
            if (playerType == 0)
            {
                GetComponent<NetworkedPlayerScript>().setPlayerType("Saidon");
            }
            else if (playerType == 1)
            {
                GetComponent<NetworkedPlayerScript>().setPlayerType("Rebel");
            }
            else
            {
                GetComponent<NetworkedPlayerScript>().setPlayerType("Saidon");
            }
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
        bool facingRightNow = currentNormal.x < 0;
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

    protected override void blinkOutAnimation()
    {
        networkAnimator.SetTrigger("blinking");
    }

    protected override void blinkInAnimation()
    {
        base.blinkInAnimation();
        CmdBlinkInAnimation();
    }

    [Command]
    public void CmdBlinkInAnimation()
    {
        RpcBlinkInAnimation();
    }

    [ClientRpc]
    public void RpcBlinkInAnimation()
    {
        if (!hasAuthority)
        {
            base.blinkInAnimation();
        }
    }

    protected override void reversalAnimation()
    {
        networkAnimator.SetTrigger("reversaling");
    }

    protected override void reversalLandedAnimation()
    {
        networkAnimator.SetTrigger("reversalLanded");
    }

    protected override void superAnimation()
    {
        networkAnimator.SetTrigger("super");
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
       networkAnimator.SetTrigger("Hitstun");
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

    protected override void updateComboHits(int hits)
    {
        CmdUpdateComboHits(hits);
    }

    [Command]
    void CmdUpdateComboHits(int hits)
    {
        comboHits = hits;
    }

    protected override void spawnSuper()
    {
        CmdSpawnSuperProjectile();
    }
    

    [Command]
    void CmdSpawnSuperProjectile()
    {
        base.spawnSuper();
        NetworkServer.SpawnWithClientAuthority(projectile, gameObject);
        RpcAssignProjectile(projectile);
    }

    [ClientRpc]
    void RpcAssignProjectile(GameObject p)
    {
        projectile = p;
    }

    protected override void activateProjectile(Vector2 direction)
    {
        CmdActivateProjectile(direction);
    }

    [Command]
    void CmdActivateProjectile(Vector2 direction)
    {
        RpcActivateProjectile(direction);
    }

    [ClientRpc]
    void RpcActivateProjectile(Vector2 direction)
    {
        base.activateProjectile(direction);
    }

    protected override void createSoundEffect(int index, int version, float volume)
    {
        CmdCreateSoundEffect(index, version, volume);
    }

    [Command]
    public void CmdCreateSoundEffect(int index, int version, float volume)
    {
        RpcCreateSoundEffect(index, version, volume);
    }

    [ClientRpc]
    public void RpcCreateSoundEffect(int index, int version, float volume)
    {
        base.createSoundEffect(index, version, volume);
    }

    protected override void stopSoundEffect(int index)
    {
        CmdStopSoundEffect(index);
    }

    [Command]
    public void CmdStopSoundEffect(int index)
    {
        RpcStopSoundEffect(index);
    }

    [ClientRpc]
    public void RpcStopSoundEffect(int index)
    {
        base.stopSoundEffect(index);
    }

    protected override void createVisualEffect(int index)
    {
        CmdCreateVisualEffect(index);
    }

    [Command]
    public void CmdCreateVisualEffect(int index)
    {
        RpcCreateVisualEffect(index);
    }

    [ClientRpc]
    public void RpcCreateVisualEffect(int index)
    {
        base.createVisualEffect(index);
    }

    protected override void superEffect(bool active)
    {
        CmdSuperEffect(active);
    }

    [Command]
    public void CmdSuperEffect(bool active)
    {
        RpcSuperEffect(active);
    }

    [ClientRpc]
    public void RpcSuperEffect(bool active)
    {
        base.superEffect(active);
    }

    public bool getHasAuthority()
    {
        return hasAuthority;
    }

    public override void killPlayer(GameObject go)
    {
        CmdKillPlayer(go);
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
        if (hasAuthority)
        {
            kills++;
        }
        player.GetComponent<NetworkedPlayerScript>().playerDeath();
    }

    void playerDeath()
    {
        if (hasAuthority)
        {
            compileData();
        }
    }

    [Command]
    void CmdUpdateStats(int mc, int hn, int hp, int k) { 
        RpcReplaceStats(mc, hn, hp, k);
    }

    [ClientRpc]
    void RpcReplaceStats(int mc, int hn, int hp, int k)
    {
        GameObject dataManager = GameObject.FindGameObjectWithTag("DataTracker");
        dataManager.GetComponent<LocalDataTracker>().replaceStats(mc, hn, hp, k, gameObject);
    }

    public override int[] compileData()
    {
        if (hasAuthority)
        {
            CmdUpdateStats(maxCombo, hitNumber, getHitPercentage(), kills);
        }
        return null;
    }
}
