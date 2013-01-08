using UnityEngine;
using System.Collections;


[AddComponentMenu("Debug/Direction Traser")]

public class DirectionTraser : MonoBehaviour {

	public Color traseColor = Color.green;
	

	void Update () {
	
		Debug.DrawLine(transform.position , transform.position + transform.forward * 5, traseColor);

	}
}
