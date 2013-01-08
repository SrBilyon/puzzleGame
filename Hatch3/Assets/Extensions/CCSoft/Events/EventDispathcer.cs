////////////////////////////////////////////////////////////////////////////////
//
// CRYSTAL CLEAR SOFTWARE
// Copyright 2012 Crystal Clear Software. http://ccsoft.ru
// All Rights Reserved. Unity CCSoft lib
// @author Osipov Stanislav lacost.20@gmail.com
//
//
// NOTICE: Crystal Soft does not allow to use, modify, or distribute this file
// for any purpose
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class EventDispathcer : MonoBehaviour
{
	public delegate void EventHandlerFunction();
	public delegate void DataEventHandlerFunction(object data);
	
	
	private Hashtable listners =  new Hashtable();
	
	
	
	//--------------------------------------
	// PUBLIC METHODS
	//--------------------------------------
	
	public void addEventListner(string eventName, EventHandlerFunction handler) {
		if(listners.ContainsKey(eventName)) {
			(listners[eventName] as ArrayList).Add(handler);	
		} else {
			ArrayList handlers =  new ArrayList();
			handlers.Add(handler);
			listners.Add(eventName, handlers);
		}
	}
	
	public void addEventListner(string eventName, DataEventHandlerFunction handler) {
		if(listners.ContainsKey(eventName)) {
			(listners[eventName] as ArrayList).Add(handler);
			
		} else {
			ArrayList handlers =  new ArrayList();
			handlers.Add(handler);
			listners.Add(eventName, handlers);
		}
	}
	
	
	public void removeEventListner(string eventName, EventHandlerFunction handler) {
		if(listners.ContainsKey(eventName)) {
			ArrayList handlers =  listners[eventName] as ArrayList;
			handlers.Remove(handler);

			if(handlers.Count == 0) {
				listners.Remove(eventName);
			}
		}
	}
	
	public void removeEventListner(string eventName, DataEventHandlerFunction handler) {
		if(listners.ContainsKey(eventName)) {
			ArrayList handlers =  listners[eventName] as ArrayList;
			handlers.Remove(handler);

			if(handlers.Count == 0) {
				listners.Remove(eventName);
			}
		}
	}
	
	
	public void dispatch(string eventName) {
		
		if(listners.ContainsKey(eventName)) {
			ArrayList handlers =  listners[eventName] as ArrayList;
			
			
			for(int i = 0; i < handlers.Count; i++) {
				try {
					if(handlers[i] is EventHandlerFunction) {
						(handlers[i] as EventHandlerFunction)();
					}
					
					if(handlers[i] is DataEventHandlerFunction) {
						(handlers[i] as DataEventHandlerFunction)(null);
					}	
				} catch {}
				
				
			}
		}

	}
	
	
	public void dispatch(string eventName, object data) {
		
		if(listners.ContainsKey(eventName)) {
			ArrayList handlers =  listners[eventName] as ArrayList;
			int len = handlers.Count;
			for(int i = 0; i < len; i++) {
				if(handlers[i] is EventHandlerFunction) {
					(handlers[i] as EventHandlerFunction)();
				}
				
				if(handlers[i] is DataEventHandlerFunction) {
					(handlers[i] as DataEventHandlerFunction)(data);
				}
			}
		}

	}

}

