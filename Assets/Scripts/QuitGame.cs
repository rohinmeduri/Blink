using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//script that terminates the application when any of the quit buttons are clicked (NOTE: doesn't work in the Unity editor)
public class QuitGame : MonoBehaviour {

	public void quitGame()
    {
        Application.Quit();
    }
}
