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

public class CoroutineThread : MonoBehaviour {
	protected bool _isStarted = false;
	
	//--------------------------------------
	// PUBLIC METHODS
	//--------------------------------------
	
	public virtual void Start() {
		if(_isStarted) {
			return;
		}
		
		StartCoroutine(Run());
	}
	
	public void Stop() {
		if(!_isStarted) {
			return;
		}
		
		StopCoroutine("Run");
	}
	
	//--------------------------------------
	// GET / SET
	//--------------------------------------
	
	public bool isStarted {
		get {
			return _isStarted;
		}
	}
	
	
	//--------------------------------------
	// PUBLIC METHODS
	//--------------------------------------
	
	
	public virtual IEnumerator Run() {
		  yield return null;
	}

}

