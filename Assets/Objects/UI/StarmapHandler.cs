using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class StarmapHandler : MonoBehaviour {

    public int StarmapSeed = -1;
    public bool StarmapOpen = false;
    public bool CanOpenStarmap = true;
    public bool CanInteractWithStarmap = true;
    public float FuelToLYValue = 14;
    string selectionObjText;
    string starmapStatusText;
    public float _tickEnteredSystem = 0;

    public Volume GlobalVolume;
    public PlayerHandler Player;
    public Star CurrentStarSystem;
    public Transform World;
    public Camera StarmapCamera;
    public Camera MainCamera;
    public RectTransform SelectionObj;
    public TMP_Text StarmapStatusObj;
    public Transform StarmapWave;
    public LineRenderer Line;
    public AudioSource JumpSoundFX;
    public static StarmapHandler singleton;

    public Planet PlanetPrefab;
    public Transform SystemStarPrefab;
    public Transform SpaceStationPrefab;


    private IEnumerator Start() {
        StarmapHandler.singleton = this;
        if(StarmapSeed == -1) StarmapSeed = Random.Range(-99999999, 99999999);
        SelectionObj.gameObject.SetActive(false);

        TMP_Text textObj = SelectionObj.GetComponentInChildren<TMP_Text>();
        if(textObj)
            selectionObjText = textObj.text;

        starmapStatusText = StarmapStatusObj.text;

        PlayerData data = SaveManager.LoadPlayerData(Player);
        if(data != null) {
            print("loading saved data...");
            StarmapSeed = data.SystemSeed;
            yield return new WaitForEndOfFrame();
            Random.InitState(StarmapSeed);
            StartCoroutine(GoToStarSystem(data.StarsystemIndex, false, true));
            DialogBox.singleton.RunDialogBox("Save data has been loaded.");
        } else {
            print("no save data found. Starting new game");
            yield return new WaitForEndOfFrame();
            Random.InitState(StarmapSeed);
            if(CurrentStarSystem) StartCoroutine(GoToStarSystem(CurrentStarSystem, false, true));
            else Debug.LogWarning("currentStarSystem was null");
        }
    }

    private void Update() {
        if(!StarmapOpen) return;
        if(!CanInteractWithStarmap) return;

        Ray ray = StarmapCamera.ScreenPointToRay(Input.mousePosition);
        bool hit = Physics.Raycast(ray, out RaycastHit hitInfo, 9999, LayerMask.GetMask("Starmap"));
        Line.gameObject.SetActive(hit);
        if(hit) {
            Star StarObj = hitInfo.collider.GetComponent<Star>();
            if(StarObj) {
                SelectionObj.gameObject.SetActive(true);
                SelectionObj.anchoredPosition = StarmapCamera.WorldToScreenPoint(hitInfo.collider.transform.position);
                TMP_Text textObj = SelectionObj.GetComponentInChildren<TMP_Text>();
                if(textObj) {
                    string threatLevel = "<color=#00FF89ff>{0}</color>";
                    if(StarObj.PlanetCount >= 8) threatLevel = "<color=#FF0000ff>{0}</color>";
                    else if(StarObj.PlanetCount >= 4) threatLevel = "<color=#FFFF00ff>{0}</color>";
                    threatLevel = string.Format(threatLevel, StarObj.PlanetCount);
                    float distance = (CurrentStarSystem.transform.position - StarObj.transform.position).magnitude;
                    string distanceString = HUD.RoundToTenths(distance).ToString();
                    textObj.text = string.Format(selectionObjText, StarObj.DisplayName, threatLevel, distanceString);
                }

                if(Input.GetButtonDown("Fire1")) {
                    StartCoroutine(GoToStarSystem(StarObj, true));
                }

                Line.transform.position = StarObj.transform.position;
                Line.SetPosition(1, Line.transform.InverseTransformPoint( CurrentStarSystem.transform.position));
            }
        }
        CameraScript starCamScript = StarmapCamera.GetComponent<CameraScript>();
        starCamScript.CameraSubject = CurrentStarSystem.transform;

        float range = Player.Fuel / FuelToLYValue;
        string rangeString = HUD.RoundToTenths(range * .95f) + " - " + HUD.RoundToTenths(range * 1.05f);
        object[] argsArray = new object[] { HUD.RoundToTenths(Player.Fuel), Player.MaxFuel, rangeString };
        StarmapStatusObj.text = string.Format(starmapStatusText, argsArray);

        if (Time.time > _tickEnteredSystem + CurrentStarSystem.TimeLimit) {
            StartCoroutine(Died("Impending doom reached the system you occupied. You died."));
        }
    }

    IEnumerator TransitionDistortion(bool In) {
        LensDistortion distortionModifier;
        if(GlobalVolume.profile.TryGet<LensDistortion>(out distortionModifier)) {
            float duration = 3;
            float startTick = Time.unscaledTime;
            while(Time.unscaledTime < startTick + duration) {
                float alpha = ((startTick + duration) - Time.unscaledTime) / duration;
                if(In) alpha = 1 - alpha;

                distortionModifier.intensity.value = alpha;
                yield return new WaitForEndOfFrame();
            }
        }
    }
    IEnumerator TweenPlayer(Vector3 startPos, Vector3 destinationPos, bool FadeIn) {
        Rigidbody playerRB = Player.GetComponent<Rigidbody>();
        playerRB.isKinematic = true;
        //Player.transform.LookAt(destinationPos);

        float duration = 3;
        float startTick = Time.unscaledTime;
        while(Time.unscaledTime < startTick + duration) {
            float alpha = ((startTick + duration) - Time.unscaledTime) / duration;
            float EasedAlpha = UIBehavior.LerpEaseIn(1 - alpha);
            if (!FadeIn) EasedAlpha = UIBehavior.LerpEaseOut(1 - alpha);
            Player.transform.position = Vector3.Lerp(startPos, destinationPos, EasedAlpha);

            yield return new WaitForEndOfFrame();
        }
        playerRB.isKinematic = false;
    }
    public IEnumerator StartSystemJumpAnimation() {
        CanOpenStarmap = false;
        yield return HideStarmap();
        StartCoroutine(TransitionDistortion(true));

        Vector3 startPos = Player.transform.position;
        Vector3 destinationPos = startPos + Vector3.one * 100;
        yield return TweenPlayer(startPos, destinationPos, true);
    }
    public IEnumerator FinishSystemJumpAnimation() {
        StartCoroutine(TransitionDistortion(false));

        Vector3 destinationPos = Player.transform.position;
        Vector3 startPos = destinationPos - Vector3.one * 100;
        yield return TweenPlayer(startPos, destinationPos, false);
        CanOpenStarmap = true;
    }
    public int GetStarIndex(Star starsystem) {
        return starsystem.transform.GetSiblingIndex();
    }
    public Star GetStarByIndex(int starsystemIndex) {
        Transform starsystemTrans = transform.GetChild(starsystemIndex);
        if(!starsystemTrans) {
            Debug.LogWarning(starsystemIndex.ToString() + " is not a valid star index.");
            return null;
        }
        Star starsystem = starsystemTrans.GetComponent<Star>();
        if(!starsystem) {
            Debug.LogWarning("star of index " + starsystemIndex.ToString() + " does not have a star script attached. Probably the camera.");
            return null;
        }
        return starsystem;
    }
    public IEnumerator GoToStarSystem(int starsystemIndex, bool ShowAnimation = true, bool free = false) {
        Star starsystem = GetStarByIndex(starsystemIndex);
        if (!starsystem) {
            Debug.LogWarning(starsystemIndex.ToString() + " is not a valid starsystem index.");
            yield break;
        }
        yield return GoToStarSystem(starsystem, ShowAnimation, free);
    }
    public IEnumerator GoToStarSystem(Star starsystem, bool ShowAnimation = true, bool free = false) {
        print("going to " + starsystem.DisplayName);
        if(!CanInteractWithStarmap) { print("Cannot interact with starmap."); yield break; }
        if(Player.Fuel <= 0 && !free) {
            HUD.singleton.DisplayMsg("You don't have enough fuel to jump.", Color.yellow, 10);
            print("Not enough fuel");
            yield break;
        }
        if(starsystem == CurrentStarSystem && !free) yield break;

        if(ShowAnimation) {
            CoroutineWithData coroutineData = new CoroutineWithData(this, DialogBox.singleton.RunDialogBox("Do you think you have enough fuel to make it to this system?\n(You will die if you don't)", true));
            CanInteractWithStarmap = false;
            yield return coroutineData.coroutine;
            CanInteractWithStarmap = true;
            if(!(bool)coroutineData.result) yield break;

            JumpSoundFX.Play();
            yield return StartSystemJumpAnimation();
        }

        World.transform.ClearAllChildren();
        _tickEnteredSystem = Time.time;

        if(!free) {
            float jumpDistance = (starsystem.transform.position - CurrentStarSystem.transform.position).magnitude;
            if(Player.Fuel / FuelToLYValue < jumpDistance) {
                Player.transform.position = Vector3.zero;
                Player.Fuel = 0;
                if(ShowAnimation) yield return FinishSystemJumpAnimation();
                //HUD.singleton.DisplayMsg("You did not have enough fuel to reach your destination and are stranded. You starved to death.", Color.red, 30);
                yield return Died("You did not have enough fuel to reach your destination and got stranded. You starved to death instantly.");
                yield break;
            }
            Player.IncrementFuel(-jumpDistance * FuelToLYValue);
        }

        //if(!free) SetStarmapWavePosition(CurrentStarSystem.transform.position.x + 2.5f);
        //else
        SetStarmapWavePosition(starsystem.transform.position.x - 5f);

        // generate new system
        CurrentStarSystem = starsystem;
        Random.InitState(starsystem.Seed);
        int numberOfPlanets = starsystem.PlanetCount; //Random.Range(1, 12);
        float minMapScale = 128;
        float mapScale = Mathf.Max(numberOfPlanets * 60, minMapScale);
        for(int i = 0; i < numberOfPlanets; i++) {
            Planet newPlanet = Instantiate<Planet>(PlanetPrefab);
            newPlanet.transform.SetParent(World, false);
            newPlanet.SetVisualForm(Random.Range(1, newPlanet.visualForms.Count));
            newPlanet.transform.localScale = Vector3.one * Random.Range(10, 35);
            newPlanet.transform.position = Vector3.Scale( RandomVector3(mapScale, Mathf.Max(mapScale * .5f, minMapScale)), new Vector3(1,0,1));
        }
        Spinbot worldSpin = World.GetComponent<Spinbot>();
        //if(worldSpin) worldSpin.axisMagnitude = Vector3.up * numberOfPlanets / 7.5f;
        // generate star
        Transform newStar = Instantiate<Transform>(SystemStarPrefab);
        newStar.SetParent(World);
        newStar.transform.localScale = Vector3.one * Mathf.Max(Random.Range(mapScale / 20, mapScale / 10), 50);
        newStar.transform.position = Vector3.zero;

        // generate space station
        if(starsystem.HasStation) {
            float stationDistance = Mathf.Max(50, mapScale / 10) + 32;
            Transform newStation = Instantiate<Transform>(SpaceStationPrefab);
            newStation.SetParent(World);
            newStation.position = RandomVector3(stationDistance, stationDistance);
            newStation.rotation = Quaternion.Euler(0, Random.Range(-180, 180), 0);

            //Player.transform.position = newStation.position + (newStation.right * -6);
        }

        // position player
        float spawnDistance = Mathf.Max(50, mapScale / 10) + 24;
        Player.transform.position = RandomVector3(spawnDistance, spawnDistance);

        yield return new WaitForEndOfFrame();

        MiniMap.singleton.RegenerateMiniMap(mapScale);

        if(ShowAnimation) yield return FinishSystemJumpAnimation();

    }

    public void DestroyStarsAcrossBoarder(float x) {
        foreach(Transform starsystemTrans in transform) {
            Star starsystem = starsystemTrans.GetComponent<Star>();
            if(starsystem && starsystemTrans.position.x < x)
                starsystem.gameObject.SetActive(false);
                //GameObject.Destroy(starsystemTrans.gameObject);
        }
    }
    public void SetStarmapWavePosition(float x) {
        DestroyStarsAcrossBoarder(x);
        StarmapWave.position = Vector3.Scale(StarmapWave.position, new Vector3(0, 1, 1)) + Vector3.right * x;
    }

    public Vector3 RandomVector3(float maxMagnitude, float minMagnitude = 0) {
        Vector2 circle = Random.insideUnitCircle.normalized;
        return new Vector3(circle.x, 0, circle.y) * Random.Range(minMagnitude, maxMagnitude);
    }

    public IEnumerator ShowStarmap() {
        SelectionObj.gameObject.SetActive(false);
        if(!CanOpenStarmap) yield break;
        if(Player.Fuel <= 0) {
            HUD.singleton.DisplayMsg("You don't have enough fuel to jump.", Color.yellow, 10);
            yield break;
        }
        yield return UIBehavior.singleton.ShowTransitionScreen();
        MainCamera.enabled = false;
        StarmapCamera.enabled = true;
        StarmapOpen = true;
        yield return UIBehavior.singleton.HideTransitionScreen();
    }
    public IEnumerator HideStarmap() {
        yield return UIBehavior.singleton.ShowTransitionScreen();
        MainCamera.enabled = true;
        StarmapCamera.enabled = false;
        StarmapOpen = false;
        yield return UIBehavior.singleton.HideTransitionScreen();
    }
    public void CancelStarmap() {
        StartCoroutine(HideStarmap());
    }

    public IEnumerator Died(string deathMsg) {
        //Rigidbody playerRB = Player.GetComponent<Rigidbody>();
        //if(playerRB) playerRB.isKinematic = true;
        UserInput playerInput = Player.GetComponent<UserInput>();
        if(playerInput) playerInput.enabled = false;

        yield return DialogBox.singleton.RunDialogBox(deathMsg);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
