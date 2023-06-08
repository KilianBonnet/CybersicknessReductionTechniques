/************************************************************************************

Copyright   :   Copyright 2014 Oculus VR, LLC. All Rights reserved.

Licensed under the Oculus VR Rift SDK License Version 3.3 (the "License");
you may not use the Oculus VR Rift SDK except in compliance with the License,
which is provided at the time of installation or download, or which
otherwise accompanies this software in either electronic or hard copy form.

You may obtain a copy of the License at

http://www.oculus.com/licenses/LICENSE-3.3

Unless required by applicable law or agreed to in writing, the Oculus VR SDK
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

************************************************************************************/

using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Controls the player's movement in virtual reality.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class OVRPlayerController : MonoBehaviour
{

    /// <summary>
    /// The rate acceleration during movement.
    /// </summary>
    public float Acceleration = 0.1f;

    private float accelerationChange;

    public bool canMove = false;

	/// <summary>
	/// The rate of damping on movement.
	/// </summary>
	public float Damping = 0.3f;

	/// <summary>
	/// The rate of additional damping when moving sideways or backwards.
	/// </summary>
	public float BackAndSideDampen = 0.5f;

	/// <summary>
	/// The force applied to the character when jumping.
	/// </summary>
	public float JumpForce = 0.3f;

	/// <summary>
	/// The rate of rotation when using a gamepad.
	/// </summary>
	public float RotationAmount = 0.1f;

	/// <summary>
	/// The rate of rotation when using the keyboard.
	/// </summary>
	public float RotationRatchet = 45f;

	/// <summary>
	/// If true, reset the initial yaw of the player controller when the Hmd pose is recentered.
	/// </summary>
	public bool HmdResetsY = true;

	/// <summary>
	/// If true, tracking data from a child OVRCameraRig will update the direction of movement.
	/// </summary>
	public bool HmdRotatesY = false;

	/// <summary>
	/// Modifies the strength of gravity.
	/// </summary>
	public float GravityModifier = 0.379f;
	
	/// <summary>
	/// If true, each OVRPlayerController will use the player's physical height.
	/// </summary>
	public bool useProfileData = true;

	protected CharacterController Controller = null;
	protected OVRCameraRig CameraRig = null;

	private float MoveScale = 1.0f;
	private Vector3 MoveThrottle = Vector3.zero;
	private float FallSpeed = 0.0f;
	private OVRPose? InitialPose;
	private float InitialYRotation = 0.0f;
	private float MoveScaleMultiplier = 1.0f;
	private float RotationScaleMultiplier = 1f;
	private bool  SkipMouseRotation = false;
	private bool  HaltUpdateMovement = false;
	private bool prevHatLeft = false;
	private bool prevHatRight = false;
	private float SimulationRate = 60f;
	private float buttonRotation = 0f;
    private bool ReadyToSnapTurn;
    private bool ReadyToMoveTurn;
    private int timeBeforeMoveAgain = 0;
    private bool canPress = true;
    public float timeLeft = 0.5f; //Only used when SnapTranslation is true
    private float timerToMove;//Only used when SnapTranslation is true
    private float timerToRotate;
    public float timeLeftRotate = 1f;
    public GameObject fader;
	public float yRot = 0;
	IEnumerator rot;
	float timer = 0;
	public int[] angles;
	int angleIndex = 0;

	float mTimer = -1;

	float bTimer = -10;
    Vector3 iposition = Vector3.zero;
    Vector3 fposition = Vector3.zero;

    Vector3 newMove = Vector3.zero;

    public bool SnapRotation;
    public bool SnapTranslation;
    public float distance; // only used when SnapTranslation is true
    public bool snapping;
	public bool blurring;
    //

    public float AmountOfSkip = 1f;
  

	void Start()
	{
        // Add eye-depth as a camera offset from the player controller
        if (!canMove)
        {
            accelerationChange = 0.0f;

        }
        else
        {
            accelerationChange = Acceleration;
        }
        var p = CameraRig.transform.localPosition;
		p.z = OVRManager.profile.eyeDepth;
		CameraRig.transform.localPosition = p;
		//#################################################################  My code ############################################################################
		yRot = angleIndex;


        timerToMove = timeLeft;



        // rot = RotateObject (angles[angleIndex], Vector3.up, 1);
		//yRot per 3rd parameter seconds
		//StartCoroutine(rot);
	}

	void Awake()
	{
		Application.targetFrameRate = 100;
		Controller = gameObject.GetComponent<CharacterController>();

		if(Controller == null)
			Debug.LogWarning("OVRPlayerController: No CharacterController attached.");

		// We use OVRCameraRig to set rotations to cameras,
		// and to be influenced by rotation
		OVRCameraRig[] CameraRigs = gameObject.GetComponentsInChildren<OVRCameraRig>();

		if(CameraRigs.Length == 0)
			Debug.LogWarning("OVRPlayerController: No OVRCameraRig attached.");
		else if (CameraRigs.Length > 1)
			Debug.LogWarning("OVRPlayerController: More then 1 OVRCameraRig attached.");
		else
			CameraRig = CameraRigs[0];

		InitialYRotation = transform.rotation.eulerAngles.y;
	}

	void OnEnable()
	{
	OVRManager.display.RecenteredPose += ResetOrientation;

		if (CameraRig != null)
		{
			CameraRig.UpdatedAnchors += UpdateTransform;
		}
	}

	void OnDisable()
	{
		OVRManager.display.RecenteredPose -= ResetOrientation;

		if (CameraRig != null)
		{
			CameraRig.UpdatedAnchors -= UpdateTransform;
		}
	}

	void Update()
	{
        if (!canMove)
        {
            accelerationChange = 0.0f;

        }
        else
        {
            accelerationChange = Acceleration;
        }
       
        if (mTimer >= 0){
			mTimer -= Time.deltaTime;
		}
		if(bTimer >= 0f){
			bTimer -= Time.deltaTime;
		}

		if (timer == -1) {
			
			if (OVRInput.Get (OVRInput.Button.Any)) {
				//print (angles [angleIndex]);
                //rot = RotateObject (angles[angleIndex], Vector3.up, 1);
                //rot = RotateObject(angles[angleIndex], Vector3.up, 1f);
                //StartCoroutine (rot);
				timer = 0;
			}
		}
		//Use keys to ratchet rotation
		if (Input.GetKeyDown(KeyCode.Q))
			buttonRotation -= RotationRatchet;

		if (Input.GetKeyDown(KeyCode.E))
			buttonRotation += RotationRatchet;

        /*
        if (OVRInput.Get(OVRInput.Button.SecondaryThumbstickLeft))
            {
            if (ReadyToSnapTurn)
            {
                buttonRotation -= RotationRatchet;
                ReadyToSnapTurn = false;
            }
        } else if (OVRInput.Get(OVRInput.Button.SecondaryThumbstickRight)) {
            if (ReadyToSnapTurn)
                {
                buttonRotation += RotationRatchet;
                ReadyToSnapTurn = false;
                            }
        } else {
            ReadyToSnapTurn = true;
         }

        */




    }

	void FixedUpdate(){

	//	print (timer);
		if(timer >= 0){
			timer += Time.deltaTime;
			//print (timer);
		}
			
		
		if (timer >= 60) {
		//	StopCoroutine (rot);

		angleIndex++;
		timer = -1;
		}


	}
    


    IEnumerator RotateObject(float angle, Vector3 axis, float inTime)
	{
		// calculate rotation speed
		float rotationSpeed = angle / inTime;

		while (true)
		{
			// save starting rotation position
			Quaternion startRotation = transform.rotation;

			float deltaAngle = 0;

			// rotate until reaching angle
			while (deltaAngle < angle)
			{
				deltaAngle += rotationSpeed * Time.deltaTime;

				deltaAngle = Mathf.Min(deltaAngle, angle);

				transform.rotation = startRotation * Quaternion.AngleAxis(deltaAngle, axis);

				yield return null;
			}

			// delay here
			yield return new WaitForSeconds(0);
		}
	}

    IEnumerator RotateObjectVR(float angle, Vector3 axis, float inTime, Vector3 euler, float rotateInfluence)
    {
        // calculate rotation speed
        //float rotationSpeed = angle / inTime;
        //Vector2 secondaryAxis = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);
        //euler.y += (secondaryAxis.x * rotateInfluence);
        //euler.y += secondaryAxis.x * rotateInfluence;
        Quaternion startRotation = transform.rotation;
        while (true)
        {
            // save starting rotation position
            // Quaternion startRotation = transform.rotation;
            float rotationSpeed = angle / inTime;
            Vector2 secondaryAxis = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);
            euler.y += (secondaryAxis.x * rotationSpeed);


            // rotate until reaching angle
            // while (deltaAngle < angle)
            //{
            //euler.y += rotationSpeed * axis.y ;
            //print(deltaAngle);

            // deltaAngle = Mathf.Min(deltaAngle, angle);
            // Vector3 finalangle = new Vector3(0f, deltaAngle, 0f);
            transform.rotation = startRotation *  Quaternion.Euler(euler);
                //startRotation * Quaternion.AngleAxis(deltaAngle, axis);

  //              yield return null;
//            }

            // delay here
            yield return new WaitForSeconds(0);
        }
    }



    protected virtual void UpdateController()
	{
		if (useProfileData)
		{
			if (InitialPose == null)
			{
				// Save the initial pose so it can be recovered if useProfileData
				// is turned off later.
				InitialPose = new OVRPose()
				{
					position = CameraRig.transform.localPosition,
					orientation = CameraRig.transform.localRotation
				};
			}

			var p = CameraRig.transform.localPosition;
			if (OVRManager.instance.trackingOriginType == OVRManager.TrackingOrigin.EyeLevel)
			{
				p.y = OVRManager.profile.eyeHeight - (0.5f * Controller.height) + Controller.center.y;
			}
			else if (OVRManager.instance.trackingOriginType == OVRManager.TrackingOrigin.FloorLevel)
			{
				p.y = - (0.5f * Controller.height) + Controller.center.y;
			}
			CameraRig.transform.localPosition = p;
		}
		else if (InitialPose != null)
		{
			// Return to the initial pose if useProfileData was turned off at runtime
			CameraRig.transform.localPosition = InitialPose.Value.position;
			CameraRig.transform.localRotation = InitialPose.Value.orientation;
			InitialPose = null;
		}
        
       // Debug.Log(iposition);

		UpdateMovement();
        

       

        Vector3 moveDirection = Vector3.zero;

		float motorDamp = (1.0f + (Damping * SimulationRate * Time.deltaTime));

		MoveThrottle.x /= motorDamp;
		MoveThrottle.y = (MoveThrottle.y > 0.0f) ? (MoveThrottle.y / motorDamp) : MoveThrottle.y;
		MoveThrottle.z /= motorDamp;

        moveDirection += MoveThrottle * SimulationRate * Time.deltaTime;



        /*while (OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick) && Mathf.Abs(Mathf.Exp(MoveThrottle.x) + Mathf.Exp(MoveThrottle.y) + Mathf.Exp(MoveThrottle.z)) < 1f)
        {
            

        }
        */
        if (SnapTranslation)
        {
           
            if(moveDirection.x > 0.5 || moveDirection.x < -0.5)
            {
                moveDirection.z = 0f;
                if(moveDirection.x > 0)  moveDirection.x = AmountOfSkip;
                if (moveDirection.x < 0) moveDirection.x = -AmountOfSkip;
            }
            else if (moveDirection.z > 0.5 || moveDirection.z < -0.5)
            {
                moveDirection.x = 0f;
                if (moveDirection.z > 0) moveDirection.z = AmountOfSkip;
                if (moveDirection.z < 0) moveDirection.z = -AmountOfSkip;
            }
            else
            {
                float AmountRemaining = Mathf.Pow(Mathf.Abs(Mathf.Pow(AmountOfSkip, 2) - Mathf.Pow(moveDirection.x, 2)), 0.5f);
                //Debug.Log(AmountRemaining);
                if (moveDirection.x > 0f) moveDirection.x = AmountOfSkip;
                if (moveDirection.x < 0f) moveDirection.x = -AmountOfSkip;
                if (moveDirection.z > 0f) moveDirection.z = AmountRemaining;
                if (moveDirection.z < 0f) moveDirection.z = -AmountRemaining;
            }
        }


        // Gravity
        if (Controller.isGrounded && FallSpeed <= 0)
			FallSpeed = ((Physics.gravity.y * (GravityModifier * 0.002f)));
		else
			FallSpeed += ((Physics.gravity.y * (GravityModifier * 0.002f)) * SimulationRate * Time.deltaTime);

		moveDirection.y += FallSpeed * SimulationRate * Time.deltaTime;

		// Offset correction for uneven ground
		float bumpUpOffset = 0.0f;

        if (Controller.isGrounded && MoveThrottle.y <= transform.lossyScale.y * 0.001f)
		{
			bumpUpOffset = Mathf.Max(Controller.stepOffset, new Vector3(moveDirection.x, 0, moveDirection.z).magnitude);
			moveDirection -= bumpUpOffset * Vector3.up;
		}

        


        Vector3 predictedXZ = Vector3.Scale((Controller.transform.localPosition + moveDirection), new Vector3(1, 0, 1));

       


        // Move contoller
        Controller.Move(moveDirection);

		Vector3 actualXZ = Vector3.Scale(Controller.transform.localPosition, new Vector3(1, 0, 1));

		if (predictedXZ != actualXZ)
			MoveThrottle += (actualXZ - predictedXZ) / (SimulationRate * Time.deltaTime);
        /*
        if (SnapTranslation)
        {
            
            float AmountRemaining = Mathf.Pow(Mathf.Pow(AmountOfSkip, 2) - Mathf.Pow(MoveThrottle.x, 2), 0.5f);
            //Debug.Log(AmountRemaining);
            if (MoveThrottle.x > 0f) MoveThrottle.x = AmountOfSkip;
            if (MoveThrottle.x < 0f) MoveThrottle.x = -AmountOfSkip;
            if (MoveThrottle.z > 0f) MoveThrottle.z = AmountRemaining;
            if (MoveThrottle.z < 0f) MoveThrottle.z = -AmountRemaining;

            print(MoveThrottle);
            
        }
    */
    }

    public virtual void UpdateMovement()
    {
        if (HaltUpdateMovement)
            return;

        bool moveForward = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
        bool moveLeft = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
        bool moveRight = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);
        bool moveBack = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);

        bool dpad_move = false;

        if (OVRInput.Get(OVRInput.Button.DpadUp))
        {
            moveForward = true;
            dpad_move = true;

        }

        if (OVRInput.Get(OVRInput.Button.DpadDown))
        {
            moveBack = true;
            dpad_move = true;
        }

        MoveScale = 1.0f;

        if ((moveForward && moveLeft) || (moveForward && moveRight) ||
             (moveBack && moveLeft) || (moveBack && moveRight))
            MoveScale = 0.70710678f;

        // No positional movement if we are in the air
        if (!Controller.isGrounded)
            MoveScale = 0.0f;

        MoveScale *= SimulationRate * Time.deltaTime;

        // Compute this for key movement
        float moveInfluence = accelerationChange * 0.1f * MoveScale * MoveScaleMultiplier;

        // Run!
        if (dpad_move || Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            moveInfluence *= 2.0f;

        Quaternion ort = transform.rotation;
        Vector3 ortEuler = ort.eulerAngles;
        ortEuler.z = ortEuler.x = 0f;
        ort = Quaternion.Euler(ortEuler);

        if (moveForward)
            MoveThrottle += ort * (transform.lossyScale.z * moveInfluence * Vector3.forward);
        if (moveBack)
            MoveThrottle += ort * (transform.lossyScale.z * moveInfluence * BackAndSideDampen * Vector3.back);
        if (moveLeft)
            MoveThrottle += ort * (transform.lossyScale.x * moveInfluence * BackAndSideDampen * Vector3.left);
        if (moveRight)
            MoveThrottle += ort * (transform.lossyScale.x * moveInfluence * BackAndSideDampen * Vector3.right);

        Vector3 euler = transform.rotation.eulerAngles;

        bool curHatLeft = OVRInput.Get(OVRInput.Button.PrimaryShoulder);

        if (curHatLeft && !prevHatLeft)
            euler.y -= RotationRatchet;

        prevHatLeft = curHatLeft;

        bool curHatRight = OVRInput.Get(OVRInput.Button.SecondaryShoulder);

        if (curHatRight && !prevHatRight)
            euler.y += RotationRatchet;

        prevHatRight = curHatRight;

        euler.y += buttonRotation;
        buttonRotation = 0f;

        float rotateInfluence = SimulationRate * Time.deltaTime * RotationAmount * RotationScaleMultiplier;
       //float rotateInfluence = 3f;
        

#if !UNITY_ANDROID || UNITY_EDITOR
        if (!SkipMouseRotation)
            //euler.y += Input.GetAxis("Mouse X") * rotateInfluence * 3.25f;
            euler.y += Input.GetAxis("Mouse X") / 2;
        //Debug.Log(Input.GetAxis("Mouse X")/2);
        if (snapping) {
            if (Input.GetAxis("Mouse X") > 3f && mTimer < 0) {
                //fader.SetActive(true);
                mTimer = 0.5f;
                bTimer = 0.25f;
                euler.y += 20f;
            } else if (Input.GetAxis("Mouse X") < -3f && mTimer < 0) {
                mTimer = 0.5f;
                bTimer = 0.25f;
                euler.y -= 20f;
                //fader.SetActive(true);
            } else {
                //
                fader.SetActive(false);
            }
        }
        //		if(blurring){
        //			if(bTimer >= 0)
        //				fader.SetActive(true);
        //			else
        //				fader.SetActive(false);
        ////			if(Input.GetAxis("Mouse X") > 3f && mTimer < 0){
        ////				fader.SetActive(true);
        ////				mTimer = 0.5f;
        ////				euler.y += 20f;
        ////			}else if(Input.GetAxis("Mouse X") < -3f && mTimer < 0) {
        ////				mTimer = 0.5f;
        ////				euler.y -= 20f;
        ////				//fader.SetActive(false);
        ////			}else{
        ////				//
        ////				//			fader.SetActive(false);
        ////			}
        //
        //		}

#endif

        moveInfluence = accelerationChange * 0.1f * MoveScale * MoveScaleMultiplier;

#if !UNITY_ANDROID // LeftTrigger not avail on Android game pad
        //moveInfluence *= 1.0f + OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger);
#endif


        Vector2 primaryAxis = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);

        if (primaryAxis.x > 0.1f) primaryAxis.x = accelerationChange;

        if (primaryAxis.y > 0.1f) primaryAxis.y = accelerationChange;
        



        if (SnapTranslation)
        {
            MoveThrottle = Vector3.zero;
          

            
                //print(OVRInput.Get(OVRInput.Button.PrimaryThumbstickUp));
            if (ReadyToMoveTurn && OVRInput.Get(OVRInput.Button.PrimaryThumbstickUp))
            {
                    MoveThrottle += ort * (primaryAxis.y * transform.lossyScale.z * moveInfluence * Vector3.forward)*distance;
                    ReadyToMoveTurn = false;
                    timerToMove = timeLeft;
                   // print(MoveThrottle);
               
            }
            else if (ReadyToMoveTurn && OVRInput.Get(OVRInput.Button.PrimaryThumbstickDown))
            { 
                    MoveThrottle += ort * (Mathf.Abs(primaryAxis.y) * transform.lossyScale.z * moveInfluence * BackAndSideDampen * Vector3.back)*distance;
                    ReadyToMoveTurn = false;
                    timerToMove = timeLeft;
            } else if (ReadyToMoveTurn && OVRInput.Get(OVRInput.Button.PrimaryThumbstickLeft))
            {
                    MoveThrottle += ort * (Mathf.Abs(primaryAxis.x) * transform.lossyScale.x * moveInfluence * BackAndSideDampen * Vector3.left)*distance;
                    ReadyToMoveTurn = false;
                    timerToMove = timeLeft;
            } else if (ReadyToMoveTurn && OVRInput.Get(OVRInput.Button.PrimaryThumbstickRight))
            {
                    MoveThrottle += ort * (primaryAxis.x * transform.lossyScale.x * moveInfluence * BackAndSideDampen * Vector3.right)*distance;
                    ReadyToMoveTurn = false;
                    timerToMove = timeLeft;
            }
            else
            {
                

                 timerToMove -= Time.deltaTime;
               // timeLeftBlackOut -= Time.deltaTime;

               
                  if (timerToMove < 0)
                  {
                       ReadyToMoveTurn = true;
                        MoveThrottle = Vector3.zero;
                  }


            }


            // float AmountRemaining = Mathf.Pow(Mathf.Pow(AmountOfSkip, 2) - Mathf.Pow(MoveThrottle.x, 2), 1 / 2);
            //Debug.Log(AmountRemaining);

//            if (MoveThrottle.x > 0.1f) MoveThrottle.x = AmountOfSkip;
  //          if (MoveThrottle.x < 0f) MoveThrottle.x = -AmountOfSkip;
    //        if (MoveThrottle.z > 0.1f) MoveThrottle.z = AmountRemaining;
      //      if (MoveThrottle.z < 0f) MoveThrottle.z = -AmountRemaining;
            /*
            if (MoveThrottle.x > 0.2f)
            {
                MoveThrottle.x = AmountOfSkip;
              //  MoveThrottle.z = 0f;
            }
            else if (MoveThrottle.x < -0.2f)
            {
                MoveThrottle.x = -AmountOfSkip;
               // MoveThrottle.z = 0f;
            }
            if (MoveThrottle.z > 0.2f)
            {
                MoveThrottle.z = AmountRemaining;
                //MoveThrottle.x = 0f;
               //MoveThrottle.z = AmountOfSkip;

            }
            else if (MoveThrottle.z < -0.2f)
            {
                MoveThrottle.z = -AmountRemaining;
              //  MoveThrottle.z = -AmountOfSkip;
             //   MoveThrottle.x = 0f;
            }*/
          
        }
        else
        {



        if (primaryAxis.y > 0.0f)
            MoveThrottle += ort * (primaryAxis.y * transform.lossyScale.z * moveInfluence * Vector3.forward);

        if (primaryAxis.y < 0.0f)
            MoveThrottle += ort * (Mathf.Abs(primaryAxis.y) * transform.lossyScale.z * moveInfluence * BackAndSideDampen * Vector3.back);

        if (primaryAxis.x < 0.0f)
            MoveThrottle += ort * (Mathf.Abs(primaryAxis.x) * transform.lossyScale.x * moveInfluence * BackAndSideDampen * Vector3.left);

        if (primaryAxis.x > 0.0f)
            MoveThrottle += ort * (primaryAxis.x * transform.lossyScale.x * moveInfluence * BackAndSideDampen * Vector3.right);




        }







    if (SnapRotation)
        {
            if (OVRInput.Get(OVRInput.Button.SecondaryThumbstickLeft) && ReadyToSnapTurn)
            {
                
                    euler.y -= RotationRatchet;
                    ReadyToSnapTurn = false;
                    timerToRotate = timeLeftRotate;
                
               
            }
            else if (OVRInput.Get(OVRInput.Button.SecondaryThumbstickRight) && ReadyToSnapTurn)
            {
               
                    euler.y += RotationRatchet;
                    ReadyToSnapTurn = false;
                    timerToRotate = timeLeftRotate;
               
            }
            else
            {
                timerToRotate -= Time.deltaTime;

                if (timerToRotate < 0)
                {
                    ReadyToSnapTurn = true;

                }
            }
            transform.rotation = Quaternion.Euler(euler);
    }
    else
    {
            Vector2 secondaryAxis = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);
           // print(secondaryAxis);
            if (secondaryAxis.x > 0.1f) secondaryAxis.x = Acceleration;
            if (secondaryAxis.x < -0.1f) secondaryAxis.x = -Acceleration;

            if (secondaryAxis.y > 0.1f) secondaryAxis.y = Acceleration;
            if (secondaryAxis.y < -0.1f) secondaryAxis.y = -Acceleration;
            //if (secondaryAxis.z > 0.1f) secondaryAxis.z = accelerationChange;
            //float rotationSpeed = 3f;

            euler.y += secondaryAxis.x * rotateInfluence;
            
            
            // print(secondaryAxis.x);
            //  Debug.Log(Quaternion.Euler(euler));
            //rot = RotateObjectVR(3f, secondaryAxis, 1.0f, euler,rotateInfluence);
            //StartCoroutine(rot);
            transform.rotation = Quaternion.Euler(euler);
        };


        //transform.rotation = Quaternion.Euler(euler);


        //Debug.Log(transform.rotation);
    }

    /// <summary>
    /// Invoked by OVRCameraRig's UpdatedAnchors callback. Allows the Hmd rotation to update the facing direction of the player.
    /// </summary>
    public void UpdateTransform(OVRCameraRig rig)
	{
		Transform root = CameraRig.trackingSpace;
		Transform centerEye = CameraRig.centerEyeAnchor;

		if (HmdRotatesY)
		{
			Vector3 prevPos = root.position;
			Quaternion prevRot = root.rotation;

			transform.rotation = Quaternion.Euler(0.0f, centerEye.rotation.eulerAngles.y, 0.0f);

			root.position = prevPos;
			root.rotation = prevRot;
		}

		UpdateController();
	}

	/// <summary>
	/// Jump! Must be enabled manually.
	/// </summary>
	public bool Jump()
	{
		if (!Controller.isGrounded)
			return false;

        MoveThrottle += new Vector3(0, transform.lossyScale.y * JumpForce, 0);

		return true;
	}

	/// <summary>
	/// Stop this instance.
	/// </summary>
	public void Stop()
	{
		Controller.Move(Vector3.zero);
		MoveThrottle = Vector3.zero;
		FallSpeed = 0.0f;
	}

	/// <summary>
	/// Gets the move scale multiplier.
	/// </summary>
	/// <param name="moveScaleMultiplier">Move scale multiplier.</param>
	public void GetMoveScaleMultiplier(ref float moveScaleMultiplier)
	{
		moveScaleMultiplier = MoveScaleMultiplier;
	}

	/// <summary>
	/// Sets the move scale multiplier.
	/// </summary>
	/// <param name="moveScaleMultiplier">Move scale multiplier.</param>
	public void SetMoveScaleMultiplier(float moveScaleMultiplier)
	{
		MoveScaleMultiplier = moveScaleMultiplier;
	}

	/// <summary>
	/// Gets the rotation scale multiplier.
	/// </summary>
	/// <param name="rotationScaleMultiplier">Rotation scale multiplier.</param>
	public void GetRotationScaleMultiplier(ref float rotationScaleMultiplier)
	{
		rotationScaleMultiplier = RotationScaleMultiplier;
	}

	/// <summary>
	/// Sets the rotation scale multiplier.
	/// </summary>
	/// <param name="rotationScaleMultiplier">Rotation scale multiplier.</param>
	public void SetRotationScaleMultiplier(float rotationScaleMultiplier)
	{
		RotationScaleMultiplier = rotationScaleMultiplier;
	}

	/// <summary>
	/// Gets the allow mouse rotation.
	/// </summary>
	/// <param name="skipMouseRotation">Allow mouse rotation.</param>
	public void GetSkipMouseRotation(ref bool skipMouseRotation)
	{
		skipMouseRotation = SkipMouseRotation;
	}

	/// <summary>
	/// Sets the allow mouse rotation.
	/// </summary>
	/// <param name="skipMouseRotation">If set to <c>true</c> allow mouse rotation.</param>
	public void SetSkipMouseRotation(bool skipMouseRotation)
	{
		SkipMouseRotation = skipMouseRotation;
	}

	/// <summary>
	/// Gets the halt update movement.
	/// </summary>
	/// <param name="haltUpdateMovement">Halt update movement.</param>
	public void GetHaltUpdateMovement(ref bool haltUpdateMovement)
	{
		haltUpdateMovement = HaltUpdateMovement;
	}

	/// <summary>
	/// Sets the halt update movement.
	/// </summary>
	/// <param name="haltUpdateMovement">If set to <c>true</c> halt update movement.</param>
	public void SetHaltUpdateMovement(bool haltUpdateMovement)
	{
		HaltUpdateMovement = haltUpdateMovement;
	}

	/// <summary>
	/// Resets the player look rotation when the device orientation is reset.
	/// </summary>
	public void ResetOrientation()
	{
		if (HmdResetsY && !HmdRotatesY)
		{
			Vector3 euler = transform.rotation.eulerAngles;
			euler.y = InitialYRotation;
			transform.rotation = Quaternion.Euler(euler);
		}
	}

    public void SetSnapRotation(bool enabledit)
    {
        SnapRotation = enabledit;
    }

    public void SetSnapTranslation(bool enabledit)
    {
        SnapTranslation = enabledit;
    }

    public void setMoveThrottle(Vector3 value)
    {
        MoveThrottle = value;
    }



}





/*
 
        if (SnapTranslation)
        {

            if (MoveThrottle.x > 0.0) newMove.x = 1.0f;
            else if (MoveThrottle.x < 0.0) newMove.x = -1.0f;
            else newMove.x = 0.0f;


            if (MoveThrottle.y <= 1.0) newMove.y = 1.0f;
            else if (MoveThrottle.y < 0.0) newMove.y = -1.0f;
            else newMove.y = 0.0f;


            if (MoveThrottle.z <= 1.0) newMove.z = 1.0f;
            else if (MoveThrottle.z < 0.0) newMove.z = -1.0f;
            else newMove.z = 0.0f;
            
            MoveThrottle = newMove;
        }
     
     
     
     
     */