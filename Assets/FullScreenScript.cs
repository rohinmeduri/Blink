using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FullScreenScript : MonoBehaviour {

    // Use this for initialization
    void Start()
    {
        gameObject.GetComponent<Image>().rectTransform.sizeDelta = new Vector2(Screen.width / GameObject.Find("Canvas").GetComponent<Canvas>().scaleFactor, Screen.height / GameObject.Find("Canvas").GetComponent<Canvas>().scaleFactor);
    }
}
