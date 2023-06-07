using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using LitJson;

public static class GlobalVariables{

	public static bool one = true; 
}


public class GUISelect : MonoBehaviour {
	private string strarr;
   
	public float diff;
	// Use this for initialization
	void Start () {


	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnCollisionEnter(Collision col){
		if (col.transform.tag == "selectable") {
			col.transform.Rotate(0, 90, 0 * Time.deltaTime);
		}
	}
	void OnCollisionStay(Collision col){
		if (col.transform.tag == "selectable" && OVRInput.Get (OVRInput.Button.Any) && GlobalVariables.one == true) {
			strarr= col.transform.name;
	//		Debug.Log(strarr);
			File.AppendAllText (Application.dataPath + "/Resources/data.txt", strarr + "," );
			GlobalVariables.one = false;
		}
	
	}
	void OnCollisionExit(Collision col){
		if (col.transform.tag == "selectable") {
			col.transform.Rotate (0, -90, 0 * Time.deltaTime);
		}
	}
}
