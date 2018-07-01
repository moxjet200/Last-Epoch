using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityCharge : MonoBehaviour {

    public GameObject activeObject = null;

	public void activate()
    {
        activeObject.SetActive(true);
    }

    public void deactivate()
    {
        activeObject.SetActive(false);
    }

}
