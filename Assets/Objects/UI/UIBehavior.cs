using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIBehavior : MonoBehaviour {

    public float TransitionLength = 1;
    public float TransitionZoomMultiplier = 10;

    public RawImage TransitionImage;
    public TMP_Text Timer;
    public CameraScript cameraScript;
    public static UIBehavior singleton;

    private void Start() {
        UIBehavior.singleton = this;
    }
    private void Update() {
        float timeLeftInSystem = (StarmapHandler.singleton._tickEnteredSystem + StarmapHandler.singleton.CurrentStarSystem.TimeLimit) - Time.time;
        int minutesLeft = Mathf.FloorToInt(timeLeftInSystem / 60);
        int secondsLeft = Mathf.RoundToInt(timeLeftInSystem % 60);
        string seconds = secondsLeft.ToString();
        if(seconds.Length == 1) seconds += "0";
        Timer.text = minutesLeft.ToString() + ":" + secondsLeft;
    }

    public IEnumerator HideTransitionScreen() {
        TransitionImage.enabled = true;
        float startTick = Time.unscaledTime;
        while(true) {
            float alpha = Mathf.Clamp01((Time.unscaledTime - startTick) / TransitionLength);
            TransitionImage.color = new Color(1,1,1, 1 - alpha);
            cameraScript.DistanceMultiplier = 1 + ((1-LerpEaseOut(alpha)) * TransitionZoomMultiplier);
            yield return new WaitForEndOfFrame();
            if(alpha >= 1) break;
        }
        TransitionImage.enabled = false;
    }
    public IEnumerator ShowTransitionScreen() {
        TransitionImage.enabled = true;
        float startTick = Time.unscaledTime;
        while(true) {
            float alpha = Mathf.Clamp01((Time.unscaledTime - startTick) / TransitionLength);
            TransitionImage.color = new Color(1, 1, 1, alpha);
            cameraScript.DistanceMultiplier = 1 + (LerpEaseIn(alpha) * TransitionZoomMultiplier);
            yield return new WaitForEndOfFrame();
            if(alpha >= 1) break;
        }
    }

    public static float LerpEaseOut(float alpha) {
        return Mathf.Sin(alpha * Mathf.PI * 0.5f);
    }
    public static float LerpEaseIn(float alpha) {
        return 1f - Mathf.Cos(alpha * Mathf.PI * 0.5f);
    }
}
