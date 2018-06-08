using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IDAssigner : MonoBehaviour {

    /**
     * Assigns ID
     */
    private int IDCounter = 1;

    public int getID()
    {
        IDCounter++;
        return IDCounter;
    }
}
