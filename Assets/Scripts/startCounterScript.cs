using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//script that creates the visual countdown effect
public class startCounterScript : MonoBehaviour {
    public Sprite fight;
    public Sprite two;
    public Sprite one;
    private float timer = 0f;
    private int number = 3;
	
	//switch the number every second as well as resize as time progresses
	void Update () {
        timer += Time.deltaTime;

        if (number > 0)
        {
            GetComponent<Image>().rectTransform.sizeDelta = new Vector2(GetComponent<Image>().rectTransform.sizeDelta.x - 2, GetComponent<Image>().rectTransform.sizeDelta.y - 2);
        }
        else if(timer < 3.1f)
        {
            GetComponent<Image>().rectTransform.sizeDelta = new Vector2(GetComponent<Image>().rectTransform.sizeDelta.x + 60, GetComponent<Image>().rectTransform.sizeDelta.y + 60);
        }
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
        if(timer >= 3 && number == 1)
        {
            GetComponent<Image>().rectTransform.sizeDelta = new Vector2(500, 300);
            GetComponent<Image>().sprite = fight;
            number = 0;
        }
        if (timer >= 4)
        {
            Destroy(gameObject);
        }
    }
}
