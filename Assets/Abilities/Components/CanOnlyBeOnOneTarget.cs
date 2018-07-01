using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SelfDestroyer))]
public class CanOnlyBeOnOneTarget : MonoBehaviour
{
    public static List<CanOnlyBeOnOneTarget> all = new List<CanOnlyBeOnOneTarget>();

    public CreationReferences references = null;

    public void Start()
    {
        references = GetComponent<CreationReferences>();
        if (references && references.creator)
        {
            all.RemoveAll(x => x == null);
            foreach(CanOnlyBeOnOneTarget other in all)
            {
                if (other.references && other.references.creator)
                {
                    if (other.references.thisAbility == references.thisAbility && other.references.creator == references.creator && other.transform.parent != transform.parent)
                    {
                        SelfDestroyer destroyer = other.GetComponent<SelfDestroyer>();
                        if (destroyer)
                        {
                            destroyer.die();
                        }
                    }
                }

            }

            all.Add(this);
        }
    }

    public void OnDestroy()
    {
        all.Remove(this);
    }

}
