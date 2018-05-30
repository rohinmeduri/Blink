using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blinks : MonoBehaviour{

	private bool gloryFull;

	public Blinks(float glory){
		if (glory != 100) {
			gloryFull = false;
		} 
		else {
			gloryFull = true;
		}
			
	}

	public bool canBlink(){
		return gloryFull;
	}

	public void teleport(){
		
	}
		
}
