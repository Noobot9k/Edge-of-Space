using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipMovment : MonoBehaviour {

    // configuration
    public float Steer = 0;
    public float Throttle = 0;
    public float SteeringTorque = 10;
    public float Acceleration = 10;

    // references
    public ParticleSystem ThrustParticleSystem;
    public AudioSource ThrustSoundFX;
    new Rigidbody rigidbody;

    private void Start() {
        rigidbody = GetComponent<Rigidbody>();
    }
    void FixedUpdate() {
        rigidbody.AddForce(-transform.right * Throttle * Acceleration, ForceMode.Acceleration);
        rigidbody.AddTorque(new Vector3(0, Steer * SteeringTorque, 0), ForceMode.Acceleration);

        ThrustSoundFX.volume = Mathf.Abs(Throttle) * .2f;
        //if(Mathf.Abs(Throttle) > .25f && !ThrustSoundFX.isPlaying) {
        //    ThrustSoundFX.Play();
        //} else ThrustSoundFX.Stop();
        if(Throttle > .25f && ThrustParticleSystem)
            ThrustParticleSystem.Play();
        else
            ThrustParticleSystem.Stop();
    }
}
