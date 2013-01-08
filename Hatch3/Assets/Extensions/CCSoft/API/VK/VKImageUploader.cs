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

public class VKImageUploader {
	
	private string proxyUrl = "http://carb.ccsoft.ru/vkupload.php";
	private Hashtable _postData;
	

	
	private VKAPI _api;
	
	public VKImageUploader(string url, VKAPI api, Hashtable postData) {
		_api = api;
		_postData = postData;
		uploadImage(url);
	}
	
	
	private void uploadImage(string url) {
		WWWUtil www = WWWUtil.createRequest();
		
		WWWForm post =  new WWWForm();
		post.AddField("vkUrl", _api.uploadeServer);
		post.AddField("imageUrl", url);
		
		www.addEventListner(WWWUtil.WWW_REQUEST_SUCCESS, onWebRequestComplete);
		www.send(proxyUrl, post);
	}
	
	
	private void onWebRequestComplete(object text) {
		Debug.Log("onWebRequestComplete");
		Hashtable data = JSON.Decode(text.ToString()) as Hashtable;
		
		Hashtable param = new Hashtable();
		param.Add("server", data["server"]);
		param.Add("photo",  data["photo"]);
		param.Add("hash",   data["hash"]);
		
		
		IFrameLib.exec("photos.saveWallPhoto", JSON.Encode(param).ToString(), onSaveWallPhotoComplete);

	}
	public void test() {
		
	}
	private void onSaveWallPhotoComplete(object data) {
		
		DebugConsole.Log("onSaveWallPhotoComplete");

		ArrayList list = data as ArrayList;
		Hashtable rData = list[0] as Hashtable;
		
	
		string photoId = rData["id"] as string;

		
		List<string> uids = _postData["uids"] as List<string>;
		DebugConsole.Log(uids);
		DebugConsole.Log(uids.Count);
		DebugConsole.Log(uids[0]);
	
		foreach(string user in uids) {
			DebugConsole.Log(user);
			
			Hashtable param =  new Hashtable();
			param.Add("owner_id", user);
			param.Add("message", _postData["title"]  + "\n" + _postData["text"] + "\n\n" + _postData["url"]);
			param.Add("attachments",  photoId );
			
			IFrameLib.exec("wall.post", JSON.Encode(param).ToString(), _api.simplePostResult);
		}
		
	}
	
	
}

