using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosesWhenSkillTreeCloses : MonoBehaviour
{

    void Awake()
    {
        UIBase.instance.skillTreeCloseEvent += Deactivate;
    }

    void OnDestroy()
    {
        UIBase.instance.skillTreeCloseEvent -= Deactivate;
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
