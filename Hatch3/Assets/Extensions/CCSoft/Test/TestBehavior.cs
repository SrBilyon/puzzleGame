using UnityEngine;
using System.Collections;
using System.Threading;

[AddComponentMenu("Test/TestBehavior")]

public class TestBehavior : MonoBehaviour {

	
	void Start() {
		StartCoroutine(myFunc(5));
	}
	
	
	// Update is called once per frame
	void Update () {
		
			
		
	}
	
	
	
	IEnumerator myFunc(int val)
	{
		while(true) {
			
			yield return new WaitForSeconds(0.1f);
	  		 Debug.Log(val);
		}
		
	}
}



