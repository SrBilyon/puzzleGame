using UnityEngine;
using System.Collections;

public class testtt : MonoBehaviour {
	
    void Start()
    {
		WWWUtil www = WWWUtil.createRequest();
		www.addEventListner(WWWUtil.WWW_REQUEST_SUCCESS, onRequestComplete);
       	
		www.send("http://carb.ccsoft.ru/vkupload.php");

    }
	
	private void onRequestComplete(object text) {
		DebugConsole.Log(text);
	}
}
