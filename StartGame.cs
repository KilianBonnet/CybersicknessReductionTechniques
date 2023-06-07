using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine.SceneManagement;
using System.Text;
using UnityEngine.UI;
using UnityEngine;
using Sigtrap.VrTunnellingPro;
using Random = UnityEngine.Random;

[Obsolete("Please, consider moving the the refactored architecture")]
public class StartGame : MonoBehaviour {
    public GameObject Checkpoints;
    public GameObject SciFiGunLightBlack;

    public Text objectiveTxt;
    public Text trialNumber;
    public GameObject compassobj;
    public GameObject canvas;
    StringBuilder builder;
    StringBuilder builderAvg;

    public Quaternion MissionDirection;
    public Transform Player;
    public Vector3 NorthDirection;
    public OVRPlayerController playerScript;
    private CharacterController characterController;

    private int countCollision= 1;
    private Transform compass;
    private Vector3 targetPosition;
    private GameObject activeChild;
    private Vector2 secondaryAxis;
    public int nauseScore = 0;
    public float avgnauseScore = 0f;
    public bool activeLaser = false;

    public int trialCounter = 0;
    private int conditionCounter = 0;
    private int shuffleNtimes;
    private bool canPress = true;
    public float TimeCounter= 0.0f;
    
    
    private GameObject current_family;

    public float lastTime = 0f;
    public Tunnelling tunnelScript;
    public bool nextTrialReady = false;
    public GameObject Canv, Selectables, ControllerModel;

    // public Randomizer randomizer;
    public int[] finalMatrixIndex; // = new int[] { 0, 1, 2, 3};
    public int[] matriz15 = new int[] { 0, 1, 2, 3 };
    public int[] matriz26 = new int[] { 1, 2, 3, 0 };
    public int[] matriz37 = new int[] { 2, 3, 0, 1 };
    public int[] matriz48 = new int[] { 3, 0, 1, 2 };

    public int[] finaltrialIndex = new int[] { 0, 1, 2 };

    private bool tunnelingIsOn;
    private bool controllersAreUsed;
    public bool activateQuestion = false;

    public int blockCounter = 0; // how many times the experiment will take place

    private float avgTime=0f;
    private float TimeofReturn=0f;
    private float FinalTimeofReturn = 0f;

    private int group_number;
    private int day_number;

    private int TotalConditions = 3;
    //first column: ViewSnapping, second column: TranslationSnapping, third column: Tunneling
    private bool[][] matrixCondition = new bool[][]{
        new bool[] {false,false }, //0
        new bool[] {false,true}, //1
        new bool[] {true,false}, //2
        new bool[] {true,true } //3
        };

    private int[] currentConditions = new int[3];

    private int TotalBlocks = 0;

    void OnTriggerEnter(Collider other){
        if(other.tag =="checkpoint"){
            
            builder.Append(PlayerPrefs.GetString("ParticipantNumber").ToString() + ";");
            builder.Append(PlayerPrefs.GetInt("GroupNumber").ToString() + ";");
            builder.Append(PlayerPrefs.GetInt("DayNumber").ToString() + ";");
            builder.AppendFormat("{0};", blockCounter);
            builder.AppendFormat("{0};", conditionCounter);
            builder.AppendFormat("{0};", finaltrialIndex[trialCounter]);
            builder.AppendFormat("{0};", countCollision);
            builder.AppendFormat("{0};", currentConditions[0]);
            builder.AppendFormat("{0};", currentConditions[1]);
            builder.AppendFormat("{0};", currentConditions[2]);
            builder.AppendFormat("{0:f6};\n", TimeCounter - lastTime);
            avgTime += (TimeCounter - lastTime);
            lastTime = TimeCounter;
            
            other.gameObject.SetActive(false);
            countCollision += 1;

        }
    }

    // Use this for initialization
    void Start () {
        characterController = GetComponent<CharacterController>();
        blockCounter = 0;
       // TotalBlocks = 1;
        builder = new StringBuilder();
        builderAvg = new StringBuilder();
        //Changing the order of matrix and trials
        group_number = PlayerPrefs.GetInt("GroupNumber");
        day_number = PlayerPrefs.GetInt("DayNumber");


        if (group_number == 1 || group_number == 5)
        {
            finalMatrixIndex = matriz15;
            //shuffleArrayNtimes(finalMatrixIndex,0);
        } else if (group_number == 2 || group_number == 6)
        {
            finalMatrixIndex = matriz26;
            // shuffleArrayNtimes(finalMatrixIndex, 1);
        }
        else if (group_number == 3 || group_number == 7)
        {
            finalMatrixIndex = matriz37;
            //shuffleNtimes = 2;
            // shuffleArrayNtimes(finalMatrixIndex, 2);
        }
        else if (group_number == 4 || group_number == 8)
        {
            finalMatrixIndex = matriz48;
            // shuffleNtimes = 3;
            // shuffleArrayNtimes(finalMatrixIndex, 3);
        }
        else
        {
            Debug.Log("Group not found");
            SceneManager.LoadScene("End");
        }


       // shuffleArrayNtimes(finalMatrixIndex, shuffleNtimes);
        print(finalMatrixIndex);
        shuffleArray(finaltrialIndex);
        //


        // Debug.Log(randomizer.finalMatrixIndex.ToString()); 
        conditionCounter = 0;
        //Debug.Log(conditionCounter);
        /// Debug.Log(randomizer.GetfinalMatrixIndexPosition(1));
        /// 
        int value = finalMatrixIndex[conditionCounter];
        playerScript.SetSnapRotation(matrixCondition[value][0]);
      
        playerScript.SetSnapTranslation ( matrixCondition[value][1]);

        if(group_number < 5 && day_number ==1) {
            tunnelingIsOn = true;
        } else if (group_number < 5 && day_number == 2)
        {
            tunnelingIsOn = false;
        }else if (group_number > 4 && day_number == 1)
        {
            tunnelingIsOn = false;
        }
        else
        {
            tunnelingIsOn = true;
        }

           
        // tunnelScript.enabled = matrixCondition[finalMatrixIndex[conditionCounter]][2];

        builder.AppendLine("PID;Group;Day;Block;Condition;Environment;Checkpoint;VS;TS;Tunnel;Time;");
        builderAvg.AppendLine("PID;Group;Day;Block;Condition;Environment;Time(Avg);Timeo of Return;Nause Score;distance;");

        current_family = Checkpoints.transform.GetChild(finaltrialIndex[trialCounter]).gameObject; 
        
        characterController.enabled = false;
        Player.position = current_family.transform.Find("PlayerInit").transform.position;
        characterController.enabled = true;
        
        Debug.Log("Player pos is (1): " + transform.position);
       compass = compassobj.transform;
        playerScript.canMove = false;
        objectiveTxt.text = "Look around and when ready\n press the button!";
        objectiveTxt.color = new Color(255f, 255f, 255f);
        compass.gameObject.SetActive(false);
        filterConditions(conditionCounter);



    }

    void filterConditions(int condition)
    {

        if (matrixCondition[finalMatrixIndex[condition]][0])
        {
            currentConditions[0] = 1;
        }
        else
        {
            currentConditions[0] = 0;
        }
        if (matrixCondition[finalMatrixIndex[condition]][1])
        {
            currentConditions[1] = 1;
        }
        else
        {
            currentConditions[1] = 0;
        }
        if (tunnelingIsOn)
        {
            currentConditions[2] = 1;
        }
        else
        {
            currentConditions[2] = 0;
        }


    }

    // Update is called once per frame
    void Update()
    {
        if(OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick) != Vector2.zero || OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick) != Vector2.zero)
        {
            controllersAreUsed = true;
        }
        else {
            controllersAreUsed = false;
        }

        if(tunnelingIsOn && controllersAreUsed)
        {
            tunnelScript.enabled = true;
        }
        else
        {
            tunnelScript.enabled = false;

        }

        if (tunnelingIsOn && playerScript.SnapRotation && OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick) != Vector2.zero)
        {
            tunnelScript.angularVelocityMax = 0;
            tunnelScript.accelerationMax = 0;
        }
        else
        {
            tunnelScript.angularVelocityMax = 13;
            tunnelScript.accelerationMax = 0.5f;
        }

        /*
        if(tunnelingIsOn && playerScript.SnapTranslation && OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick) != Vector2.zero)
        {
            tunnelScript.accelerationMax = 0;
        }
        else
        {
            tunnelScript.accelerationMax = 0.5f;

        }
        */
        
        /*
        if (matrixCondition[finalMatrixIndex[conditionCounter]][2] && (OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick) != Vector2.zero || OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick) != Vector2.zero))
        {
            tunnelScript.enabled = true;
        }
        else
        {
            tunnelScript.enabled = false;
        }
        */
        trialNumber.text = conditionCounter.ToString() + "/" + TotalConditions.ToString();

        if (OVRInput.GetDown(OVRInput.Button.Four) && !playerScript.canMove )
        {
            playerScript.canMove = true;
            objectiveTxt.color = new Color(255f, 255f, 255f);
            compass.gameObject.SetActive(true);
            lastTime = 0f;
            TimeCounter = 0f;
        }

        

        

        ChangeNorthDirection();
        if (countCollision < current_family.transform.childCount && playerScript.canMove)
        {
            activeChild = current_family.transform.GetChild(countCollision).gameObject;
           
            objectiveTxt.text = "Go to checkpoint " + countCollision;
            activeChild.SetActive(true);
            ChangeMissionDirection();
            canPress = false;

            TimeCounter += Time.deltaTime;



        }
        else if (countCollision >= current_family.transform.childCount)
        {
            objectiveTxt.text = "Go to initial position";
            objectiveTxt.color = new Color(255f, 0.0f, 0.0f);
            compassobj.SetActive(false);
            canPress = true;
            activateQuestion = true;
            TimeofReturn += Time.deltaTime;
        }

        

        if (OVRInput.GetDown(OVRInput.Button.Four) && activateQuestion)
        {
            playerScript.canMove = false;
            Selectables.SetActive(true);
            Canv.SetActive(true);
            FinalTimeofReturn = TimeofReturn;
            canPress = false;
            activeLaser = true;
            SciFiGunLightBlack.SetActive(true);
        } 


        if (nextTrialReady)
        {
            nextTrialReady = false;
            Selectables.SetActive(false);
            Canv.SetActive(false);
            Vector3 initialPosition = current_family.transform.Find("PlayerInit").transform.position;
            float distance = Mathf.Pow(Mathf.Pow(Player.transform.position.x - initialPosition.x, 2f) + Mathf.Pow(Player.transform.position.y - initialPosition.y, 2f) + Mathf.Pow(Player.transform.position.z - initialPosition.z, 2f), 0.5f);
            builderAvg.Append(PlayerPrefs.GetString("ParticipantNumber").ToString() + ";");
            builderAvg.Append(PlayerPrefs.GetInt("GroupNumber").ToString() + ";");
            builderAvg.Append(PlayerPrefs.GetInt("DayNumber").ToString() + ";");
            builderAvg.AppendFormat("{0};", blockCounter);
            builderAvg.AppendFormat("{0};", conditionCounter);
            builderAvg.AppendFormat("{0};", finaltrialIndex[trialCounter]);
            builderAvg.AppendFormat("{0:f6};", avgTime / 9);
            builderAvg.AppendFormat("{0:f6};", FinalTimeofReturn);
            builderAvg.AppendFormat("{0};",nauseScore); //Nausea Score not implemented yet
            builderAvg.AppendLine(distance.ToString() + ";");
            TimeofReturn = 0;
            avgTime = 0;
            FinalTimeofReturn = 0;
            // builder.AppendLine(distance.ToString()+";\n");
            trialCounter += 1;
            //
            if (trialCounter < 3)
            {
                current_family = Checkpoints.transform.GetChild(finaltrialIndex[trialCounter]).gameObject;

                Debug.Log(conditionCounter);
                //  builder.Append("Trial "+trialCounter+";");
            }
            else if (trialCounter >= 3 && conditionCounter < TotalConditions)
            {
                conditionCounter += 1;
                trialCounter = 0;
                print(finalMatrixIndex[conditionCounter]);
                   
                current_family = Checkpoints.transform.GetChild(finaltrialIndex[trialCounter]).gameObject;
                int value = finalMatrixIndex[conditionCounter];
                
                playerScript.SnapRotation = matrixCondition[value][0];
                playerScript.SnapTranslation = matrixCondition[value][1];

               

                // current_family = null;
                //CreateText();
                //SceneManager.LoadScene("End");
            }
            else if (trialCounter >= 3 && conditionCounter >= TotalConditions)
            {
                //CreateText();   
                blockCounter += 1;
                print(TotalBlocks);
                Debug.Log(blockCounter);
                if (blockCounter > TotalBlocks)
                {
                    CreateText();
                    SceneManager.LoadScene("End");
                }
                else if(blockCounter <= TotalBlocks)
                {
                    
                    //builder = new StringBuilder();
                    // builderAvg = new StringBuilder();
                    trialCounter = 0;
                    conditionCounter = 0;
                    countCollision = 1;
                    avgTime = 0f;
                    TimeofReturn = 0f;
                    FinalTimeofReturn = 0f;
                    nauseScore = 0;
                    avgnauseScore = 0f;
                    activeLaser = false;
                   
                    playerScript.SetSnapRotation(matrixCondition[finalMatrixIndex[conditionCounter]][0]);

                    playerScript.SetSnapTranslation(matrixCondition[finalMatrixIndex[conditionCounter]][1]);

                        if (group_number < 5 && day_number == 1)
                        {
                            tunnelingIsOn = true;
                        }
                        else if (group_number < 5 && day_number == 2)
                        {
                            tunnelingIsOn = false;
                        }
                        else if (group_number > 4 && day_number == 1)
                        {
                            tunnelingIsOn = false;
                        }
                        else
                        {
                            tunnelingIsOn = true;
                        }


                  //  builder.AppendLine("PID;Group;Day;Block;Condition;Environment;Checkpoint;VS;TS;Tunnel;Time;");
                   // builderAvg.AppendLine("PID;Group;Day;Block;Condition;Environment;Time(Avg);Timeo of Return;Nause Score;distance;");

                    current_family = Checkpoints.transform.GetChild(finaltrialIndex[trialCounter]).gameObject;

                    playerScript.enabled = false;
                    Player.position = current_family.transform.Find("PlayerInit").transform.position;
                    playerScript.enabled = true;

                    Debug.Log("Player pos is (2): " + transform.position);
                    compass = compassobj.transform;
                    playerScript.canMove = false;
                    objectiveTxt.text = "Look around and when ready\n press the button!";
                    objectiveTxt.color = new Color(255f, 255f, 255f);
                    compass.gameObject.SetActive(false);
                    filterConditions(conditionCounter);
                    activateQuestion = false;


                }
            }
            characterController.enabled = false;
            Player.position = current_family.transform.Find("PlayerInit").transform.position;
            characterController.enabled = true;

            Debug.Log("Player pos is (3): " + transform.position);
            compass = compassobj.transform;
            playerScript.canMove = false;
            objectiveTxt.text = "Look around and when ready\n press the button!";
            objectiveTxt.color = new Color(255f, 255f, 255f);
            compass.gameObject.SetActive(false);
            filterConditions(conditionCounter);

            countCollision = 1;
            compassobj.SetActive(false);
            canPress = false;
            activeLaser = false;
	         SciFiGunLightBlack.SetActive(false);

            nextTrialReady = false;
            activateQuestion = false;

        }


        

       
        /*
        if (trialCounter >= Checkpoints.transform.childCount )
        {
            objectiveTxt.text = "Thank you for your participation";
            objectiveTxt.color = new Color(255f, 255f, 255f);
        }*/

    }       

        void CreateText()
        {
     
        string filename;
        if(PlayerPrefs.GetString("ParticipantNumber") != "")
        {
            filename = PlayerPrefs.GetString("ParticipantNumber");

        }
        else
        {
            filename = "GenericFile";
        }
       // Debug.Log(filename);
        using (System.IO.StreamWriter writer = new System.IO.StreamWriter(Application.dataPath + "/Data/P"+ filename +".txt"))
        {
            writer.Write(builder);
        }
        using (System.IO.StreamWriter writer = new System.IO.StreamWriter(Application.dataPath + "/Data/P" + filename + "Avg.txt"))
        {
            writer.Write(builderAvg);
        }


    }


    public void ChangeMissionDirection()
    {
        targetPosition = activeChild.transform.position;
        Vector3 dir = compass.transform.position - targetPosition;
        MissionDirection = Quaternion.LookRotation(dir);

        MissionDirection.z = -MissionDirection.y;
        MissionDirection.x = 0;
        MissionDirection.y = 0;

        compass.localRotation =  MissionDirection * Quaternion.Euler(NorthDirection);


    }


    public void ChangeNorthDirection()
    {
        NorthDirection.z = Player.eulerAngles.y;
        
    }

    public static void shuffleArray(int[] arr)
    {
        int lenght = arr.Length;

        for (int i = 0; i < lenght; i++)
        {
            swap(arr, i, Random.Range(0, lenght - 1));

        }

    }

    public static void shuffleArrayNtimes(int[] arr, int ntimes)
    {
        int lenght = arr.Length;

        for (int j = 0; j < ntimes; j++)
        {

            for (int i = 0; i < lenght-1; i++)
            {
                swap(arr, i, i + 1);

            }
        }
        
    }

    public static void swap(int[] arr, int a, int b)
    {
        int temp = arr[a];
        arr[a] = arr[b];
        arr[b] = temp;
    }

}
