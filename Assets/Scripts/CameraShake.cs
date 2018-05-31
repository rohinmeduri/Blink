using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour {
    Vector3 originalPosition;
    Quaternion originalRotation;

    float shakeIntensity;
    float shakeDecay;
    float xChange = 0;
    float yChange = 0;
    Vector3 difference;
    int followDuration = 0;

    GameObject playerToFollow;

    public const int ZOOM_DURATION = 150;
    public const float CAMERA_MAX_DISPLACEMENT_X = 2.1f;
    public const float CAMERA_MAX_DISPLACEMENT_Y = 1.5f;
    

    private void Start()
    {
        originalPosition = gameObject.GetComponent<Transform>().position;
        originalRotation = gameObject.GetComponent<Transform>().rotation;
    }

    // Update is called once per frame
    void Update () {
		if(followDuration <= ZOOM_DURATION && playerToFollow != null)
        {
            float cameraCurrentX = gameObject.GetComponent<Transform>().position.x;
            float cameraCurrentY = gameObject.GetComponent<Transform>().position.y;
            //camera pan
            xChange = playerToFollow.GetComponent<Transform>().position.x - cameraCurrentX;
            yChange = playerToFollow.GetComponent<Transform>().position.y - cameraCurrentY;

            //make sure camera isn't moving too far
            if (xChange + cameraCurrentX > CAMERA_MAX_DISPLACEMENT_X)
            {
                xChange = CAMERA_MAX_DISPLACEMENT_X - cameraCurrentX;
            }
            else if (xChange + cameraCurrentX < -1 * CAMERA_MAX_DISPLACEMENT_X)
            {
                xChange = -1 * CAMERA_MAX_DISPLACEMENT_X - cameraCurrentX;
            }
            if (yChange + cameraCurrentY > CAMERA_MAX_DISPLACEMENT_Y)
            {
                yChange = CAMERA_MAX_DISPLACEMENT_Y - cameraCurrentY;
            }
            else if (yChange + cameraCurrentY < -1 * CAMERA_MAX_DISPLACEMENT_Y)
            {
                yChange = -1 * CAMERA_MAX_DISPLACEMENT_Y - cameraCurrentY;
            }

            difference = new Vector3(xChange, yChange, 0);


            //zoom in - max zoom is 7
            /*if (GetComponent<Camera>().orthographicSize > 4.25)
            {
                GetComponent<Camera>().orthographicSize -= 0.01f;
            }*/

            //shake the camera by moving it randomly in the vicinity of its position
            Vector2 shakeFactor = Random.insideUnitCircle * shakeIntensity;
            Vector3 shakeVector = new Vector3(shakeFactor.x, shakeFactor.y, 0);

            //move camera according to calculated values;
            gameObject.GetComponent<Transform>().position += difference / 50 + shakeVector;
            followDuration++;

            //reduce intensity of shake (decrease radius of randomnesss)
            if (shakeIntensity > 0)
            {
                shakeIntensity -= shakeDecay;
            }

            //rotate camera slightly
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
        xChange = 0;
        yChange = 0;
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
        if (GetComponent<Camera>().orthographicSize < 5.25)
        {
            GetComponent<Camera>().orthographicSize += 0.02f;
        }

    }
}
