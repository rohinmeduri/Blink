using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CursorScript : MonoBehaviour {
    private float CURSOR_SPEED = Screen.height / 50;
    public GameObject Canvas;

    private bool visible = false;
    private float disableTime = 2f;
    private float inactiveCounter;


    private void Start()
    {
        inactiveCounter = 0f;
    }

    // Update is called once per frame
    void Update () {
        
        //move cursor
        var xChange = Input.GetAxis("Horizontal1");
        var yChange = Input.GetAxis("Vertical1");

        if(xChange == 0 && yChange == 0 && !Input.GetButtonUp("Submit1"))
        {
            inactiveCounter += Time.deltaTime;
            if (inactiveCounter >= disableTime)
            {
                visible = false;
            }
        }
        else
        {
            inactiveCounter = 0;
            visible = true;
        }

        if(SceneManager.GetActiveScene().name == "Multiplayer Battle Scene")
        {
            visible = false;
        }

        GetComponent<Image>().enabled = visible;

        var change = new Vector3(xChange, yChange, 0);

        GetComponent<Transform>().position = GetComponent<Transform>().position + change * CURSOR_SPEED;
        if(GetComponent<Transform>().position.x > Screen.width)
        {
            GetComponent<Transform>().position = new Vector3(Screen.width, GetComponent<Transform>().position.y, 0);
        }
        else if(GetComponent<Transform>().position.x < 0)
        {
            GetComponent<Transform>().position = new Vector3(0, GetComponent<Transform>().position.y, 0);
        }
        if (GetComponent<Transform>().position.y < 0)
        {
            GetComponent<Transform>().position = new Vector3(GetComponent<Transform>().position.x, 0, 0);
        }
        else if (GetComponent<Transform>().position.y > Screen.height)
        {
            GetComponent<Transform>().position = new Vector3(GetComponent<Transform>().position.x, Screen.height, 0);
        }

        //detect clicks
        if (visible && Input.GetButtonUp("Submit1"))
        {
            Vector3 clickPoint = transform.position;
            //Vector3 clickPoint = GetComponent<RectTransform>().anchoredPosition3D;
            Debug.Log(clickPoint.x + ", " + clickPoint.y + ", " + clickPoint.z);

            Transform[] children = Canvas.GetComponentsInChildren<Transform>();
            foreach (Transform child in children)
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(child.GetComponent<RectTransform>(), clickPoint) 
                    && (child.GetComponent<Button>() != null) 
                    && child.GetComponent<Button>().interactable == true)
                {
                    if (child.GetComponent<Button>().tag.Equals("Back"))
                    {
                        GameObject.Find("CursorSound").GetComponent<CursorSound>().playBack();
                    }
                    else
                    {
                        GameObject.Find("CursorSound").GetComponent<CursorSound>().playRegular();
                    }
                    child.GetComponent<Button>().onClick.Invoke();
                }
            }
        }
    }
}
