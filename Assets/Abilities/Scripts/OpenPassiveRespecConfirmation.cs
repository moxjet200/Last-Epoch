using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenPassiveRespecConfirmation : MonoBehaviour
{

    void OnEnable()
    {
        Debug.Log("hi");
        UIBase.instance.openPassiveRespecConfirmation();
        if (PassiveRespecPanel.instance)
        {
            PassiveRespecPanel.instance.updateGoldCost();
        }
        this.enabled = false;
    }

}