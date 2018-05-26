using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeChanger : MonoBehaviour {

    public void changeVolume(float value)
    {
        GameObject musicPlayer = GameObject.FindGameObjectWithTag("Music Player");
        musicPlayer.GetComponent<AudioSource>().volume = value;
    }
}
