using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour {

    public float MetalValue = 5;
    public float HealthValue = 0.5f;
    public bool PlayerGravity = true;
    public float PlayerGravityRange = 10;
    public float PlayerGravityMultiplier = 10;

    new Rigidbody rigidbody;

    private void OnCollisionEnter(Collision collision) {
        PlayerHandler ship = collision.gameObject.GetComponent<PlayerHandler>();
        if(!ship) return;

        ship.TryIncrementMetal(MetalValue);
        ship.TakeDamage(-HealthValue);
        GameObject.Destroy(gameObject);
    }

    private void Start() {
        rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate() {
        if(!PlayerGravity) return;

        Vector3 targetPos = PlayerHandler.singleton.transform.position;
        Vector3 toPlayerVector = (targetPos - transform.position);
        if(toPlayerVector.magnitude > PlayerGravityRange) return;
        rigidbody.AddForce(toPlayerVector.normalized * PlayerGravityMultiplier, ForceMode.Acceleration);
    }

}
