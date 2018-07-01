using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this component can be used to update the target of an attachable ability object that interacts with its parent
public class ParentTargetUpdater : MonoBehaviour
{

    public void updateTargetToParent()
    {
        if (transform.parent)
        {
            GameObject parent = transform.parent.gameObject;
            foreach (ParentTargeter targeter in GetComponents<ParentTargeter>())
            {
                targeter.setNewParentTarget(parent);
            }
        }
    }

    public void updateTarget(GameObject _target)
    {
        foreach (ParentTargeter targeter in GetComponents<ParentTargeter>())
        {
            targeter.setNewParentTarget(_target);
        }
    }

    

}