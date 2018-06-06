using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShakeChanger : MonoBehaviour {
    public void changeIntensity(float multiplier)
    {
        GameObject.Find("Main Camera").GetComponent<CameraShake>().changeShakeIntensity(multiplier);
    }
}
