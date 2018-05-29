using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IDAssigner : MonoBehaviour {

    private int IDCounter = 1;

    public int getID()
    {
        IDCounter++;
        return IDCounter;
    }
}
