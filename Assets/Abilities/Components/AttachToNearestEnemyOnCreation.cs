using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AlignmentManager))]
[RequireComponent(typeof(LocationDetector))]
public class AttachToNearestEnemyOnCreation : OnCreation
{
    public enum lifeState
    {
        alive, dead, either, zeroHealth
    }


    [Tooltip("If the target already has a buff from this skill should this replace it?")]
    public bool replaceExistingDebuff = true;

    public Vector3 displacement = new Vector3(0, 0, 0);

    public bool destroyIfFailedToAttach = false;

    public lifeState requiredLifeState = lifeState.alive;

    public bool useRangeLimit = false;

    public float rangeLimit = 0f;

    public bool eraseInsteadOfAttaching = false;

    // Use this for initialization
    public override void onCreation()
    {
        bool attached = false;
        Alignment alignment = GetComponent<AlignmentManager>().alignment;
        if (alignment == null) { Debug.LogError("AttachToNearestAllyOnCreation component on " + gameObject.name + " cannot function as alignment is null");
            if (destroyIfFailedToAttach)
            {
                SelfDestroyer dest = Comp<SelfDestroyer>.GetOrAdd(gameObject);
                dest.die();
            }
            return;
        }
        // find a list of alignment objects that belong to enemies
        List<AlignmentManager> alignments = new List<AlignmentManager>();
        foreach (BaseHealth healthObject in BaseHealth.all)
        {
            if (healthObject.alignmentManager && !alignment.isSameOrFriend(healthObject.alignmentManager.alignment))
            {
                alignments.Add(healthObject.alignmentManager);
            }
        }
        // remove inactive objects from the list
        for (int i = alignments.Count - 1; i >= 0; i--)
        {
            if (!alignments[i].gameObject.activeSelf)
            {
                alignments.Remove(alignments[i]);
            }
        }
        // remove dying objects from the list
        if (requiredLifeState == lifeState.alive)
        {
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
        }
        // or remove living objects from the list
        else if (requiredLifeState == lifeState.dead)
        {
            for (int i = alignments.Count - 1; i >= 0; i--)
            {
                if (alignments[i].GetComponent<Dying>() != null)
                {
                    if (alignments[i].GetComponent<StateController>().currentState != alignments[i].GetComponent<StateController>().dying)
                    {
                        alignments.Remove(alignments[i]);
                    }
                }
                else
                {
                    alignments.Remove(alignments[i]);
                }
            }
        }
        // slightly different to dead as it will include things that died this step, but also things without a dying state
        else if (requiredLifeState == lifeState.zeroHealth)
        {
            BaseHealth bh = null;
            for (int i = alignments.Count - 1; i >= 0; i--)
            {
                bh = alignments[i].GetComponent<BaseHealth>();
                if (bh && bh.currentHealth > 0)
                {
                    alignments.Remove(alignments[i]);
                }
            }
        }
        if (alignments.Count == 0) {
            if (destroyIfFailedToAttach)
            {
                SelfDestroyer dest = Comp<SelfDestroyer>.GetOrAdd(gameObject);
                dest.die();
            }
            return;
        }
        else
        {
            // find the nearest alignment object that is an enemy
            AlignmentManager nearestAlignment = alignments[0];
            float distance = Vector3.Distance(transform.position, nearestAlignment.transform.position);
            foreach (AlignmentManager alignmentManager in alignments)
            {
                if (Vector3.Distance(transform.position, alignmentManager.transform.position) < distance)
                {
                    nearestAlignment = alignmentManager;
                    distance = Vector3.Distance(transform.position, alignmentManager.transform.position);
                }
            }

            // return if it is not close enough
            if (useRangeLimit && distance > rangeLimit)
            {
                if (destroyIfFailedToAttach)
                {
                    SelfDestroyer dest = Comp<SelfDestroyer>.GetOrAdd(gameObject);
                    dest.die();
                }
                return;
            }

            // if the nearest one already has a buff of this type then remove it if necessary
            if (replaceExistingDebuff && GetComponent<CreationReferences>())
            {
                CreationReferences[] references = nearestAlignment.transform.GetComponentsInChildren<CreationReferences>();
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

            attached = true;
            // attach to the nearest one
            transform.parent = nearestAlignment.transform;
            // move to the ally's location
            transform.localPosition = displacement;
            // change to the ally's rotation
            transform.rotation = transform.parent.rotation;

            if (eraseInsteadOfAttaching)
            {
                BaseHealth bh = nearestAlignment.GetComponent<BaseHealth>();
                if (bh)
                {
                    bh.HealthDamagePercent(1000);
                }
                Dying dying = nearestAlignment.GetComponent<Dying>();
                if (dying && !dying.delaysCannotBeChanged)
                {
                    dying.destructionDelay = 0.02f;
                    dying.sinkingDelay = 0.01f;
                }

                transform.parent = null;
            }
        }
        if (destroyIfFailedToAttach && !attached)
        {
            SelfDestroyer dest = Comp<SelfDestroyer>.GetOrAdd(gameObject);
            dest.die();
        }
    }
}
