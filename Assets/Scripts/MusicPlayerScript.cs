using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MusicPlayerScript : MonoBehaviour {
    public static bool created = false;
    private bool lobbyScene = true;
    public AudioClip menuMusic;
    public AudioClip battleMusic;
              
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
        var lobbySceneNow = (scene.name == "_Main Menu" || scene.name == "Local Lobby Menu" || 
            scene.name == "Multiplayer Lobby" || scene.name == "Tutorial" || scene.name == "Scene and Music Select Menu" || 
            scene.name == "Character Select Menu");
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
}
