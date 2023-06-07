using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapTranslation : MonoBehaviour {

    public Transform Player;
    public Transform PlayerForward;
    public Transform PlayerBack;
    public Transform PlayerRight;
    public Transform PlayerLeft;
    private bool ReadyToMoveTurn;
    private float timeLeft;
    private float timerToMove;

    public OVRPlayerController VrPlayer;
    

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if (VrPlayer.SnapTranslation)
        {
            VrPlayer.setMoveThrottle(Vector3.zero);
            if (ReadyToMoveTurn && OVRInput.Get(OVRInput.Button.PrimaryThumbstickUp))
            {
                if (PlayerForward.GetComponent<canTeleport>().teleportationAvailable)
                {
                    Player.transform.position = PlayerForward.position;
                    ReadyToMoveTurn = false;
                    timerToMove = timeLeft;

                }

                


            }
            else if (ReadyToMoveTurn && OVRInput.Get(OVRInput.Button.PrimaryThumbstickDown))
            {

                if (PlayerBack.GetComponent<canTeleport>().teleportationAvailable)
                {
                    Player.transform.position = PlayerBack.position;
                    ReadyToMoveTurn = false;
                    timerToMove = timeLeft;
                }
               
            }
            else if (ReadyToMoveTurn && OVRInput.Get(OVRInput.Button.PrimaryThumbstickLeft))
            {
                if (PlayerLeft.GetComponent<canTeleport>().teleportationAvailable)
                {
                    Player.transform.position = PlayerLeft.position;
                    ReadyToMoveTurn = false;
                    timerToMove = timeLeft;
                }
               
            }
            else if (ReadyToMoveTurn && OVRInput.Get(OVRInput.Button.PrimaryThumbstickRight))
            {
                if (PlayerRight.GetComponent<canTeleport>().teleportationAvailable)
                {
                    Player.transform.position = PlayerRight.position;
                    ReadyToMoveTurn = false;
                    timerToMove = timeLeft;
                }
               
            }
            else
            {
                timerToMove -= Time.deltaTime;

                if (timerToMove < 0)
                {
                    ReadyToMoveTurn = true;
                }


            }


        }
    }
        
}
