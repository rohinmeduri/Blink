using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MusicPlayerScript : MonoBehaviour {
    public static bool created = false;
    private bool lobbyScene = true;
    public AudioSource audioSource;

    public AudioClip menuMusic;
    public AudioClip battleMusic;
    public AudioClip operaticMusic;
    public AudioClip victoryMusic;
              
	// Use this for initialization
	void Start () {
        if (created)
        {
            Destroy(gameObject);
        }
        created = true;
        DontDestroyOnLoad(gameObject);
        audioSource = GameObject.Find("MusicPlayer").GetComponent<AudioSource>();
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
                audioSource.clip = menuMusic;
            }/*
            else
            {
                audioSource.clip = battleMusic;
            }*/

            audioSource.PlayDelayed(0.5f);
        }
    }

    public void chooseMusic(int index)
    {
        switch (index)
        {
            case 0:
                audioSource.clip = menuMusic;
                break;
            case 1:
                audioSource.clip = battleMusic;
                break;
            case 2:
                audioSource.clip = operaticMusic;
                break;
            default:
                break;
        }

        audioSource.PlayDelayed(0.5f);
    }

    public void queueVictory()
    {
        audioSource.Stop();
        Invoke("playVictory", 2);
    }

    private void playVictory()
    {
        audioSource.PlayOneShot(victoryMusic);
        /**
         * Uncomment the below code if want to have music return after victory sound
         */

        //Invoke("stopVictoryMusic", 4f);
    }
    /*
    private void stopVictoryMusic()
    {
        audioSource.volume = 0;
        audioSource.clip = battleMusic;
        audioSource.Play();
        afterVictory();
    }

    private void afterVictory()
    {
        if (audioSource.volume < 1)
        {
            audioSource.volume += Time.deltaTime;
            Debug.Log("victpry");
            Invoke("afterVictory", Time.deltaTime);
        }
    }
    */
}
