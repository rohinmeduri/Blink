using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour {
    Vector3 originalPosition;
    Quaternion originalRotation;

    float shakeIntensity;
    float shakeDecay;
    float xPosition = 0;
    float yPosition = 0;
    Vector3 difference;
    int followDuration = 0;

    GameObject playerToFollow;

    public const int ZOOM_DURATION = 150;

    private void Start()
    {
        originalPosition = gameObject.GetComponent<Transform>().position;
        originalRotation = gameObject.GetComponent<Transform>().rotation;
    }

    // Update is called once per frame
    void Update () {
		if(followDuration <= ZOOM_DURATION && playerToFollow != null)
        {
            //camera pan
            xPosition = (playerToFollow.GetComponent<Transform>().position - gameObject.GetComponent<Transform>().position).x;
            yPosition = (playerToFollow.GetComponent<Transform>().position - gameObject.GetComponent<Transform>().position).y;
            difference = new Vector3(xPosition, yPosition, 0);
            if (GetComponent<Camera>().orthographicSize > 7)
            {
                GetComponent<Camera>().orthographicSize -= 0.01f;
            }
            gameObject.GetComponent<Transform>().position += difference / 50 + Random.insideUnitSphere * shakeIntensity;
            followDuration++;

            //camera shake
            if (shakeIntensity > 0)
            {
                shakeIntensity -= shakeDecay;
            }
            transform.rotation = new Quaternion(
                       originalRotation.x + Random.Range(-shakeIntensity, shakeIntensity) * .002f,
                       originalRotation.y + Random.Range(-shakeIntensity, shakeIntensity) * .002f,
                       originalRotation.z + Random.Range(-shakeIntensity, shakeIntensity) * .002f,
                       originalRotation.w + Random.Range(-shakeIntensity, shakeIntensity) * .002f);
        }
        else
        {
            reset();
        }
	}

    public void shake (float intensity, GameObject victim)
    {
        xPosition = 0;
        yPosition = 0;
        shakeIntensity = intensity;
        if(shakeIntensity > 1.5f)
        {
            shakeIntensity = 1.5f;
        }
        shakeDecay = intensity * 0.04f;
        playerToFollow = victim;
        followDuration = 0;
    }

    private void reset()
    {
        gameObject.GetComponent<Transform>().position += (originalPosition - gameObject.GetComponent<Transform>().position) / 50;
        gameObject.GetComponent<Transform>().rotation = originalRotation;
        if (GetComponent<Camera>().orthographicSize < 8)
        {
            GetComponent<Camera>().orthographicSize += 0.02f;
        }

    }
}
