using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualEffectCreator : MonoBehaviour {

    private Animator effectsAnimator;
    private int index;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void setVisualEffects(string playerType, int index)
    {
        this.index = index;
        effectsAnimator = gameObject.AddComponent<Animator>() as Animator;
        effectsAnimator.runtimeAnimatorController = Resources.Load("Animations/VisualEffects/" + playerType + "/EffectsAnimator" + index) as RuntimeAnimatorController;
    }

    /**
     * Setting Visual Effects
     * 0: Ground Jump
     * 1: Air Jump
     * 2: Wall Jump
     * 3: Land
     * 4: Dash Burst
     * 5: 
     */
    public void triggerEffect(GameObject player)
    {
        gameObject.GetComponent<SpriteRenderer>().flipX = true;
        switch (index)
        {
            case 0:
                gameObject.transform.position = player.transform.position + new Vector3(0, -0.8f, 0);
                effectsAnimator.SetTrigger("Jump");
                break;
            case 1:
                gameObject.transform.position = player.transform.position + new Vector3(0, -0.8f, 0);
                effectsAnimator.SetTrigger("AirJump");
                break;
            case 2:
                gameObject.GetComponent<SpriteRenderer>().flipX = player.GetComponent<SpriteRenderer>().flipX;
                if (gameObject.GetComponent<SpriteRenderer>().flipX)
                    gameObject.transform.position = player.transform.position + new Vector3(-0.35f, 0, 0);
                else
                    gameObject.transform.position = player.transform.position + new Vector3(0.35f, 0, 0);
                effectsAnimator.SetTrigger("WallJump");
                break;
            case 3:
                break;
            case 4:
                gameObject.GetComponent<SpriteRenderer>().flipX = player.GetComponent<Rigidbody2D>().velocity.x < 0;
                if (gameObject.GetComponent<SpriteRenderer>().flipX)
                    gameObject.transform.position = player.transform.position + new Vector3(-0.5f, -0.5f, 0);
                else
                    gameObject.transform.position = player.transform.position + new Vector3(0.5f, -0.5f, 0);
                effectsAnimator.SetTrigger("DashBurst");
                break;
            default:
                break;
        }
    }
}
