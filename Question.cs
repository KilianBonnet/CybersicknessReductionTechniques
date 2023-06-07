using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Question : MonoBehaviour {
	AudioSource a;
	// Use this for initialization
	void Start () {
		a = GetComponent<AudioSource> ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (Input.GetKey ("p")) {
			a.Play ();
		
		}

	}
}
