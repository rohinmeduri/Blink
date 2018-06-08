using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicSelectionScript : MonoBehaviour {

    //function that handles button input from stage select screen to call the appropriate function to change music
    public void chooseMusic(int index)
    {
        GameObject.Find("MusicPlayer").GetComponent<MusicPlayerScript>().chooseMusic(index);
    }
}
