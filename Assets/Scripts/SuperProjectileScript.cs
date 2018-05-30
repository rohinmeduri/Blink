using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SuperProjectileScript : NetworkBehaviour {
    private GameObject sender;
    private bool active = false;
    public Sprite chargingSuper;
    public Sprite activatedSuper;

    public void setSender(GameObject s)
    {
        sender = s;
    }

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
                sender.GetComponent<LocalPlayerScript>().killPlayer(collider.gameObject);
            }
        }
    }

    IEnumerator DestroyProjectile(float time)
    {
        yield return new WaitForSeconds(time);

        Destroy(gameObject);
    }

    public void activate(Vector2 direction)
    {
        active = true;
        rotate(direction);
        GetComponent<Rigidbody2D>().velocity = direction * 25;
        GetComponent<BoxCollider2D>().enabled = true;
        GetComponent<CircleCollider2D>().enabled = true;
    }

    public virtual void rotate(Vector2 direction)
    {
        GetComponent<Transform>().eulerAngles = new Vector3(0, 0, Mathf.Rad2Deg * Mathf.Atan2(direction.y, direction.x) - 90f);
    }
}


