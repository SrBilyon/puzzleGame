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
using System;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("API/VK API")]

public class VKAPI : SocialAPI {

	private string _authKey;
	private sApiUserInfo _userInfo =  new sApiUserInfo();
	private string _uploadeServer = "";
	private int _allow;
	
	
		
	private List<string> _appFriends = null;
	private List<string> _allFriends = null;
	
	private List<sApiShortUserInfo>  _appFriendsInfo =  null;
	
	
	//--------------------------------------
	// PUBLIC METHODS
	//--------------------------------------
	
	
	public void onAppLoaded(string JSONData) {
		
		Hashtable data = JSON.Decode(JSONData) as Hashtable;
		_userInfo.id   = data["viewer_id"] as string;
		_authKey       = data["auth_key"]  as string;
		
				
		initApplication();
	}
	
	
	public void initApplication() {	
		
		IFrameLib.addCallback("onWindowFocus", onWindowFocus);
		IFrameLib.addCallback("onWindowBlur",  onWindowBlur);
		
		
		requestPlayerInfo();
		requestUploadServerUrl();
		requestFriendList();
		requestApplicationFriendList();
		getUserSettings();
	}
	

	public void requestPlayerInfo() {
		IFrameLib.exec("getProfiles", "{ 'uids': " + _userInfo.id + ", 'fields': 'uid, first_name, last_name, birthdate, photo_rec, city,sex'} ", onPlayerInfo);
	}
	
	public void requestUploadServerUrl() {
		IFrameLib.exec("photos.getWallUploadServer", "{}", onUploadServerUrl);
	}
	
	
	public override void invite() {
		IFrameLib.callEvent("showInviteBox");
	}
	
	public override void invite(List<string> uids) {
		invite();
	}
	
	
	private void getUserSettings() {
		IFrameLib.exec("getUserSettings", "{ }", reciveUserSettings);
	}
	
	public void requestFriendList() {
		if (_appFriends == null) {
			Hashtable param =  new Hashtable();
			param.Add("uid", _userInfo.id);
			param.Add("fields", "uid,first_name,photo_rec,last_name,bdate");
			IFrameLib.exec("friends.get", JSON.Encode(param).ToString() , onFriendsList);
		} else {
			dispatch(SocialApiEvent.FRIENDS_LOADED);
		}
	}
	
	
	/**
	 * Загрузить друзей с установленным приложением
	 */
	public void requestApplicationFriendList() {
		if (_appFriends == null) {
			IFrameLib.exec("friends.getAppUsers", "{}", onAppFriendsList);
		} else {
			dispatch(SocialApiEvent.APP_FRIENDS_LOADED);
		}
	}
	
	
	public override void post(string title, string text, string url) {
		List<string> uids =  new List<string>();
		
		uids.Add(_userInfo.id);
		post(title, text, url, uids);
	}
	
	public override void post(string title, string text, string url, List<string> uids) {
		
		foreach( string user in uids) {
			Hashtable param =  new Hashtable();
			param.Add("owner_id", user);
			param.Add("message", title  + "\n" + text + "\n\n" + url);
			IFrameLib.exec("wall.post", JSON.Encode(param).ToString(), simplePostResult);
		}
		
	}
	
	public override void post(string title, string text, string url, List<string> uids, string image) {
		Hashtable postData = new Hashtable();
		if(uids == null) {
			uids =  new List<string>(); 
			uids.Add(_userInfo.id);
		}
		
		if(uids.Count == 0) {
			uids.Add(_userInfo.id);
		}
		
		postData.Add("title", title);
		postData.Add("text", text);
		postData.Add("url", url);
		postData.Add("uids", uids);
		
		new VKImageUploader(image, this, postData);
	}
	
	//--------------------------------------
	// EVENTS
	//--------------------------------------
	
	private void onPlayerInfo(object data) {
		try {
			DebugConsole.Log("onPlayerInfo");
			Hashtable info  = (data as ArrayList)[0] as Hashtable;
			
			_userInfo.fullName  = info["first_name"] as string;
			_userInfo.name	    = info["first_name"] as string;
			_userInfo.city 		= info["city"] as string;
			_userInfo.birthDate = info["bdate"] as string;
			
			dispatch(SocialApiEvent.PLAYER_INFO_LOADED);

		} catch(System.Exception e) {
			DebugConsole.Log(e.Message);
		}
		
	}
	
	private void onUploadServerUrl(object data) {
		DebugConsole.Log("onUploadServerUrl: " + JSON.Encode(data).ToString());
		Hashtable rData = data as Hashtable;
		_uploadeServer = rData["upload_url"] as string;
	}
	
	public void simplePostResult(object data) {
		DebugConsole.Log("simplePostResult: " + JSON.Encode(data).ToString());
	}
	
	
	private void onWindowFocus(object data) {
		Application.ExternalCall("showPlayer");
	}
	
	private void onWindowBlur(object data) {
		Application.ExternalCall("hidePlayer");
	}
	
	private void reciveUserSettings(object data) {
		DebugConsole.Log("reciveUserSettings: " + JSON.Encode(data).ToString());

		_allow	= Int32.Parse(data.ToString());
		
		DebugConsole.Log("allow perm " + _allow);
			
	}
	
	private void onFriendsList(object data) {
		ArrayList friendsArray = data as ArrayList;
		_allFriends 		= new List<string>();
		_appFriendsInfo		= new List<sApiShortUserInfo>();
		
		int len = friendsArray.Count;
		sApiShortUserInfo ui;
		
		for(int i = 0; i < len; i++) {
			Hashtable userData = friendsArray[i] as Hashtable;
			
			ui = new sApiShortUserInfo();
			_allFriends.Add(userData["uid"] as string);
			
			ui.id			= userData["uid"] as string;
			ui.url			= "http://vk.com/id" + userData["uid"] as string;
			ui.photo		= userData["photo_rec"] as string;
			ui.name			= userData["first_name"] as string;
			_appFriendsInfo.Add(ui);
			
		}
		
		DebugConsole.Log("FriendsList loaded");
		dispatch(SocialApiEvent.FRIENDS_LOADED);
	}
	
	private void onAppFriendsList(object data) {
		_appFriends =  new List<string>();
		ArrayList userList  = data as ArrayList;
		int len = userList.Count;
		for(int i = 0; i < len; i++) {
			_appFriends.Add(userList[i] as string);
		}
		
		dispatch(SocialApiEvent.APP_FRIENDS_LOADED);
		DebugConsole.Log("onAppFriendsList: " + JSON.Encode(data).ToString());
	}
	
	
	
	
	
	//--------------------------------------
	// GET / SET
	//--------------------------------------

	
	public override ApiType type {
		get {
			return ApiType.VK;
		}
	}
	
	public override sApiUserInfo userInfo{
		get {
			return _userInfo;
		}
	}
	
	
	public string uploadeServer {
		get {
			if(_uploadeServer == "") {
				DebugConsole.LogWarning("uploadeServer URL undefined");
			}
			return _uploadeServer;
		} 
	}
	
	public string authKey {
		get {
			return _authKey;
		}
	}

	
}
