using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeChanger : MonoBehaviour {
    
    private void Start()
    {
        GetComponent<Slider>().value = AudioListener.volume;
    }
    public void changeVolume(float value)
    {
        // GameObject musicPlayer = GameObject.Find("MusicPlayer");
        // musicPlayer.GetComponent<AudioSource>().volume = value;
        AudioListener.volume = value;
    }
}
