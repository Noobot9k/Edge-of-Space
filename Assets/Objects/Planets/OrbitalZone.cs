using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitalZone : MonoBehaviour {

    public enum OrbitalZoneType { Refuel, Damage, Heal, AsteroidSpawn };
    public OrbitalZoneType orbitalZoneType = OrbitalZoneType.Damage;
    public Vector2 Range = new Vector2(.75f, 1);
    public float Rate = 5;
    public Color DamageColor = new Color(1, 0, 0, .25f);
    public Color RefuelColor = new Color(1, 1, 0, .25f);
    public Color HealColor = new Color(0, 1, 0, .25f);
    public Color AsteroidColor = new Color(0, 0, 1, .25f);

    public MeshRenderer zoneRenderer;
    public List<Rigidbody> AsteroidPrefabs = new List<Rigidbody>();
    Transform World;

    private void Start() {
        //zoneRenderer = GetComponentInChildren<MeshRenderer>();
        VisualUpdate();
        World = GameObject.FindGameObjectWithTag("World").transform;

        if (orbitalZoneType == OrbitalZoneType.AsteroidSpawn) {
            int numOfAsteroids = Mathf.RoundToInt(transform.lossyScale.x * Range.y);
            for(int i = 0; i < numOfAsteroids; i++) {
                Rigidbody asteroid = Instantiate<Rigidbody>(AsteroidPrefabs[Random.Range(1, AsteroidPrefabs.Count)]);
                asteroid.transform.SetParent(World);
                asteroid.angularVelocity = Random.rotation.eulerAngles / 180;
                asteroid.transform.localScale = Vector3.one * Random.Range(.5f, 1f);
                Vector2 circle = Random.insideUnitCircle.normalized;
                asteroid.transform.position = transform.position + (new Vector3(circle.x, 0, circle.y)/2 * Random.Range(transform.lossyScale.x * Range.x, transform.lossyScale.x * Range.y));
            }
        }
    }

    private void OnTriggerStay(Collider other) {
        PlayerHandler playerHandler = other.GetComponent<PlayerHandler>();
        if (playerHandler) {
            float distance = (other.transform.position - transform.position).magnitude;
            if (distance > Range.x * transform.lossyScale.x/2 && distance < Range.y * transform.lossyScale.x/2)
                TickZoneEffect(playerHandler);
        }
    }

    public void TickZoneEffect(PlayerHandler ship) {
        float deltaTime = Time.fixedDeltaTime;

        Rigidbody rigidbody = ship.GetComponent<Rigidbody>();
        if (!rigidbody) {
            Debug.LogWarning("Player ship doesn't have a rigidbody");
            return;
        }

        if(orbitalZoneType == OrbitalZoneType.Damage) {
            ship.TakeDamage(Rate * deltaTime, true);
        } else if(orbitalZoneType == OrbitalZoneType.Refuel) {
            ship.IncrementFuel(Rate * rigidbody.velocity.magnitude * deltaTime);
        } else if(orbitalZoneType == OrbitalZoneType.Heal) {
            ship.TakeDamage(-Rate * deltaTime);
        }
    }

    public void VisualUpdate() {
        zoneRenderer.material.SetVector("_Range", new Vector4(Range.x, Range.y, 0, 0));

        if(orbitalZoneType == OrbitalZoneType.Damage) {
            zoneRenderer.material.SetColor("_Color", DamageColor);
        } else if(orbitalZoneType == OrbitalZoneType.Refuel) {
            zoneRenderer.material.SetColor("_Color", RefuelColor);
        } else if(orbitalZoneType == OrbitalZoneType.Heal) {
            zoneRenderer.material.SetColor("_Color", HealColor);
        } else if(orbitalZoneType == OrbitalZoneType.AsteroidSpawn) {
            zoneRenderer.material.SetColor("_Color", AsteroidColor);
        }
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, Range.x * transform.lossyScale.x/2);
        Gizmos.DrawWireSphere(transform.position, Range.y * transform.lossyScale.x/2);
    }
}
