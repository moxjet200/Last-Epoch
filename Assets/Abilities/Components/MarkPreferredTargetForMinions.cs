using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkPreferredTargetForMinions : MonoBehaviour
{
    public void Start()
    {
        if (transform.parent)
        {
            CreationReferences references = GetComponent<CreationReferences>();
            if (references && references.creator)
            {
                SummonTracker tracker = references.creator.GetComponent<SummonTracker>();
                {
                    if (tracker)
                    {
                        foreach(Summoned summon in tracker.summons)
                        {
                            TargetFinder tf = summon.GetComponent<TargetFinder>();
                            if (tf)
                            {
                                tf.preferredTarget = transform.parent.gameObject;
                            }
                        }
                    }
                }
            }


        }
    }


}
