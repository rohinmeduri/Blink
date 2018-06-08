using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

//script that syncs behavior/position of superProjectile across the network
public class SuperProjectileNetworked : SuperProjectileScript {
    
    
   public override void rotate(Vector2 direction)
   {
        CmdRotate(direction);
   }

    [Command]
    void CmdRotate(Vector2 direction)
    {
        base.rotate(direction);
        RpcRotate(direction);
    }

    [ClientRpc]
    void RpcRotate(Vector2 direction)
    {
        base.rotate(direction);
    }

    //only detect collisions on one copy of the super projectile so that slightly different positioning does not create problems
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasAuthority)
        {
            base.OnTriggerEnter2D(collision);
        }
    }
}
