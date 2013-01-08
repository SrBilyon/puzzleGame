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

public class EasyEventDispatcher  {
	
	public delegate void EventHandlet(string name);
	
	public event EventHandlet SimpleEvent;
	
		
	//--------------------------------------
	// PUBLIC METHODS
	//--------------------------------------
	
	public void dispath(string name) {
		if(SimpleEvent != null) {
			SimpleEvent(name);
		}
	}	
	
	
}
