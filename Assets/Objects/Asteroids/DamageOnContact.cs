using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOnContact : MonoBehaviour {

    public float CollisionPushForce = 10;
    public float CollisionDamage = 10;
    new Rigidbody rigidbody;

    private void Start() {
        rigidbody = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision) {
        PlayerHandler playerHandler = collision.transform.GetComponent<PlayerHandler>();
        if(!playerHandler) return;
        Rigidbody playerRB = playerHandler.GetComponent<Rigidbody>();
        if(!playerRB) return;

        Vector3 toPlayerVector = (playerHandler.transform.position - transform.position);
        playerRB.AddForce(toPlayerVector.normalized * CollisionPushForce, ForceMode.VelocityChange);
        if (rigidbody) rigidbody.AddForce(-toPlayerVector.normalized * CollisionPushForce);

        playerHandler.TakeDamage(CollisionDamage);

        Asteroid asteroid = GetComponent<Asteroid>();
        if(asteroid)
            asteroid.Crumble();
    }

}
