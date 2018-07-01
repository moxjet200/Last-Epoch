using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// needs to listen for the object's death
[RequireComponent(typeof(SelfDestroyer))]
[RequireComponent(typeof(CreationReferences))]
public class CreatePortalOnDeath : MonoBehaviour
{
    public GameObject portalPrefab;

    void Awake()
    {
        GetComponent<SelfDestroyer>().deathEvent += createPortal;
    }

    public void createPortal()
    {
        GameObject portal = Instantiate(portalPrefab, transform.position, Quaternion.identity);
        GameObject caster = GetComponent<CreationReferences>().creator;
        if (caster && caster.GetComponent<PortalAbility>())
        {
            Destroy(caster.GetComponent<PortalAbility>().currentPortal);
            caster.GetComponent<PortalAbility>().currentPortal = portal;
        }
    }
}
