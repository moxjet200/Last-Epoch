using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using System;

[RequireComponent(typeof(AbilityObjectConstructor))]
[RequireComponent(typeof(AbilityMover))]
[RequireComponent(typeof(CreationReferences))]
[RequireComponent(typeof(LocationDetector))]
public class SplitProjectiles : OnCreation
{
    [Range(0f, 180f)]
    public float angle = 15;

    public bool extraProjectileIsDifferentAbility = false;

    public Ability otherAbility = null;

    bool soundsRemoved = false;

    void Start()
    {
        if (GetComponent<SelfDestroyer>()) { GetComponent<SelfDestroyer>().deathEvent += MakeExtraProjectiles; }
    }

    void Update()
    {
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
        
        // create the other projectile
        List<GameObject> newProjectiles = new List<GameObject>();

        Vector3 direction = Quaternion.Euler(0, -angle, 0) * startDirection;
        Ability ability = thisAbility;
        if (extraProjectileIsDifferentAbility && otherAbility != null) { ability = otherAbility; }
        GameObject newProjectile = constructor.constructAbilityObject(ability, projectileStartPoint, projectileStartPoint + direction);
        // move the projectile to make it catch up
        if (newProjectile.GetComponent<AbilityMover>()) { newProjectile.transform.position += newProjectile.GetComponent<AbilityMover>().positionDelta * Vector3.Distance(transform.position, GetComponent<LocationDetector>().startLocation); }
        // don't endlessly loop, silly
        Destroy(newProjectile.GetComponent<SplitProjectiles>());
        // add the projectile to the list
        newProjectiles.Add(newProjectile);

        direction = Quaternion.Euler(0, angle, 0) * startDirection;
        // change the angle of this projectile
        GetComponent<AbilityMover>().SetDirection(direction);

        // destroy this component
        if (GetComponent<SelfDestroyer>()) { GetComponent<SelfDestroyer>().deathEvent -= MakeExtraProjectiles; }
        Destroy(this);
    }
}

