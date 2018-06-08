using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//script that plays sound effects in battle scenes
public class SoundEffectPlayer : MonoBehaviour {

    private AudioSource[] soundEffectSource;
    private AudioClip[][] soundEffectClip;


    /**
     * Setting sound effects
     * 0: Attack
     * 1: Blink
     * 2: HitSoft
     * 3: HitHard
     * 4: Super
     * 5: SuperPowering
     * 6: Jump/Land
     * 7: Reversal
     * 8: Run
     * 9: Bighit
     * 
     */

    //load all sound effect files
    public virtual void setSoundEffects(string playerType)
    {
        soundEffectSource = new AudioSource[10];
        soundEffectClip = new AudioClip[10][];
        int[] clipSize = {5, 1, 1, 1, 1, 2, 2, 1, 1, 1};
        for (int i = 0; i < soundEffectSource.Length; i++)
        {
            soundEffectSource[i] = gameObject.AddComponent<AudioSource>();
            soundEffectSource[i].playOnAwake = false;
        }
        for(int i = 0; i < soundEffectClip.Length; i++)
        {
            soundEffectClip[i] = new AudioClip[clipSize[i]];
        }
        soundEffectClip[0][0] = Resources.Load("SoundEffects/" + playerType + "/UAttack") as AudioClip;
        soundEffectClip[0][1] = Resources.Load("SoundEffects/" + playerType + "/DUAttack") as AudioClip;
        soundEffectClip[0][2] = Resources.Load("SoundEffects/" + playerType + "/FAttack") as AudioClip;
        soundEffectClip[0][3] = Resources.Load("SoundEffects/" + playerType + "/DDAttack") as AudioClip;
        soundEffectClip[0][4] = Resources.Load("SoundEffects/" + playerType + "/DAttack") as AudioClip;
        soundEffectClip[1][0] = Resources.Load("SoundEffects/" + playerType + "/Blink") as AudioClip;
        soundEffectClip[2][0] = Resources.Load("SoundEffects/" + playerType + "/HitSoft") as AudioClip;
        soundEffectClip[3][0] = Resources.Load("SoundEffects/" + playerType + "/HitHard") as AudioClip;
        soundEffectClip[4][0] = Resources.Load("SoundEffects/" + playerType + "/Super") as AudioClip;
        soundEffectClip[5][0] = Resources.Load("SoundEffects/" + playerType + "/SuperUp") as AudioClip;
        soundEffectClip[5][1] = Resources.Load("SoundEffects/" + playerType + "/SuperDown") as AudioClip;
        soundEffectClip[6][0] = Resources.Load("SoundEffects/" + playerType + "/Jump") as AudioClip;
        soundEffectClip[6][1] = Resources.Load("SoundEffects/" + playerType + "/Land") as AudioClip;
        soundEffectClip[7][0] = Resources.Load("SoundEffects/" + playerType + "/Reversal") as AudioClip;
        soundEffectClip[8][0] = Resources.Load("SoundEffects/" + playerType + "/Run") as AudioClip;
        soundEffectClip[9][0] = Resources.Load("SoundEffects/" + playerType + "/Bighit") as AudioClip;
    }

    //play sound effect given by index and version (version corresponds to player type)
    public virtual void playSoundEffect(int index, int version, float volume)
    {
        soundEffectSource[index].volume = volume;
        soundEffectSource[index].PlayOneShot(soundEffectClip[index][version]);
    }

    //cancel a sound effect
    public virtual void stopSoundEffect(int index)
    {
        soundEffectSource[index].Stop();
    }

}
