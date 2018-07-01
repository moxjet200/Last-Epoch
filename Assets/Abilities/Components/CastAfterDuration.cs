using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AbilityObjectConstructor))]
public class CastAfterDuration : MonoBehaviour
{

    public Ability ability = null;
    public float interval = 1f;
    public bool limitCasts = false;
    public int remainingCasts = 0;
    [HideInInspector]
    public float age;
    public Vector3 offset = Vector3.zero;
    [Header("Scaling")]
    public Tags.Properties scalingProperty = Tags.Properties.None;
    public List<Tags.AbilityTags> scalingTags = new List<Tags.AbilityTags>();

    [Header("castingProperties")]
    public bool randomAiming = false;
    public bool castFromCreatorCastPoint = false;
    [Tooltip("casts at the creator's UsingAbility's targetLocation")]
    public bool castAtCreatorTargetLocation = false;

    [Header("Randomisation")]
    public float castChance = 1f;
    public bool failedCastConsumesARemainingCast = false;

    SizeManager creatorSizeManager = null;
    AbilityObjectConstructor abilityObjectConstructor = null;
    UsingAbility creatorUsingAbility = null;

    // used for aiming, updated every cast
    Vector3 from;
    Vector3 targetPoint;

    // Use this for initialization
    void Start()
    {
        abilityObjectConstructor = Comp<AbilityObjectConstructor>.GetOrAdd(gameObject);
        // calculate scaling
        if (scalingProperty != Tags.Properties.None)
        {
            CreationReferences references = GetComponent<CreationReferences>();
            if (references && references.creator)
            {
                BaseStats stats = references.creator.GetComponent<BaseStats>();
                if (stats)
                {
                    float scaler = stats.GetStatValue(scalingProperty);
                    if (scaler != 0)
                    {
                        interval /= scaler;
                    }
                }
            }
        }
        // get creator components if necessery
        if (castFromCreatorCastPoint || castAtCreatorTargetLocation)
        {
            CreationReferences references = GetComponent<CreationReferences>();
            if (references && references.creator)
            {
                if (castFromCreatorCastPoint) { creatorSizeManager = references.creator.GetComponent<SizeManager>(); }
                
                if (castAtCreatorTargetLocation) { creatorUsingAbility = references.creator.GetComponent<UsingAbility>(); }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (age >= interval)
        {
            if (!limitCasts || (remainingCasts > 0))
            {
                if (castChance >= 1)
                {
                    cast();
                    if (limitCasts) { remainingCasts--; }
                }
                else if (castChance > 0)
                {
                    if (Random.Range(0f,1f) < castChance)
                    {
                        cast();
                        if (limitCasts) { remainingCasts--; }
                    }
                    else if (limitCasts && failedCastConsumesARemainingCast)
                    {
                        remainingCasts--;
                    }
                }
                
                age -= interval;
            }
        }
        age += Time.deltaTime;
    }

    public void cast()
    {
        // calculate points to cast from and to
        if (creatorSizeManager && castFromCreatorCastPoint)
        {
            bool affectsHeight; bool affectsDirection;
            from = creatorSizeManager.getCastPosition(ability.animation, out affectsHeight, out affectsDirection) + offset;
            targetPoint = from + creatorSizeManager.transform.forward;
        }
        else
        {
            from = new Vector3(transform.position.x, getY(transform.position), transform.position.z) + offset;
            targetPoint = from + transform.forward;
        }

        // change target location to creator's target location if necessary
        if (castAtCreatorTargetLocation)
        {
            targetPoint = creatorUsingAbility.getTargetLocation() + new Vector3(0, 1.2f, 0);
        }

        // randomise point to cast to
        if (randomAiming)
        {
            if (castAtCreatorTargetLocation) { targetPoint += getRandomPositionInCircle(0.5f, 2f); }
            else if (creatorSizeManager && castFromCreatorCastPoint) { targetPoint += getRandomPositionInCircle(0.5f, 2f) - creatorSizeManager.transform.forward; }
            else { targetPoint += getRandomPositionInCircle(0.5f, 2f) - transform.forward; }
        }

        // cast ability
        abilityObjectConstructor.constructAbilityObject(ability, from, targetPoint);
    }


    // gets the Y coordinate of the ground at a position
    public float getY(Vector3 position)
    {
        // the maximum height above the ground to look for a surface
        float height = 10f;
        // raycast down from this height
        RaycastHit hit;
        LayerMask mask = 1 << LayerMask.NameToLayer("Default");
        if (Physics.Raycast(new Vector3(position.x, position.y + height, position.z), Vector3.down, out hit, Mathf.Infinity, mask))
        {
            return hit.point.y + 1.2f;
        }
        // if nothing was hit, just return the y value of the point
        return position.y;
    }

    public Vector3 getRandomPositionInCircle(float minRadius, float maxRadius)
    {

        float angle = Random.Range(0f, 2 * Mathf.PI);
        float distance = Random.Range(minRadius, maxRadius);
        return new Vector3(distance * Mathf.Cos(angle), 0, distance * Mathf.Sin(angle));
    }
}
