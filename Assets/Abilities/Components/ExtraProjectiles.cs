using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using System;

[RequireComponent(typeof(AbilityObjectConstructor))]
[RequireComponent(typeof(AbilityMover))]
[RequireComponent(typeof(CreationReferences))]
[RequireComponent(typeof(LocationDetector))]
public class ExtraProjectiles : OnCreation {

    [Range(0f,180f)]
    public float angle = 15;

    public int numberOfExtraProjectiles = 2;

    public bool randomAngles = false;

    public bool delayExtraProjectiles = false;

    public float delayWindow = 0.5f;

    public bool randomiseDelay = false;

    float angleOfNextProjectile = 0;

    bool soundsRemoved = false;

    void Start()
    {
        if (GetComponent<SelfDestroyer>()) { GetComponent<SelfDestroyer>().deathEvent += MakeExtraProjectiles; }
    }

    void Update() {
        MakeExtraProjectiles();
    }

    public override void onCreation()
    {
        
    }


    public void MakeExtraProjectiles()
    {
        // get references to necessary components and locations
        CreationReferences creationReferences = GetComponent<CreationReferences>();
        Ability thisAbility = creationReferences.thisAbility;
        AbilityObjectConstructor constructor = GetComponent<AbilityObjectConstructor>();
        Vector3 targetLocation = GetComponent<LocationDetector>().targetLocation;
        Vector3 projectileStartPoint = GetComponent<LocationDetector>().startLocation;
        Vector3 startDirection = targetLocation - projectileStartPoint;

        // reduce the intensity of any lights
        if (numberOfExtraProjectiles != 0)
        {
            foreach (Light light in GetComponentsInChildren<Light>())
            {
                light.intensity /= numberOfExtraProjectiles;
            }
        }

        // setup angle calculations
        float angleBetweenProjectiles = 2 * angle / numberOfExtraProjectiles;
        Vector3 direction;
        
        if (randomAngles)
        {
            angleOfNextProjectile = UnityEngine.Random.Range(-angle, angle);
        }
        else
        {
            angleOfNextProjectile = -angle;
        }

        // create the projectiles
        List<GameObject> newProjectiles = new List<GameObject>();
        for (int i = 0; i <= numberOfExtraProjectiles; i++)
        {
            if (i != numberOfExtraProjectiles/2)
            {
                direction = Quaternion.Euler(0, angleOfNextProjectile, 0) * startDirection;
                GameObject newProjectile = constructor.constructAbilityObject(thisAbility, projectileStartPoint, projectileStartPoint + direction, gameObject);
                // move the projectile to make it catch up
                if (newProjectile.GetComponent<AbilityMover>()) { newProjectile.transform.position += newProjectile.GetComponent<AbilityMover>().positionDelta * Vector3.Distance(transform.position, GetComponent<LocationDetector>().startLocation); }
                // don't endlessly loop, silly
                Destroy(newProjectile.GetComponent<ExtraProjectiles>());
                // add the projectile to the list
                newProjectiles.Add(newProjectile);
                // disable sound for new projectiles
                foreach (PlayOneShotSound playSound in newProjectile.GetComponents<PlayOneShotSound>())
                {
                    if (playSound.playEvent == PlayOneShotSound.PlayEvent.start)
                    {
                        playSound.active = false;
                    }
                }
            }
            if (randomAngles)
            {
                angleOfNextProjectile = UnityEngine.Random.Range(-angle, angle);
            }
            else
            {
                angleOfNextProjectile += angleBetweenProjectiles;
            }
        }


        // delay projectiles if necessary
        if (delayExtraProjectiles)
        {
            float delay = 0f;
            GameObject delayer;
            EnablesChildrenAfterDelay enabler;
            for (int i = 0; i < newProjectiles.Count; i++)
            {
                // calculate the delay
                if (!randomiseDelay) { delay += (delayWindow / newProjectiles.Count); }
                else { delay = UnityEngine.Random.Range(0f, delayWindow); }

                // create the delayer
                delayer = new GameObject();
                enabler = delayer.AddComponent<EnablesChildrenAfterDelay>();
                enabler.delay = delay;

                // set the parent of the new projectile to the delay
                newProjectiles[i].transform.SetParent(delayer.transform);
                newProjectiles[i].SetActive(false);
            }
        }


        // destroy this component
        if (GetComponent<SelfDestroyer>()) { GetComponent<SelfDestroyer>().deathEvent -= MakeExtraProjectiles; }
        Destroy(this);
    }
}
