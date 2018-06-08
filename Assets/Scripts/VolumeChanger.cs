using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//script that handles input from the volume sliders to change the volume of the application
public class VolumeChanger : MonoBehaviour {
    
    private void Start()
    {
        GetComponent<Slider>().value = AudioListener.volume;
    }
    public void changeVolume(float value)
    {
        AudioListener.volume = value;
    }
}
