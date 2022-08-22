using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {

    public Transform CameraSubject;
    [Range(0, 1)]
    public float CameraLeadMultiplier = .25f;
    public float CameraLeadSmoothing = 1;
    public float Distance = 25;
    public float DistanceMultiplier = 1;
    public Quaternion RotationOffset = Quaternion.Euler(60, 90, 0);
    Vector3 positionOffset = Vector3.zero;

    void Start() {

    }
    void LateUpdate() {
        float deltaTime = Time.smoothDeltaTime;

        if(!CameraSubject) return;

        Rigidbody subjectRigidbody = CameraSubject.GetComponent<Rigidbody>();
        Vector3 targetPosition = CameraSubject.position;
        if(subjectRigidbody) {
            positionOffset = Vector3.Lerp(positionOffset, subjectRigidbody.velocity * CameraLeadMultiplier, CameraLeadSmoothing * deltaTime);
            targetPosition += positionOffset;
        } else positionOffset = Vector3.zero;

        transform.rotation = RotationOffset;
        transform.position = targetPosition + transform.forward * -Distance * DistanceMultiplier;
    }
}
