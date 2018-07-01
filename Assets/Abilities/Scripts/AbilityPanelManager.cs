using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityPanelManager : MonoBehaviour {

    public static AbilityPanelManager instance = null;

    public int abilityClicked = 0;

    void Awake()
    {
        instance = this;
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
