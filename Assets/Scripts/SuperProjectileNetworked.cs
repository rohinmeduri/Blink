using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SuperProjectileNetworked : SuperProjectileScript {
    
    
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasAuthority)
        {
            base.OnTriggerEnter2D(collision);
        }
    }
}
