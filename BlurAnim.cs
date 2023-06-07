using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlurAnim : MonoBehaviour {

	Material mat;


	// Use this for initialization
	void Start () {

		mat = GetComponent<Renderer> ().material;
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		if(Mathf.Abs (Input.GetAxisRaw("Mouse X")) > 1.5f)
			mat.color = new Color (mat.color.r, mat.color.g, mat.color.b, Mathf.Abs (Input.GetAxis("Mouse X"))/5 - 1.5f/5);
		else if(Input.GetAxisRaw("Mouse X") <= 1)
			mat.color = new Color (mat.color.r, mat.color.g, mat.color.b, 0);
	}
}
