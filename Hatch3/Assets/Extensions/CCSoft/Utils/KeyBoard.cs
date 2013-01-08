using UnityEngine;
using System.Collections;

public class KeyBoard : MonoBehaviour {

	static void DisableKeys( KeyCode[] keys ) {
		if( !Event.current.isKey ) {
		    return;
		}
	
		foreach( KeyCode key in keys ) {
		    if( Event.current.keyCode == key ) {
		        Event.current.Use();
		    }
		}
	}
	
	static void DisableKey( KeyCode key) {
		DisableKeys( new KeyCode[]{ key } );
	}
}
