using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorSound : MonoBehaviour {

    private bool created = false;
    public AudioClip regularClick;
    public AudioClip backClick;

    // Use this for initialization
    void Start()
    {
        if (created) {
            Destroy(gameObject);
        }
        created = true;
        DontDestroyOnLoad(gameObject);
    }


    // Update is called once per frame
    void Update () {
		
	}

    public void playRegular()
    {
        GetComponent<AudioSource>().PlayOneShot(regularClick);
    }
    public void playBack()
    {
        GetComponent<AudioSource>().PlayOneShot(backClick);
    }
}
