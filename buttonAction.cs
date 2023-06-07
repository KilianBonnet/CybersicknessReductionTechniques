using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

public class buttonAction : MonoBehaviour {

    public Button StartButton;
    private Button btn;
    public Text inputTxt;
    public Dropdown groupSelected;
    public Dropdown daySelected;
    // Use this for initialization
    void Start () {
        btn = StartButton.GetComponent<Button>();
        btn.onClick.AddListener(TaskOnClick);
    }
	
	// Update is called once per frame
	void Update () {
       

    }

    void TaskOnClick()
    {
        int valuePlusOne;
        string textprefab;
      if(inputTxt.text != "")
        {
            textprefab = inputTxt.text;
            valuePlusOne = groupSelected.value +1;
            
            PlayerPrefs.SetString("ParticipantNumber",textprefab );
            PlayerPrefs.SetInt("GroupNumber", valuePlusOne);
            valuePlusOne = daySelected.value + 1;
            PlayerPrefs.SetInt("DayNumber", valuePlusOne);
            SceneManager.LoadScene("TunnelingSnapping");
        }
        
    }
}
