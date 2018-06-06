using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeChanger : MonoBehaviour {

    public void changeVolume(float value)
    {
        // GameObject musicPlayer = GameObject.Find("MusicPlayer");
        // musicPlayer.GetComponent<AudioSource>().volume = value;
        AudioListener.volume = value;
    }
}
