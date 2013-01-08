using System; 
using UnityEngine;
using System.Collections; 
using System.Collections.Generic; 

public class WWWUtil : EventDispathcer
{
	
	public const string WWW_REQUEST_ERROR 	= "www_request_error";
	public const string WWW_REQUEST_SUCCESS = "www_request_error";
	
	public static WWWUtil createRequest() {
		GameObject inst =  new GameObject("WWWUtil");
		return inst.AddComponent(typeof(WWWUtil)) as WWWUtil;
	}
	
	
	
	public void send(string url) {
		DebugConsole.LogWarning(url);
		
  		WWW www = new WWW (url);
	    StartCoroutine(WaitForRequest (www));
    }
	
	
    public void send(string url, WWWForm form)
    {
    	
        WWW www = new WWW(url, form);

        StartCoroutine(WaitForRequest(www));
    }
	
	
	private IEnumerator WaitForRequest(WWW www)
    {
        yield return www;

        // check for errors
        if (www.error == null) {
			dispatch(WWW_REQUEST_SUCCESS, www.text);
			Destroy(gameObject);
        } else {
			dispatch(WWW_REQUEST_ERROR, www.error);
            DebugConsole.LogError("WWW Error: "+ www.error);
			Destroy(gameObject);
        }
	}
   
}

