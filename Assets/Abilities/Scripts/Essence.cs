using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Essence : MonoBehaviour
{
    public EssenceType essenceType = EssenceType.None;

    public void Start()
    {
        if (transform.parent)
        {
            EssenceManager manager = transform.parent.GetComponent<EssenceManager>();
            if (!manager) { manager = transform.parent.gameObject.AddComponent<EssenceManager>(); }
            if (manager)
            {
                manager.addEssence(this);
            }
        }
    }

    public void destroy()
    {
        SelfDestroyer destroyer = GetComponent<SelfDestroyer>();
        if (destroyer) { destroyer.die(); }
        else { Destroy(gameObject); }
    }
}
