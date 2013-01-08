////////////////////////////////////////////////////////////////////////////////
//
// CRYSTAL CLEAR SOFTWARE
// Copyright 2012 Crystal Clear Software. http://ccsoft.ru
// All Rights Reserved. CCsoft Bear Shooter
// @author Osipov Stanislav lacost.20@gmail.com
//
//
// NOTICE: Crystal Soft does not allow to use, modify, or distribute this file
// for any purpose
//
////////////////////////////////////////////////////////////////////////////////


using UnityEngine;
using System.Collections;


public class ExternalStackTask: CoroutineThread {


	public float callRate = 0;
	
	public override void Start() {
		if(_isStarted) {
			return;
		}
		
		StartCoroutine(Run());
	}
	
	//--------------------------------------
	// PRIVATE METHODS
	//--------------------------------------
	
	public override IEnumerator Run() {
		while(true) {
			yield return new WaitForSeconds(callRate);
			Debug.Log("ccc");
		}
	}
}