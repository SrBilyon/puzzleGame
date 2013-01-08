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

public class LocalEmulatedAPI : SocialAPI {
	
	public sApiUserInfo _userInfo =  new sApiUserInfo();
	public float apiLoadTimeOut = 1f;
	
	public override void Awake() {
		base.Awake();
		
		_userInfo.fullName = "test test";
		_userInfo.name = "test1";
	}
	
	void Start() {
		onLoad();
	}
	
	
	public override void invite() {
		DebugConsole.Log("invited");
	}
	
	public override void invite(List<string> uids) {
		invite();
	}
	
	public override void post(string title, string text, string url) {
		DebugConsole.Log("post");
	}
	
	public override void post(string title, string text, string url, List<string> uids) {
		post(title, text, url);
	}
	
	public override void post(string title, string text, string url, List<string> uids, string image) {
		post(title, text, url, uids);
	}
	

	//--------------------------------------
	// EVENTS
	//--------------------------------------
	
	public void onLoad() {
		dispatch(SocialApiEvent.PLAYER_INFO_LOADED);
	}
	
	//--------------------------------------
	// GET / SET
	//--------------------------------------
	
	public override ApiType type {
		get {
			return ApiType.LOCAL;
		}
	}
	
	
	public override sApiUserInfo userInfo{
		get {
			return _userInfo;
		}
	}
}
