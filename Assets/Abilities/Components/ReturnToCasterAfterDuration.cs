using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SelfDestroyer))]
public class ReturnToCasterAfterDuration : MonoBehaviour
{

    public float duration = 1f;
    [HideInInspector]
    public float age;

    public bool returning = false;
    GameObject creator = null;
    AbilityMover mover = null;

    public bool destroyOnReachingCreator = false;
    public Vector3 displacement = Vector3.zero;

    // Use this for initialization
    void Start()
    {
        CreationReferences references = GetComponent<CreationReferences>();
        if (references)
        {
            creator = references.creator;
        }
        mover = GetComponent<AbilityMover>();
    }

    // Update is called once per frame
    void Update()
    {
        if (returning && creator && mover)
        {
            mover.SetDirection(creator.transform.position + displacement - transform.position);
            
            if (destroyOnReachingCreator)
            {
                if (Maths.manhattanDistance(transform.position, creator.transform.position + displacement) < mover.speed * Time.deltaTime * 2)
                {
                    SelfDestroyer destroyer = GetComponent<SelfDestroyer>();
                    if (destroyer) { destroyer.die(); }
                    else { Destroy(gameObject); }
                }
            }
        }

        if (age >= duration && !returning) { returning = true; }
        age += Time.deltaTime;

    }
}
