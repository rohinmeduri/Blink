﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MusicPlayerScript : MonoBehaviour {
    public static bool created = false;
    private bool lobbyScene = true;
    public AudioClip menuMusic;
    public AudioClip battleMusic;
    public AudioClip victoryMusic;
              
	// Use this for initialization
	void Start () {
        if (created)
        {
            Destroy(gameObject);
        }
        created = true;
        DontDestroyOnLoad(gameObject);
	}

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        var lobbySceneNow = !(scene.name == "Local Battle Scene" || scene.name == "Multiplayer Battle Scene");
        if(lobbySceneNow != lobbyScene)
        {
            lobbyScene = lobbySceneNow;
            if (lobbyScene)
            {
                GetComponent<AudioSource>().clip = menuMusic;
            }
            else
            {
                GetComponent<AudioSource>().clip = battleMusic;
            }

            GetComponent<AudioSource>().PlayDelayed(0.5f);
        }
    }

    public void queueVictory()
    {
        GetComponent<AudioSource>().Stop();
        Invoke("playVictory", 2);
    }

    private void playVictory()
    {
        GetComponent<AudioSource>().PlayOneShot(victoryMusic);
        /**
         * Uncomment the below code if want to have music return after victory sound
         */

        //Invoke("stopVictoryMusic", 4f);
    }
    /*
    private void stopVictoryMusic()
    {
        GetComponent<AudioSource>().volume = 0;
        GetComponent<AudioSource>().clip = battleMusic;
        GetComponent<AudioSource>().Play();
        afterVictory();
    }

    private void afterVictory()
    {
        if (GetComponent<AudioSource>().volume < 1)
        {
            GetComponent<AudioSource>().volume += Time.deltaTime;
            Debug.Log("victpry");
            Invoke("afterVictory", Time.deltaTime);
        }
    }
    */
}
