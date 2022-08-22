using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMap : MonoBehaviour {

    public float MapScale = 100;

    public RectTransform Container;
    public GameObject World;

    public MiniMapIcon PlanetIconPrefab;
    public MiniMapIcon StarIconPrefab;
    public MiniMapIcon AsteroidIconPrefab;
    public MiniMapIcon SatelliteIconPrefab;
    public MiniMapIcon ShipIconPrefab;

    public static MiniMap singleton;

    private void Start() {
        MiniMap.singleton = this;
        RegenerateMiniMap(0);
    }

    public void RegenerateMiniMap(float scaleInWorldUnits) {
        scaleInWorldUnits += 64;
        scaleInWorldUnits *= 2; // arrives as radius, convert to diameter.
        //print("Regenerating Minimap..." + scaleInWorldUnits.ToString());
        MapScale = Mathf.Max(scaleInWorldUnits * 1.1f, 250); 
        Container.ClearAllChildren();

        foreach(Transform body in World.transform) {
            MiniMapIcon icon;
            if(body.GetComponent<Planet>())
                icon = Instantiate<MiniMapIcon>(PlanetIconPrefab);
            else if(body.CompareTag("Star"))
                icon = Instantiate<MiniMapIcon>(StarIconPrefab);
            else if (body.GetComponent<Asteroid>())
                icon = Instantiate<MiniMapIcon>(AsteroidIconPrefab);
            else //if (body.CompareTag("Satellite"))
                icon = Instantiate<MiniMapIcon>(SatelliteIconPrefab);

            icon.Subject = body;
            RectTransform rectTransform = icon.GetComponent<RectTransform>();
            rectTransform.SetParent(Container, false);
            rectTransform.anchoredPosition = new Vector2(body.position.x, body.position.z) * Container.rect.height / MapScale;
        }

        GameObject playerShipObj = GameObject.FindGameObjectWithTag("Player");
        if(playerShipObj) {
            MiniMapIcon shipIcon = Instantiate<MiniMapIcon>(ShipIconPrefab);
            shipIcon.Subject = playerShipObj.transform;
            RectTransform shipRectTransform = shipIcon.GetComponent<RectTransform>();
            shipRectTransform.SetParent(Container, false);
            shipRectTransform.anchoredPosition = Vector2.zero;
        } else Debug.LogWarning("unable to find player ship by tag!");
    }

}
