using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialisationPanelManager : MonoBehaviour {

    public static SpecialisationPanelManager instance = null;

    public int slotClicked = 0;

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
