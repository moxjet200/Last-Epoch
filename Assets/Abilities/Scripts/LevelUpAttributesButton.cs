using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUpAttributesButton : MonoBehaviour {

    public static LevelUpAttributesButton instance = null;


	// Use this for initialization
	void Start () {
        LevelUpAttributesButton.instance = this;
        gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void clicked()
    {
        UIBase ui = UIBase.instance;
        ui.openStats();
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
