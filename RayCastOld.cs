using System;
using UnityEngine.UI;
using UnityEngine;

[Obsolete("This class is supposed to work with StartGame.class that is also Obsolete")]
public class RayCastOld : MonoBehaviour {
	public GameObject SciFiGunLightBlack;
    public StartGame startScript;
    public Text selectedNumber;

    void FixedUpdate(){
		RaycastHit hit;
		Vector3 fwd = SciFiGunLightBlack.transform.TransformDirection(Vector3.forward);
		if (!Physics.Raycast(SciFiGunLightBlack.transform.position, fwd, out hit))
			return;
			
		if (hit.collider.CompareTag("selectable")) {
			selectedNumber.text = hit.collider.name;
			
			if (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) == 0) 
				return;
			
            print("Collided with " + hit.collider.gameObject.name);

            startScript.nauseScore = int.Parse(hit.collider.gameObject.name);

            startScript.activateQuestion = false;
            startScript.nextTrialReady = true;
            startScript.lastTime = 0f;
            startScript.TimeCounter = 0f;
		}
        else
			selectedNumber.text = "";
    } 
}
