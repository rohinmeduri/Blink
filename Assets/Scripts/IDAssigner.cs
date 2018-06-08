using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IDAssigner : MonoBehaviour {

    /**
     * Keeps track of ids for networked players
     */
    private static int IDCounter = 1;

    public int getID()
    {
        IDCounter++;
        return IDCounter;
    }
}
