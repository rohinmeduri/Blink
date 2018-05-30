using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectPlayer : MonoBehaviour {

    private AudioSource[] soundEffectSource;

	// Use this for initialization
	void Start () {
        
    }

    // Update is called once per frame
    void Update () {
		
	}


    /**
     * Setting sound effects
     * 0: Attack
     * 1: Blink
     * 2: HitSoft
     * 3: HitHard
     * 4: Super
     * 5: Jump
     * 6: Reversal
     * 7: Run
     * 8: Land
     * 9: Splat
     * 
     */
    public virtual void setSoundEffects(string playerType)
    {
        soundEffectSource = new AudioSource[10];
        for (int i = 0; i < soundEffectSource.Length; i++)
        {
            soundEffectSource[i] = gameObject.AddComponent<AudioSource>();
            soundEffectSource[i].playOnAwake = false;
        }
        soundEffectSource[0].clip = Resources.Load("Audio/Effects/" + playerType + "/Attack") as AudioClip;
        soundEffectSource[1].clip = Resources.Load("Audio/Effects/" + playerType + "/Blink") as AudioClip;
        soundEffectSource[2].clip = Resources.Load("Audio/Effects/" + playerType + "/HitSoft") as AudioClip;
        soundEffectSource[3].clip = Resources.Load("Audio/Effects/" + playerType + "/HitHard") as AudioClip;
        soundEffectSource[4].clip = Resources.Load("Audio/Effects/" + playerType + "/Super") as AudioClip;
        soundEffectSource[5].clip = Resources.Load("Audio/Effects/" + playerType + "/Jump") as AudioClip;
        soundEffectSource[6].clip = Resources.Load("Audio/Effects/" + playerType + "/Reversal") as AudioClip;
        soundEffectSource[7].clip = Resources.Load("Audio/Effects/" + playerType + "/Run") as AudioClip;
        soundEffectSource[8].clip = Resources.Load("Audio/Effects/" + playerType + "/Land") as AudioClip;
        soundEffectSource[9].clip = Resources.Load("Audio/Effects/" + playerType + "/Splat") as AudioClip;
    }

    public virtual void playSoundEffect(int index, float volume)
    {
        soundEffectSource[index].volume = volume;
        soundEffectSource[index].Play();
    }

}
