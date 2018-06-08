using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundFXOptionScript : MonoBehaviour {
    public static bool soundFX = true;
    public Sprite onImage;
    public Sprite offImage;
    public int frameCounter = 0;
	// Use this for initialization
	void Start () {
    }
	
	// Update is called once per frame
	void Update () {
        frameCounter++;
        if(frameCounter == 2)
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            GameObject[] playerAIs = GameObject.FindGameObjectsWithTag("PlayerAI");

            if (!soundFX)
            {
                foreach (GameObject player in players)
                {
                    player.GetComponent<LocalPlayerScript>().soundEffectsVolume = 0f;
                }
                foreach (GameObject ai in playerAIs)
                {
                    ai.GetComponent<LocalPlayerScript>().soundEffectsVolume = 0f;
                }
                GetComponent<Image>().sprite = offImage;
                soundFX = false;
            }
            else
            {
                foreach (GameObject player in players)
                {
                    player.GetComponent<LocalPlayerScript>().soundEffectsVolume = 1f;
                }
                foreach (GameObject ai in playerAIs)
                {
                    ai.GetComponent<LocalPlayerScript>().soundEffectsVolume = 1f;
                }
                GetComponent<Image>().sprite = onImage;
                soundFX = true;
            }
        }
	}

    public void toggleSoundFX()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        GameObject[] playerAIs = GameObject.FindGameObjectsWithTag("PlayerAI");

        if (soundFX)
        {
            foreach(GameObject player in players)
            {
                player.GetComponent<LocalPlayerScript>().soundEffectsVolume = 0f;
            }
            foreach (GameObject ai in playerAIs)
            {
                ai.GetComponent<LocalPlayerScript>().soundEffectsVolume = 0f;
            }
            GetComponent<Image>().sprite = offImage;
            soundFX = false;
        }
        else
        {
            foreach (GameObject player in players)
            {
                player.GetComponent<LocalPlayerScript>().soundEffectsVolume = 1f;
            }
            foreach (GameObject ai in playerAIs)
            {
                ai.GetComponent<LocalPlayerScript>().soundEffectsVolume = 1f;
            }
            GetComponent<Image>().sprite = onImage;
            soundFX = true;
        }
    }
}
