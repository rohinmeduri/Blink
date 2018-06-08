using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

//script that controls the behavior of super projectiles
public class SuperProjectileScript : NetworkBehaviour {
    private GameObject sender;
    private bool active = false;
    public RuntimeAnimatorController mageController;
    public RuntimeAnimatorController saidonController;
    public RuntimeAnimatorController rebelController;

    //tell the super projectile who sent it
    public void setSender(GameObject s)
    {
        sender = s;
    }

    //detect collisions and kill players
    protected virtual void OnTriggerEnter2D(Collider2D collider)
    {
        if (active)
        {
            if (collider.gameObject.tag != "Player" && collider.gameObject.tag != "PlayerAI")
            {
                StartCoroutine(DestroyProjectile(1));
            }
            else
            {
                if(collider.gameObject != sender)
                {
                    sender.GetComponent<LocalPlayerScript>().killPlayer(collider.gameObject);
                }
            }
        }
    }

    //destroy projectile after a certain amount of time
    IEnumerator DestroyProjectile(float time)
    {
        yield return new WaitForSeconds(time);

        Destroy(gameObject);
    }

    //fire the projectile
    public void activate(Vector2 direction)
    {
        active = true;
        rotate(direction);
        GetComponent<Rigidbody2D>().velocity = direction * 25;
        GetComponent<CircleCollider2D>().enabled = true;
    }

    //rotate the projectile
    public virtual void rotate(Vector2 direction)
    {
        GetComponent<Transform>().eulerAngles = new Vector3(0, 0, Mathf.Rad2Deg * Mathf.Atan2(direction.y, direction.x) - 90f);
    }

    //set the type of projectile (corresponds to the player type who sent it)
    public void setType(string type)
    {
        if (type.Equals("Mage")){
            GetComponent<Animator>().runtimeAnimatorController = mageController;
        }
        else if (type.Equals("Rebel")){
            GetComponent<Animator>().runtimeAnimatorController = rebelController;
        }
        else
        {
            GetComponent<Animator>().runtimeAnimatorController = saidonController;
        }

    }
}


