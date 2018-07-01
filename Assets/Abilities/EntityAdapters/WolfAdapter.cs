using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfAdapter : EntityAdapter
{
    public bool retaliatesWithLightning = false;

    public override GameObject adapt(GameObject entity)
    {
        if (retaliatesWithLightning)
        {
            // activate lightning vfx
            foreach (PSMeshRendererUpdater psmru in entity.GetComponentsInChildren<PSMeshRendererUpdater>(true))
            {
                psmru.gameObject.SetActive(true);
                psmru.IsActive = true;
                psmru.UpdateMeshEffect();
            }
        }

        return base.adapt(entity);
    }



}