using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AlignmentManager))]
[RequireComponent(typeof(LocationDetector))]
public class AttachToCreatorOnCreation : OnCreation
{
    [Tooltip("If the target already has a buff from this skill should this replace it?")]
    public bool replaceExistingBuff = true;

    public bool toggle = false;

    public Vector3 displacement = new Vector3(0,0,0);

    // Use this for initialization
    public override void onCreation()
    {
        // find the creator
        GameObject creator = GetComponent<CreationReferences>().creator;
        if (!creator) { Destroy(this); return; }

        bool buffRemoved = false;

        // if the creator already has a buff of this type then remove it if necessary
        if (replaceExistingBuff && GetComponent<CreationReferences>()) {
            CreationReferences[] references = creator.transform.GetComponentsInChildren<CreationReferences>();
            Ability thisAbility = GetComponent<CreationReferences>().thisAbility;
            for (int i = 0; i<references.Length; i++)
            {
                // check if the ability if the same as this one
                if (references[i].thisAbility == thisAbility)
                {
                    // destroy the existing buff
                    if (references[i].gameObject.GetComponent<SelfDestroyer>()) { references[i].gameObject.GetComponent<SelfDestroyer>().die(); }
                    else { Destroy(references[i].gameObject); }
                    buffRemoved = true;
                }
            }
        }

        if (toggle && buffRemoved)
        {
            SelfDestroyer destroyer = Comp<SelfDestroyer>.GetOrAdd(gameObject);
            destroyer.die();
        }

        // attach to the creator
        transform.parent = creator.transform;
        // move to the creator's location
        transform.localPosition = displacement;
        // change to the creator's rotation
        transform.rotation = transform.parent.rotation;
    }
}
