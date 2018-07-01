using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUpPassivesButton : MonoBehaviour {

    public static LevelUpPassivesButton instance = null;


    // Use this for initialization
    void Start()
    {
        LevelUpPassivesButton.instance = this;
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update () {
		if (UIBase.instance.passiveTreeOpen) { deactivate(); }
	}

    public void clicked()
    {
        UIBase ui = UIBase.instance;
        if (ui)
        {
            ui.openPassiveTree(true);

            // play a sound
            UISounds.playSound(UISounds.UISoundLabel.Confirm);

            deactivate();
        }
    }

    public static void activate()
    {
        if (instance)
        {
            instance.gameObject.SetActive(true);
        }
    }

    public static void deactivate()
    {
        if (instance)
        {
            instance.gameObject.SetActive(false);
        }
    }
}
