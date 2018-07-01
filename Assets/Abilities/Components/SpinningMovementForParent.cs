using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// This is not a state, but provides behaviour for the UsingAbility State for situations where an ability requires continuous movement (e.g. Fury Leap)
[RequireComponent(typeof(NavMeshAgent))]
public class SpinningMovementForParent : MonoBehaviour
{
    NavMeshAgent agent = null;
    Ability ability = null;
    public Vector3 destination = Vector3.zero;
    public bool parentIsPlayer = false;
    public GameObject parent = null;
    public MovementFromAbility movementFromAbility = null;
    public float distantSpeed = 6f;
    public float nearSpeed = 0.5f;

    public void Start()
    {
        parent = transform.parent.gameObject;
        if (parent == PlayerFinder.getPlayer()) { parentIsPlayer = true; }
        if (parentIsPlayer)
        {
            agent = GetComponent<NavMeshAgent>();
            movementFromAbility = parent.GetComponent<MovementFromAbility>();
            destination = movementFromAbility.destination;
            transform.position = transform.parent.position;
            agent.baseOffset = parent.GetComponent<NavMeshAgent>().baseOffset;
        }
    }


    public void Update()
    {
        updateMovementAndPositon();
    }

    public void updateMovementAndPositon()
    {
        destination = movementFromAbility.destination;
        agent.destination = destination;
        if (Vector3.Distance(transform.position, destination) < 0.5f) { agent.speed = nearSpeed; }
        else { agent.speed = distantSpeed; }
        transform.parent.position = transform.position;
    }

    public void LateUpdate()
    {
        updateMovementAndPositon();
    }


}
