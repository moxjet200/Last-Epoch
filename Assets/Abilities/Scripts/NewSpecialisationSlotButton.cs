﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewSpecialisationSlotButton : MonoBehaviour {

    public static NewSpecialisationSlotButton instance = null;


	// Use this for initialization
	void Start () {
        NewSpecialisationSlotButton.instance = this;
        gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
        if (UIBase.instance.skillsOpen) { deactivate(); }
    }

    public void clicked()
    {
        UIBase ui = UIBase.instance;
        ui.openSkills();

        // play a sound
        UISounds.playSound(UISounds.UISoundLabel.Confirm);

        deactivate();
    }

    public static void activate()
    {
        instance.gameObject.SetActive(true);
    }

    public static void deactivate()
    {
        instance.gameObject.SetActive(false);
    }
}
