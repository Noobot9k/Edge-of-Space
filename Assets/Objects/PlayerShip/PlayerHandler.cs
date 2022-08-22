using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandler : MonoBehaviour {

    public float Fuel = 100;
    public float MaxFuel = 100;
    public float Health = 100;
    public float MaxHealth = 100;
    public float FireRate = 4;
    public bool IsFullAuto = false;
    public float Metal = 0;
    public int CurrentUpgradeIndex = 0;

    public bool IsShooting = false;
    public bool ShootThisFrame = false;
    public Vector3 LookVector = Vector3.forward;

    float _lastShotFired = -100;

    public List<Upgrade> Upgrades = new List<Upgrade>();

    public AudioSource CollisionSoundFX;
    public AudioSource CollectionSoundFX;
    public Projectile LazerPrefab;
    public static PlayerHandler singleton;
    GameObject world;

    public void TakeDamage(float damage, bool IsHeatDamage = false) {
        if(damage > 0) {
            if(IsHeatDamage) HUD.singleton.DisplayMsg(0);
            else CollisionSoundFX.Play();
        } else {
            HUD.singleton.DisplayMsg(2);
        }

        Health -= damage;
        if(Health <= 0) {
            Health = 0;
            StartCoroutine(StarmapHandler.singleton.Died("Your ship was damaged beyond repair. You have died."));
        } else if(Health >= MaxHealth) Health = MaxHealth;
    }

    public void IncrementFuel(float deltaFuel) {
        HUD.singleton.DisplayMsg(1);
        Fuel += deltaFuel;
        Fuel = Mathf.Clamp(Fuel, 0, MaxFuel);
    }
    public bool TryIncrementMetal(float deltaMetal) {
        if(Metal + deltaMetal >= 0) {
            Metal += deltaMetal;

            if (deltaMetal > 0) {
                CollectionSoundFX.Play();
            }

            if(Upgrades.Count > CurrentUpgradeIndex + 1) {
                if(Metal > Upgrades[CurrentUpgradeIndex + 1].MetalCost) {
                    CurrentUpgradeIndex++;
                    Upgrade newUpgrade = Upgrades[CurrentUpgradeIndex];
                    Metal -= newUpgrade.MetalCost;
                    MaxFuel = newUpgrade.NewMaxFuel;
                    MaxHealth = newUpgrade.NewMaxHealth;
                    FireRate = newUpgrade.NewFireRate;
                    IsFullAuto = newUpgrade.IsFullAuto;

                    StartCoroutine(DialogBox.singleton.RunDialogBox("NEW UPGRADE\nYou collected enough metal to upgrade your ship.\n+Max Health, +Max Fuel +Fire rate"));
                }
            }

            return true;
        } else return false;
    }

    private void Start() {
        PlayerHandler.singleton = this;
        world = GameObject.FindGameObjectWithTag("World");
    }
    private void Update() {
        if((IsShooting && IsFullAuto) || ShootThisFrame) {
            if(Time.time > _lastShotFired + 1 / FireRate) {
                _lastShotFired = Time.time;
                Shoot(LookVector);
            }
        }
    }
    public void Shoot(Vector3 direction) {
        Projectile lazer = Instantiate<Projectile>(LazerPrefab);
        lazer.transform.SetParent(world.transform);
        lazer.transform.position = transform.position;
        lazer.transform.LookAt(lazer.transform.position + direction);
        lazer.transform.rotation *= Quaternion.Euler(0,90,90);
    }
}

[System.Serializable]
public class Upgrade {
    public float MetalCost = 100;
    public float NewMaxFuel = 120;
    public float NewMaxHealth = 120;
    public float NewFireRate = 6;
    public bool IsFullAuto = true;

    public Upgrade(float cost, float fuel, float health) {
        MetalCost = cost;
        NewMaxFuel = fuel;
        NewMaxHealth = health;
    }
    public Upgrade(float cost, float fuel, float health, float fireRate, bool fullAuto) {
        MetalCost = cost;
        NewMaxFuel = fuel;
        NewMaxHealth = health;
        NewFireRate = fireRate;
        IsFullAuto = fullAuto;
    }
}