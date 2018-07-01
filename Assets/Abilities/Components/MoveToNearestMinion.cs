using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AlignmentManager))]
[RequireComponent(typeof(LocationDetector))]
public class MoveToNearestMinion : OnCreation
{
    public enum lifeState
    {
        alive, dead, either
    }

    public Vector3 displacement = new Vector3(0, 0, 0);

    public bool destroyIfFailedToMove = false;

    public lifeState requiredLifeState = lifeState.alive;

    public bool kill = false;

    public bool eraseCorpse = false;

    // Use this for initialization
    public override void onCreation()
    {

        bool success = false;

        CreationReferences references = GetComponent<CreationReferences>();

        if (!references || !references.creator) {

            if (destroyIfFailedToMove)
            {
                SelfDestroyer dest = Comp<SelfDestroyer>.GetOrAdd(gameObject);
                dest.failedAbility = true;
                dest.die();
            }
            return;
        }

        SummonTracker tracker = references.creator.GetComponent<SummonTracker>();

        if (!tracker) {
            if (destroyIfFailedToMove)
            {
                SelfDestroyer dest = Comp<SelfDestroyer>.GetOrAdd(gameObject);
                dest.failedAbility = true;
                dest.die();
            }
            return;
        }

        // find a lit of summon
        List<Summoned> summons = new List<Summoned>();
        foreach (Summoned summoned in tracker.summons)
        {
            if (summoned)
            {
                summons.Add(summoned);
            }
        }
        // remove inactive objects from the list
        for (int i = summons.Count - 1; i >= 0; i--)
        {
            if (!summons[i].gameObject.activeSelf)
            {
                summons.Remove(summons[i]);
            }
        }
        // remove dying objects from the list
        if (requiredLifeState == lifeState.alive)
        {
            for (int i = summons.Count - 1; i >= 0; i--)
            {
                if (summons[i].GetComponent<Dying>() != null)
                {
                    if (summons[i].GetComponent<StateController>().currentState == summons[i].GetComponent<StateController>().dying)
                    {
                        summons.Remove(summons[i]);
                    }
                }
            }
        }
        // or remove living objects from the list
        else if (requiredLifeState == lifeState.dead)
        {
            for (int i = summons.Count - 1; i >= 0; i--)
            {
                if (summons[i].GetComponent<Dying>() != null)
                {
                    if (summons[i].GetComponent<StateController>().currentState != summons[i].GetComponent<StateController>().dying)
                    {
                        summons.Remove(summons[i]);
                    }
                }
                else
                {
                    summons.Remove(summons[i]);
                }
            }
        }
        if (summons.Count == 0)
        {
            if (destroyIfFailedToMove)
            {
                SelfDestroyer dest = Comp<SelfDestroyer>.GetOrAdd(gameObject);
                dest.failedAbility = true;
                dest.die();
            }
            return;
        }
        else
        {
            // find the nearest summon
            Summoned nearest = summons[0];
            float distance = Vector3.Distance(transform.position, nearest.transform.position);
            foreach (Summoned summon in summons)
            {
                if (Vector3.Distance(transform.position, summon.transform.position) < distance)
                {
                    nearest = summon;
                    distance = Vector3.Distance(transform.position, summon.transform.position);
                }
            }

            // move to the summon's location
            transform.position = nearest.transform.position;
            // change to the summon's rotation
            transform.rotation = nearest.transform.rotation;

            success = true;

            if (kill) {
                Dying dying = nearest.GetComponent<Dying>();

                if (dying)
                {
                    if (eraseCorpse)
                    {
                        dying.setDelays(0.01f, 0.02f);
                    }

                    dying.myHealth.HealthDamage(dying.myHealth.maxHealth * 200);
                }
            }
        }
        if (destroyIfFailedToMove && success == false)
        {
            SelfDestroyer dest = Comp<SelfDestroyer>.GetOrAdd(gameObject);
            dest.failedAbility = true;
            dest.die();
        }
    }
}
