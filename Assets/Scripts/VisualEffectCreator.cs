using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//script that creates visual effects
public class VisualEffectCreator : MonoBehaviour {

    private Animator effectsAnimator;
    private int index;

    public void setVisualEffects(string playerType, int index)
    {
        this.index = index;
        effectsAnimator = gameObject.AddComponent<Animator>() as Animator;
        effectsAnimator.runtimeAnimatorController = Resources.Load("VisualEffects/" + playerType + "/EffectsAnimator" + index) as RuntimeAnimatorController;
    }

    /**
     * Setting Visual Effects
     * 0: Ground Jump
     * 1: Air Jump
     * 2: Wall Jump
     * 3: Land
     * 4: Dash Burst
     * 5: Attack
     */
     //script that creates the effect and assigns relevant values based on the effect type
    public void triggerEffect(GameObject player)
    {
        rotate(new Vector2(1, 0));
        gameObject.GetComponent<SpriteRenderer>().flipX = true;
        switch (index)
        {
            case 0:
                gameObject.transform.position = player.transform.position + new Vector3(0, -0.8f, 0);
                break;
            case 1:
                gameObject.transform.position = player.transform.position + new Vector3(0, -0.8f, 0);
                break;
            case 2:
                gameObject.GetComponent<SpriteRenderer>().flipX = player.GetComponent<SpriteRenderer>().flipX;
                if (gameObject.GetComponent<SpriteRenderer>().flipX)
                    gameObject.transform.position = player.transform.position + new Vector3(-0.35f, 0, 0);
                else
                    gameObject.transform.position = player.transform.position + new Vector3(0.35f, 0, 0);
                break;
            case 3:
                gameObject.transform.position = player.transform.position + new Vector3(0, -0.8f, 0);
                break;
            case 4:
                gameObject.GetComponent<SpriteRenderer>().flipX = player.GetComponent<Rigidbody2D>().velocity.x < 0;
                if (gameObject.GetComponent<SpriteRenderer>().flipX)
                    gameObject.transform.position = player.transform.position + new Vector3(-0.5f, -0.5f, 0);
                else
                    gameObject.transform.position = player.transform.position + new Vector3(0.5f, -0.5f, 0);
                break;
            case 5:
                Vector2 direction = player.GetComponent<LocalPlayerScript>().getDirection();
                rotate(-direction);
                gameObject.transform.position = player.transform.position + new Vector3(direction.x, direction.y, 0);
                break;
            default:
                break;
        }
        effectsAnimator.SetTrigger("Trigger");
    }

    //rotates effect to match the players that created them
    public virtual void rotate(Vector2 direction)
    {
        GetComponent<Transform>().eulerAngles = new Vector3(0, 0, Mathf.Rad2Deg * Mathf.Atan2(direction.y, direction.x));
    }
}
