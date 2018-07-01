using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiralMovement : MonoBehaviour
{
    public enum ConstantType
    {
        AngularVelocity, TangentialVelocity, BothAreMaxima
    }
    
    [Header("Keeps angle or tangential velocity constant")]
    public ConstantType constantVelocity = ConstantType.AngularVelocity;
    public float angleChangedPerSecond = 0f;
    public float tangentialVelocity = 0f;

    [Header("Other movement properties")]
    public float outwardSpeed = 0f;
    public float outwardDistance = 0f;

    public bool centreOnCaster = false;
    public bool centreOnTransform = false;
    public Transform centreTransform = null;
    public Vector3 offsetFromTransform = Vector3.zero;
    Vector3 centre = Vector3.zero;
    public float yRot = 0f;
    public bool randomStartAngle = false;

    AbilityMover abilityMover = null;
    Vector3 newDirection;
    [Header("Also changes direction of Ability Mover")]
    public bool changeFacingDirection = true;

    // Use this for initialization
    void Start()
    {
        abilityMover = GetComponent<AbilityMover>();
        centre = transform.position;
        if (centreOnCaster)
        {
            CreationReferences references = GetComponent<CreationReferences>();
            if (references && references.creator)
            {
                centreOnTransform = true;
                centreTransform = references.creator.transform;
            }
        }
        if (randomStartAngle)
        {
            yRot = Random.Range(0, 360);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // get the current Position
        Vector3 currentPos = transform.position;
        // update the centre
        if (centreOnTransform && centreTransform) { centre = centreTransform.position + offsetFromTransform; }
        // update the position
        transform.position = centre + new Vector3(outwardDistance * Mathf.Cos(yRot * 2 * Mathf.PI / 360), 0, outwardDistance * Mathf.Sin(yRot * 2 * Mathf.PI / 360));
        // update the distance
        outwardDistance += outwardSpeed * Time.deltaTime;
        // update the angle depending on what must be kept constant
        if (outwardDistance != 0) {
            if (constantVelocity == ConstantType.TangentialVelocity)
            {
                yRot += (360 / 2 * Mathf.PI) * tangentialVelocity * Time.deltaTime / outwardDistance;
            }
            else if (constantVelocity == ConstantType.BothAreMaxima)
            {
                float delta1 = (360 / 2 * Mathf.PI) * tangentialVelocity * Time.deltaTime / outwardDistance;
                float delta2 = angleChangedPerSecond * Time.deltaTime;
                yRot += Mathf.Min(delta1, delta2);
            }
        }
        if (constantVelocity == ConstantType.AngularVelocity) {
            yRot += angleChangedPerSecond * Time.deltaTime;
        }
        // set the direction of the ability mover
        newDirection = transform.position - currentPos;
        abilityMover.SetDirection(newDirection, changeFacingDirection);

    }



}
