using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SuperProjectileNetworked : SuperProjectileScript {

    /*protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasAuthority)
        {
            base.OnCollisionEnter2D(collision);
        }
        else
        {
            Debug.Log("no have authority");
        }
    }*/
    
   public override void rotate(Vector2 direction)
   {
        Debug.Log("rotate function called");
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
        Debug.Log("rotate");
    }
}
