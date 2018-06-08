using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//this script is used by the camera shake option (a slider) in the pause menus and options menu - it changes the amount the camera shakes
public class CameraShakeChanger : MonoBehaviour {
    private void Start()
    {
        //sync value for each instance of the camera slider with the intensity that has already been set
        GetComponent<Slider>().value = GameObject.Find("Main Camera").GetComponent<CameraShake>().getIntensityMultiplier();
    }

    //this function is called when the slider's value is changed
    public void changeIntensity(float multiplier)
    {
        GameObject.Find("Main Camera").GetComponent<CameraShake>().changeShakeIntensity(multiplier);
    }
}
