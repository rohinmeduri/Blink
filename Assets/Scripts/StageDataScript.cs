using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageDataScript : MonoBehaviour {

    public static bool created = false;
    public Sprite stage;
    // Use this for initialization

    void Start()
    {
        if (created)
        {
            Destroy(gameObject);
        }
        created = true;
        DontDestroyOnLoad(gameObject);
    }
}
