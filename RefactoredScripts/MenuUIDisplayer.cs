using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuUIDisplayer : MonoBehaviour {
    [SerializeField] private GameObject standaloneUi;
    [SerializeField] private GameObject desktopUi;

    private void Start() {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        desktopUi.SetActive(true);
        standaloneUi.SetActive(false);
#elif UNITY_ANDROID
        desktopUi.SetActive(false);
        standaloneUi.SetActive(true);
#endif
    }
}
