using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsMenuScript : MonoBehaviour {
    private bool show = false;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
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
            //GetComponent<RectTransform>().anchoredPosition = new Vector2(-1600, 0);
            show = false;
        }
        else
        {
            //GetComponent<RectTransform>().anchoredPosition = new Vector2(-260, 0);
            show = true;
        }
    }

    void slideIn()
    {
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
