using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorSound : MonoBehaviour {

    /**
     * Used to create the clicking sound when the on-screen mouse is clicked
     */

    private bool created = false;
    public AudioClip regularClick;
    public AudioClip backClick;

    // Use this for initialization
    void Start()
    {
        // ensure one instance
        if (created) {
            Destroy(gameObject);
        }
        created = true;
        DontDestroyOnLoad(gameObject);
    }


    // Update is called once per frame
    void Update () {
		
	}

    // regular click sound
    public void playRegular()
    {
        GetComponent<AudioSource>().PlayOneShot(regularClick);
    }
    // back button click sound
    public void playBack()
    {
        GetComponent<AudioSource>().PlayOneShot(backClick);
    }
}
