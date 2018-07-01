using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityAcceleration : MonoBehaviour
{
    AbilityMover abilityMover = null;
    public float acceleration = 0f;
    public bool canGoBelowZero = false;

    // Use this for initialization
    void Start()
    {
        abilityMover = GetComponent<AbilityMover>();
    }

    // Update is called once per frame
    void Update()
    {
        abilityMover.speed += acceleration * Time.deltaTime;
        if (!canGoBelowZero && abilityMover.speed < 0) { abilityMover.speed = 0; }
    }



}
