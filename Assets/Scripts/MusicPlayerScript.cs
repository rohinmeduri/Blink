using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//script that plays music based on what sccene the player is on
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

        //make sure music player persists between scenes so music doesn't stop
        DontDestroyOnLoad(gameObject);
        audioSource = GameObject.Find("MusicPlayer").GetComponent<AudioSource>();
	}

    //keep track of which scene the player is on
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    //check what music to play when the scene is changed
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        var lobbySceneNow = !(scene.name == "Local Battle Scene" || scene.name == "Multiplayer Battle Scene");
        if(lobbySceneNow != lobbyScene)
        {
            lobbyScene = lobbySceneNow;
            if (lobbyScene)
            {
                audioSource.clip = menuMusic;
            }

            audioSource.PlayDelayed(0.5f);
        }
    }

    //function that allows user to pick the music that plays from the stage select screen
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

    //function that prepares to play the victory sound by stopping other music
    public void queueVictory()
    {
        audioSource.Stop();
        Invoke("playVictory", 2);
    }

    //function that plays victory sound when a player wins
    private void playVictory()
    {
        audioSource.PlayOneShot(victoryMusic);
    }
}
