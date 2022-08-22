using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour {

    public float MetalPickupSpawnSpeed = 5;

    public Rigidbody MetalPickupPrefab;
    Transform World;

    private void Start() {
        World = GameObject.FindGameObjectWithTag("World").transform;
    }

    public void Crumble() {
        for(int i = 0; i < Random.Range(1, 3); i++) {
            Rigidbody metalPickup = Instantiate<Rigidbody>(MetalPickupPrefab);
            metalPickup.transform.SetParent(World);
            metalPickup.transform.position = transform.position;
            metalPickup.velocity = Vector3.Scale(Random.insideUnitSphere, new Vector3(1, 0, 1)) * MetalPickupSpawnSpeed;
        }
        GameObject.Destroy(gameObject);
    }
}
