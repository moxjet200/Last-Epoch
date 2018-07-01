using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AlignmentManager))]
[RequireComponent(typeof(LocationDetector))]
public class MoveToNearestEnemyOnCreation : OnCreation
{
    public float maxRange = 5f;
    public Vector3 offset = new Vector3(0,0,0);
    public bool moveToAllyInstead = false;

    // Use this for initialization
    public override void onCreation()
    {
        Alignment alignment = GetComponent<AlignmentManager>().alignment;
        if (alignment == null) { Debug.LogError("AttachToNearestAllyOnCreation component on " + gameObject.name + " cannot function as alignment is null"); return; }
        // find a lit of alignment objects that belong to enemies
        List<AlignmentManager> alignments = new List<AlignmentManager>();
        if (moveToAllyInstead)
        {
            foreach (BaseHealth healthObject in BaseHealth.all)
            {
                if (alignment.friends.Contains(healthObject.GetComponent<AlignmentManager>().alignment))
                {
                    alignments.Add(healthObject.GetComponent<AlignmentManager>());
                }
            }
        }
        else
        {
            foreach (BaseHealth healthObject in BaseHealth.all)
            {
                if (alignment.foes.Contains(healthObject.GetComponent<AlignmentManager>().alignment))
                {
                    alignments.Add(healthObject.GetComponent<AlignmentManager>());
                }
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
            // find the nearest alignment object that is an enemy
            AlignmentManager nearestAlignment = alignments[0];
            float distance = Vector3.Distance(transform.position, nearestAlignment.transform.position);
            foreach(AlignmentManager alignmentManager in alignments)
            {
                if (Vector3.Distance(transform.position, alignmentManager.transform.position) < distance)
                {
                    nearestAlignment = alignmentManager;
                    distance = Vector3.Distance(transform.position, alignmentManager.transform.position);
                }
            }

            // only move if the nearest one is within range
            if (distance < maxRange)
            {
                // move to the enemy's location
                transform.position = nearestAlignment.transform.position + offset;
                //// change to the ally's rotation
                //transform.rotation = transform.parent.rotation;
            }
        }
    }
}
