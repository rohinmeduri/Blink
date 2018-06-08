using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraShakeChanger : MonoBehaviour {
    private void Start()
    {
        GetComponent<Slider>().value = GameObject.Find("Main Camera").GetComponent<CameraShake>().getIntensityMultiplier();
    }
    public void changeIntensity(float multiplier)
    {
        GameObject.Find("Main Camera").GetComponent<CameraShake>().changeShakeIntensity(multiplier);
    }
}
