using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour {

    public string DisplayName = null;
    public bool HasStation = false;
    public bool IsVictoryStation = false;
    public int PlanetCount = -1;
    public float TimeLimit = 5 * 60;
    public int Seed = -1;

    const string randomGlyphs = "abcdefghijklmnopqrstuvwxyz0123456789_.";

    public SpriteRenderer CurrentLocationIcon;
    public MeshRenderer SatelliteIcon;

    void Start() {
        if(DisplayName == null || DisplayName == "") DisplayName = generateRandomString(4, 12);
        if(Seed == -1) Seed = Random.Range(-99999999, 99999999);
        if(PlanetCount == -1) PlanetCount = Random.Range(1, 12);
    }
    void Update() {
        SatelliteIcon.enabled = HasStation;
        CurrentLocationIcon.enabled = StarmapHandler.singleton.CurrentStarSystem == this;
    }

    string generateRandomString(int minCharAmount, int maxCharAmount) {
        string myString = "";
        int charAmount = Random.Range(minCharAmount, maxCharAmount); //set those to the minimum and maximum length of your string
        for(int i = 0; i < charAmount; i++) {
            myString += randomGlyphs[Random.Range(0, randomGlyphs.Length)];
        }
        return myString;
    }
}
