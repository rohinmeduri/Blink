using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/* Script that takes the local player script and syncs values across a network for online battle
 * NOTE: command function are run on the server, rpc functions are run on clients
 *       As clients cannot communicate directly to one another, much of this script consists of telling the server to
 *       tell the other clients to update some value or perform an action
 */ 

public class NetworkedPlayerScript : LocalPlayerScript
{

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
            foreach (var i in players)
            {
                //initialize local player
                if (i == gameObject)
                {
                    setPlayerType(characterStrings[GetComponent<CharacterDataObject>().getCharacter(0)]);
                    gameObject.layer = 2;
                    GetComponent<NetworkedPlayerScript>().setPlayerID(1);
                }
                //initialize other player objects that were already in the scene and belong to others
                else
                {
                    i.GetComponent<NetworkedPlayerScript>().setPlayerID(IDAssigner.GetComponent<IDAssigner>().getID());

                    //randomly assign player type - characters are functionally equivalent so its is not necessary to sync character selections
                    Random rnd = new Random();
                    int playerType = (Random.Range(0, 3));
                    if (playerType == 0)
                    {
                        i.GetComponent<NetworkedPlayerScript>().setPlayerType("Mage");
                    }
                    else if (playerType == 1)
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

    public override void setPlayerID(int ID)
    {
        //set colors for other players so they are easy to distinguish from you local client player
        if (ID == 1)
        {
            setColor(0);
        }
        else if (ID == 2)
        {
            setColor(1);
        }
        else if (ID == 3)
        {
            setColor(2);
        }
        else
        {
            setColor(3);
        }
        base.setPlayerID(ID);
    }

    //scripts used to sync the starting positions of the players
    public void setPlayerNumber(int number)
    {
        playerNumber = number;
    }

    public void OnPlayerNumber(int num)
    {
        playerNumber = num;
        callSetPlayerPosition(num);
    }

    protected override void setPlayerPosition(int posNum)
    {
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


    //this function is called on all other clients when a new player is created so that it can be initialized properly
    public void newPlayer()
    {
        if (!hasAuthority && (getPlayerID() == 0))
        {
            IDAssigner = GameObject.Find("ID Assigner");
            GetComponent<NetworkedPlayerScript>().setPlayerID(IDAssigner.GetComponent<IDAssigner>().getID());

            //once again, randomize the player type (see above)
            Random rnd = new Random();
            int playerType = (Random.Range(0, 3));
            if (playerType == 0)
            {
                GetComponent<NetworkedPlayerScript>().setPlayerType("Mage");
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

    //sync glory changes after attacks
    protected override void attackGloryUpdate(GameObject otherPlayer, int hits, bool trueHit)
    {
        base.attackGloryUpdate(otherPlayer, hits, trueHit);
        CmdGloryUpdate(numGlory, otherPlayer.GetComponent<LocalPlayerScript>().numGlory, otherPlayer);
    }

    //sync flory changes after reversals
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

    //creates blink animations on all cients
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

    //trigger other animations across all clients
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

    //functions that create super projectiles on all cients
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

    //tells all clients to 'activate' the super projectile
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

    //syncs sound effects on all clients
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

    //functions that sync visual effects on all clients
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

    //tells all clients to create the 'super glow' on this player
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

    //functions that sync player stats across the network
    [Command]
    void CmdUpdateStats(int mc, int hn, int hp, int k)
    {
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
