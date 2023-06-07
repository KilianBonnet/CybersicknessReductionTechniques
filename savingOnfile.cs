using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using LitJson;

public class savingOnfile : MonoBehaviour {
	private string jsonstring;
	private JsonData itemData;
	// Use this for initialization
	void Start () {
		//jsonstring = File.ReadAllText (Application.dataPath + "/Resources/data.json");

		//itemData = JsonMapper.ToObject (jsonstring);
		//Debug.Log(itemData["q3"] );
	}  
	
	// Update is called once per frame
	void Update () {
		
	}
}
