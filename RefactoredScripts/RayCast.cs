using UnityEngine.UI;
using UnityEngine;

public class RayCast : MonoBehaviour {
	[SerializeField]
	private Transform sciFiGunLightBlack;
	
	[SerializeField]
	private GameManager gameManager;
    
	[SerializeField]
	private Text selectionUi;

    void FixedUpdate(){
		RaycastHit hit;
		Vector3 fwd = sciFiGunLightBlack.TransformDirection(Vector3.forward);
		if (!Physics.Raycast(sciFiGunLightBlack.position, fwd, out hit))
			return;
			
		if (hit.collider.CompareTag("selectable")) {
			selectionUi.text = hit.collider.name;
			
			if (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) == 0) 
				return;

			int nauseaScore = int.Parse(hit.collider.gameObject.name);
			gameManager.GetCurrentMission().OnNauseaScoreSelection(nauseaScore);
		}
        else
			selectionUi.text = string.Empty;
    } 
}
