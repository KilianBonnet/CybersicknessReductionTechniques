using UnityEngine;

public class Compass : MonoBehaviour {
    [SerializeField] 
    private Transform playerTransform;

    [HideInInspector]
    public Vector3 target;

    private void Update() {
        Vector3 northDirection = Vector3.zero;
        Vector3 dir = transform.position - target;

        northDirection.z = playerTransform.eulerAngles.y;
        Quaternion missionDirection = Quaternion.LookRotation(dir);

        missionDirection.z = -missionDirection.y;
        missionDirection.x = 0;
        missionDirection.y = 0;

        transform.localRotation = missionDirection * Quaternion.Euler(northDirection);
    }

}
