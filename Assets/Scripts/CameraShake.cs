using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour {
    Vector3 originalPosition;
    Quaternion originalRotation;

    float shakeIntensity;
    float shakeDecay;

    private void Start()
    {
        originalPosition = gameObject.GetComponent<Transform>().position;
        originalRotation = gameObject.GetComponent<Transform>().rotation;
    }

    // Update is called once per frame
    void Update () {
		if(shakeIntensity > 0)
        {
            gameObject.GetComponent<Transform>().position = originalPosition + Random.insideUnitSphere * shakeIntensity;
            shakeIntensity -= shakeDecay;
            transform.rotation = new Quaternion(
                       originalRotation.x + Random.Range(-shakeIntensity, shakeIntensity) * .002f,
                       originalRotation.y + Random.Range(-shakeIntensity, shakeIntensity) * .002f,
                       originalRotation.z + Random.Range(-shakeIntensity, shakeIntensity) * .002f,
                       originalRotation.w + Random.Range(-shakeIntensity, shakeIntensity) * .002f);
        }
        else
        {
            gameObject.GetComponent<Transform>().position = originalPosition;
            gameObject.GetComponent<Transform>().rotation = originalRotation;
        }
	}

    public void shake (float intensity)
    {
        shakeIntensity = intensity;
        shakeDecay = intensity * 0.04f;
    }
}
