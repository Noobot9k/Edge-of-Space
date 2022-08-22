using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    public Vector3 TravelDirection = new Vector3(0, 1, 0);
    public float TravelSpeed = 5;

    public ParticleSystem DestroyedParticleSystem;
    public AudioSource ShotSound;
    GameObject world;

    private void Start() {
        world = GameObject.FindGameObjectWithTag("World");
        ShotSound.transform.SetParent(world.transform);
    }

    private void Update() {
        float deltaTime = Time.deltaTime;
        Vector3 pos = transform.position;
        Vector3 delta = transform.up.normalized; //(transform.up * TravelSpeed * deltaTime);
        Vector3 nextPos = pos + delta;
        bool hit = Physics.Raycast(pos, delta, out RaycastHit hitInfo, delta.magnitude, ~LayerMask.GetMask("Player"));
        if(hit && hitInfo.collider.isTrigger) hit = false;
        if(hit) nextPos = hitInfo.point;

        transform.position = nextPos;
        if(hit) {
            Asteroid otherAsteroid = hitInfo.collider.GetComponent<Asteroid>();
            if(otherAsteroid)
                otherAsteroid.Crumble();
            DestroyedParticleSystem.gameObject.SetActive(true);
            DestroyedParticleSystem.transform.SetParent(world.transform);
            GameObject.Destroy(gameObject);
        }
    }
}
