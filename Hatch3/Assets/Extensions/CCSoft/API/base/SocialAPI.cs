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
using System.Collections.Generic;


public abstract class SocialAPI : MonoBehaviour, ISocialAPI
{

	public delegate void APIEventHandler();
	
	private Hashtable listners =  new Hashtable();
	private static SocialAPI _instance = null;
	
	
	public virtual void Awake() {
		_instance = this;
		IFrameLib.init(this);
	}
	
	//--------------------------------------
	// PUBLIC METHODS
	//--------------------------------------
	
	public void addApiEventListner(SocialApiEvent e, APIEventHandler handler) {
		if(listners.ContainsKey(e)) {
			(listners[e] as ArrayList).Add(handler);
			
		} else {
			ArrayList handlers =  new ArrayList();
			handlers.Add(handler);
			listners.Add(e, handlers);
		}
	}
	
	public void removeApiEventListner(SocialApiEvent e, APIEventHandler handler) {
		if(listners.ContainsKey(e)) {
			ArrayList handlers =  listners[e] as ArrayList;
			foreach(APIEventHandler func in handlers) {
				if(func == handler) {
					handlers.Remove(handler);
				}
			}
			
			if(handlers.Count == 0) {
				listners.Remove(e);
			}
		}
	}
	
	public abstract void invite();
	public abstract void invite(List<string> uids);
	
	public abstract void post(string title, string text, string url);
	public abstract void post(string title, string text, string url, List<string> uids);
	public abstract void post(string title, string text, string url, List<string> uids, string image);

	
	//--------------------------------------
	// GET / SET
	//--------------------------------------
	
	
		
	public static SocialAPI instance {
		get {
			return _instance;
		}
	}
	
	
	public abstract ApiType type {
		get;
	}
	
	public abstract sApiUserInfo userInfo{
		get;
	}
	
	
	//--------------------------------------
	// PROTECTED METHODS
	//--------------------------------------	
	
	protected void dispatch(SocialApiEvent e) {
		
		if(listners.ContainsKey(e)) {
			ArrayList handlers =  listners[e] as ArrayList;
			foreach(APIEventHandler func in handlers) {
				func();
			}
		}
		
		DebugConsole.Log("API EVENT: " + e.ToString());
	}
	
	
}

