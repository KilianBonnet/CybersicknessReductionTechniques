using System;
using TMPro;
using UnityEngine;

internal enum DirectionMethod {
    COMPASS,
    TRAIL
}

public class DirectionManager : MonoBehaviour {
    [Header("UI")] [SerializeField] private TextMeshProUGUI toggleInfoUi;
    
    [Header("Tools")]
    [SerializeField] private Trail trail;
    [SerializeField] private Compass compass;
    
    [Header("Parameters")]
    [SerializeField] private DirectionMethod currentDirectionMethod;
    [SerializeField] private KeyCode switchMethodKey;

    private Vector3 objective;

    private bool isDisplaying;

    private void Start() {
        UpdateUiInfo();
    }

    private void Update() {
        if (!Input.GetKeyDown(switchMethodKey) || !isDisplaying) return;
        HideDirections();
        currentDirectionMethod = currentDirectionMethod == DirectionMethod.TRAIL
            ? DirectionMethod.COMPASS
            : DirectionMethod.TRAIL;
        ShowDirections();
        UpdateUiInfo();
    }


    public void HideDirections() {
        isDisplaying = false;
        compass.gameObject.SetActive(false);
        trail.StopTrail();
        toggleInfoUi.gameObject.SetActive(false);
    }

    public void ShowDirections() {
        switch (currentDirectionMethod) {
            case DirectionMethod.COMPASS:
                compass.gameObject.SetActive(true);
                compass.target = objective;
                return;
            case DirectionMethod.TRAIL:
                trail.DrawTrail(objective);
                return;
        }
    }

    public void UpdateObjective(Vector3 newObjective) {
        isDisplaying = true;
        objective = newObjective;
        ShowDirections();
    }

    private void UpdateUiInfo() {
        toggleInfoUi.gameObject.SetActive(true);
        DirectionMethod methodToToggle  = currentDirectionMethod == DirectionMethod.TRAIL ? DirectionMethod.COMPASS : DirectionMethod.TRAIL;
        toggleInfoUi.text = $"Press \"{switchMethodKey}\" to toggle {methodToToggle}";
    }
}
