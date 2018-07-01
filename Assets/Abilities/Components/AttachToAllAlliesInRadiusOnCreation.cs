using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AlignmentManager))]
[RequireComponent(typeof(LocationDetector))]
public class AttachToAllAlliesInRadiusOnCreation : OnCreation
{
    [Tooltip("If the target already has a buff from this skill should this replace it?")]
    public bool replaceExistingBuff = true;

    public Vector3 displacement = new Vector3(0,0,0);

    public float radius = 0f;

    // Use this for initialization
    public override void onCreation()
    {
        Alignment alignment = GetComponent<AlignmentManager>().alignment;
        if (alignment == null) { Debug.LogError("AttachToNearestAllyOnCreation component on " + gameObject.name + " cannot function as alignment is null"); return; }
        // find a lit of alignment objects that belong to allies
        List<AlignmentManager> alignments = new List<AlignmentManager>();
        foreach (BaseHealth healthObject in BaseHealth.all)
        {
            if (alignment.isSameOrFriend(healthObject.GetComponent<AlignmentManager>().alignment))
            {
                alignments.Add(healthObject.GetComponent<AlignmentManager>());
            }
        }
        // remove dying objects from the list
        for (int i = alignments.Count - 1; i >= 0; i--)
        {
            if (alignments[i].GetComponent<Dying>() != null)
            {
                if (alignments[i].GetComponent<StateController>().currentState == alignments[i].GetComponent<StateController>().dying)
                {
                    alignments.Remove(alignments[i]);
                }
            }
        }
        if (alignments.Count == 0) { return; }
        else
        {
            // attach to allies within the radius
            foreach (AlignmentManager ally in alignments)
            {
                if (Vector3.Distance(transform.position, ally.transform.position) <= radius)
                {
                    // if the nearest one already has a buff of this type then remove it if necessary
                    if (replaceExistingBuff && GetComponent<CreationReferences>())
                    {
                        CreationReferences[] references = ally.transform.GetComponentsInChildren<CreationReferences>();
                        Ability thisAbility = GetComponent<CreationReferences>().thisAbility;
                        for (int i = 0; i < references.Length; i++)
                        {
                            // check if the ability if the same as this one
                            if (references[i].thisAbility == thisAbility)
                            {
                                // destroy the existing buff
                                if (references[i].gameObject.GetComponent<SelfDestroyer>()) { references[i].gameObject.GetComponent<SelfDestroyer>().die(); }
                                else { Destroy(references[i].gameObject); }
                            }
                        }
                    }

                    // attach a copy to the ally
                    GameObject copy = Instantiate(gameObject, ally.transform);
                    // move the copy to the ally's location
                    copy.transform.localPosition = displacement;
                    // remove this component from the copy
                    Destroy(copy.GetComponent<AttachToAllAlliesInRadiusOnCreation>());
                }
            }
        }
        Destroy(gameObject);
    }
}
