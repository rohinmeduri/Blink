using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//script that hides/shows the options menu in the main menu
public class OptionsMenuScript : MonoBehaviour {
    private bool show = false;
	
	// slide in/out the options menu
	void Update () {
        if (show)
        {
            slideIn();
        }
        else
        {
            slideOut();
        }
	}

    public void toggle()
    {
        if (show)
        {
            show = false;
        }
        else
        {
            show = true;
        }
    }

    void slideIn()
    {
        //slide in until it reaches final position
        if(GetComponent<RectTransform>().anchoredPosition.x < -260)
        {
            GetComponent<RectTransform>().anchoredPosition = new Vector2(GetComponent<RectTransform>().anchoredPosition.x + 52, 0);
            if(GetComponent<RectTransform>().anchoredPosition.x > -260)
            {
                GetComponent<RectTransform>().anchoredPosition = new Vector2(-260, 0);
            }
            
        }
    }

    void slideOut()
    {
        //slide out until it reaches final position
        if (GetComponent<RectTransform>().anchoredPosition.x > -1600)
        {
            GetComponent<RectTransform>().anchoredPosition = new Vector2(GetComponent<RectTransform>().anchoredPosition.x -160, 0);
            if (GetComponent<RectTransform>().anchoredPosition.x < -1600)
            {
                GetComponent<RectTransform>().anchoredPosition = new Vector2(-1600, 0);
            }
        }
    }
}
