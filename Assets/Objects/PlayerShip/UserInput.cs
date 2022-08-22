using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInput : MonoBehaviour {

    ShipMovment controller;
    PlayerHandler handler;
    new public Camera camera;

    void Start() {
        controller = GetComponent<ShipMovment>();
        handler = GetComponent<PlayerHandler>();
    }
    void Update() {
        if(StarmapHandler.singleton.StarmapOpen) return;

        if(Input.GetButtonDown("OpenStarMap")) {
            StartCoroutine(StarmapHandler.singleton.ShowStarmap());
        } else if(Input.GetButtonUp("OpenStarMap")) {
            //StartCoroutine(StarmapHandler.singleton.HideStarmap());
        }

        controller.Steer = Input.GetAxisRaw("Horizontal");
        controller.Throttle = Input.GetAxis("Vertical");

        Ray mouseOrigin = camera.ScreenPointToRay(Input.mousePosition);
        bool hit = LinePlaneIntersection(out Vector3 intersection, mouseOrigin.origin, mouseOrigin.direction, Vector3.zero, Vector3.up);
        if(hit)
            handler.LookVector = (intersection - transform.position);
            //handler.Shoot((intersection - transform.position));
        else
            Debug.LogWarning("Mouse is pointed at the sky or something"); //handler.Shoot(Vector3.forward); //

        handler.IsShooting = Input.GetButton("Fire1");
        handler.ShootThisFrame = Input.GetButtonDown("Fire1");

    }

    private void OnDisable() {
        controller.Steer = 0;
        controller.Throttle = 0;
    }

    // yoinked from https://stackoverflow.com/questions/34892197/how-calc-intersection-plane-and-line-unity3d
    public static bool LinePlaneIntersection(out Vector3 intersection, Vector3 linePoint, Vector3 lineDirection, Vector3 planePoint, Vector3 planeNormal) {
        float length;
        float dotNumerator;
        float dotDenominator;
        Vector3 vector;
        intersection = Vector3.zero;

        //calculate the distance between the linePoint and the line-plane intersection point
        dotNumerator = Vector3.Dot((planePoint - linePoint), planeNormal);
        dotDenominator = Vector3.Dot(lineDirection, planeNormal);

        if(dotDenominator != 0.0f) {
            length = dotNumerator / dotDenominator;

            vector = lineDirection.normalized * length;

            intersection = linePoint + vector;

            return true;
        } else
            return false;
    }
}
