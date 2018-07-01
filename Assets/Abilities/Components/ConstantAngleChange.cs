using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantAngleChange : MonoBehaviour
{
    AbilityMover abilityMover = null;
    Vector3 newDirection;
    public bool changeFacingDirection = true;
    public float angleChangedPerSecond = 0f;

    // Use this for initialization
    void Start()
    {
        abilityMover = GetComponent<AbilityMover>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newDirection = Quaternion.Euler(0, angleChangedPerSecond * Time.deltaTime, 0) * abilityMover.positionDelta;
        abilityMover.SetDirection(newDirection, changeFacingDirection);
    }



}

