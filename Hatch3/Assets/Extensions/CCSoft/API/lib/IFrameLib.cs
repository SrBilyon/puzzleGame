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

public class IFrameLib: MonoBehaviour  {

	
	public delegate void functionPointer(object data);
	
	private static Dictionary<int, functionPointer> APICallStack = new Dictionary<int, functionPointer>();
	private static int stackId = 0;
	
	private static List<string> _externalCallStack = new List<string>();
	private static float _externalCallStackTimeOut = 0;
	private static bool  _stackIsRuning = false;
	
	private	static  string _callAPI				= "";
	private static  string _eventAPI			= "";
	private static  string _eventCallbackAPI	= "";
	
	
	private static string  _callbackTpl	= "function(data) {response(%CALLBACK_ID%, data)}";
	private const string   CALLBACK_PATTERN = "%CALLBACK_ID%";
	
	private static ISocialAPI _api;
	
	private static IFrameLib _instance;

	
	void Update() {
		if(_stackIsRuning) {
			_externalCallStackTimeOut++;
			
			if(_externalCallStackTimeOut >= 10) {
				_externalCallStackTimeOut = 0;
				ExternalStackCallFunction();
			}
		}
	}
	
	//--------------------------------------
	// PUBLIC METHODS
	//--------------------------------------
	
	public static void init(ISocialAPI api) {
		_api	= api;
	
		switch (_api.type) {

			case ApiType.VK: {
				_callAPI			= "VKapi";
				_eventAPI			= "VK.callMethod";
				_eventCallbackAPI	= "AddCallBack";
				break;
			}
		}
	
		register();
	}

	public static void register() {
		_api.gameObject.AddComponent<IFrameLib>();
		_instance = _api.gameObject.GetComponent<IFrameLib>();

		
		Application.ExternalCall("register", "receiver", "onAppLoaded", _api.gameObject.name);		
	}
	
		
	/**
	 * Выполнить метод API соц. сети
	 * Если callback не указан, exec() попытается вернуть значение
	 * @param method вызываемый метод соц. сети
	 * @param args аргументы передаваемые серверу
	 * @param callback callback-функция в которую будет передан ответ
	 */	
	public static void exec(string method, string args, functionPointer callback) {
		string callPatern;
		callPatern = _callAPI + "('" + method +"'," + args  + ", " + _callbackTpl.Replace(CALLBACK_PATTERN, registerCallBack(callback).ToString()) + ")";	

		_instance.ExternalCall(callPatern);
	
	}
	
	
	/**
	 * Сгенерировать событие для API соц. сети
	 * @param	eventName имя события
	 * @param	callback callback-функция в которую будет передан ответ
	 * @param	args параметры передаваемые вместе с событием
	 */
	public static void callEvent(string eventName, functionPointer callback, string args) {
		string callPatern;
		callPatern = _eventAPI + "('" + eventName +"'," + args + ",function(data) {response(" + registerCallBack(callback).ToString() + ", data)})";
		
		_instance.ExternalCall(callPatern);
	}
	
	
	/**
	 * Сгенерировать событие для API соц. сети
	 * @param	eventName имя события
	 * @param	callback callback-функция в которую будет передан ответ
	 */
	public static void callEvent(string eventName,  functionPointer callback) {
		string callPatern;
		callPatern = _eventAPI + "('" + eventName +"', function(data) {response(" + registerCallBack(callback).ToString() + ", data)})";
		
		_instance.ExternalCall(callPatern);
	}
	
	
	/**
	 * Сгенерировать событие для API соц. сети
	 * @param	eventName имя события
	 */
	public static void callEvent(string eventName) {
		string callPatern;
		callPatern = _eventAPI + "('" + eventName +"')";
		
		_instance.ExternalCall(callPatern);
	}
	
			
	/**
	 * Подключить перехватчик событий генерируемые API соц. сетью
	 * @param	eventName названия события
	 * @param	callback callback-функция которорая будет обрабатывать это событие
	 */
	public static void addCallback(string eventName, functionPointer callback){
		Application.ExternalCall(_eventCallbackAPI, eventName, registerCallBack(callback));
	}

	
	//--------------------------------------
	// PRIVATE METHODS
	//--------------------------------------
	
	private static int registerCallBack(functionPointer callback) {
		
		stackId ++ ;
		DebugConsole.LogWarning("registerCallBack" + stackId);
		APICallStack.Add(stackId, callback);
		
		return stackId;
	}
	
	private void ExternalCall(string callPatern) {
		_externalCallStack.Add(callPatern);
		if(!_stackIsRuning) {
			ExternalStackCallFunction();
		}
		
	}
	
	public void ExternalStackCallFunction() {
		try {
			
			_stackIsRuning = true;
			
			if(_externalCallStack.Count == 0) {
				_stackIsRuning = false;
				return;
			}
			
			string callPatern = _externalCallStack[0];
			_externalCallStack.RemoveAt(0);
				
			
			Application.ExternalCall(callPatern);
			DebugConsole.Log(callPatern);
		
			
		} catch(Exception e) {
			DebugConsole.LogError(e.Message);
		}
	}
	
	
	//--------------------------------------
	// NON-STATIC METHODS
	//--------------------------------------

	/**
	 * Общая callback-функция для получения данных от API соц. сети
	 * @param	id
	 * @param	data
	 */
	public void receiver(string data) {
		try {
			Hashtable requestData = JSON.Decode(data) as Hashtable;
			int id = Int32.Parse(requestData["id"].ToString());
			
			//DebugConsole.Log("ID: " + id);
			
			if(APICallStack.ContainsKey(id)) {
				functionPointer func = APICallStack[id];
				func(requestData["response"]);
				DebugConsole.Log("Call Performed " + id);
			} else {
				DebugConsole.LogWarning("GOT Unknwod api id");
			}
		} catch(Exception ex) {
			DebugConsole.Log(ex.StackTrace);
		}
	}
	

}
