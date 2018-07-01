using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EraseParentCorpse : MonoBehaviour
{
    public bool detach = false;

    // Use this for initialization
    void Start()
    {
        if (transform.parent)
        {
            Dying dying = transform.parent.GetComponent<Dying>();
            if (dying) { dying.setDelays(0f, 0f); }
            if (detach)
            {
                transform.parent = null;
            }
        }
    }
}
