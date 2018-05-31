using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class startCounterScript : MonoBehaviour {
    public Sprite two;
    public Sprite one;
    private float timer = 0f;
    private int number = 3;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        timer += Time.deltaTime;
        GetComponent<Image>().rectTransform.sizeDelta = new Vector2(GetComponent<Image>().rectTransform.sizeDelta.x - 2, GetComponent<Image>().rectTransform.sizeDelta.y - 2);
        if(timer >= 1 && number == 3)
        {
            GetComponent<Image>().rectTransform.sizeDelta = new Vector2(300, 300);
            GetComponent<Image>().sprite = two;
            number = 2;
        }
        if (timer >= 2 && number == 2)
        {
            GetComponent<Image>().rectTransform.sizeDelta = new Vector2(300, 300);
            GetComponent<Image>().sprite = one;
            number = 1;
        }
        if (timer >= 4)
        {
            Destroy(gameObject);
        }
    }
}
