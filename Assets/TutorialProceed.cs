using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using System.Text.RegularExpressions;

public class TutorialProceed : MonoBehaviour {

    private string[] tutorialText;
    public Text displayedText;
    
    void Start()
    {

        tutorialText = read();

        for (int i = 0; i < tutorialText.Length; i++)
        {
            Debug.Log(tutorialText[i]);
        }
    }
    void MoveText(int i) {

	}

    string[] read()
    { //reads text file and organizes each line into a string array

        FileStream fStream = new FileStream("Assets/Resources/TutorialText.txt", FileMode.Open);
        string instructions = "";
        using (StreamReader fReader = new StreamReader(fStream, true))
        {
            instructions = fReader.ReadToEnd();
        }

        string[] result = Regex.Split(instructions, "\r\n?|\n", RegexOptions.Singleline); //checks for "enter" in the text file, and splits string accordingly

        return result;
    }
}
