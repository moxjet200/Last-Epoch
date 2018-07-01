using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD;

public class CastEndSoundFade : MonoBehaviour {

// crates a box to choose an fmod event in inspector
[EventRef] public string insertcast;

//name of the event to call latee in script
FMOD.Studio.EventInstance cast;

// name of param we call later in script
FMOD.Studio.ParameterInstance fadeParam;


	

	void Start () {

		// cast is now whatever we put in the insertclass box in inspector
		cast = RuntimeManager.CreateInstance(insertcast);

		// fadeParam is now linked to our param in fmod
		cast.getParameter ("castEnd", out fadeParam);


		
	}


	//when the icebolt shoots the param is 0 and the sound 'cast' is started then when it ends the param goes to 1 (on)

	/*	void Castend ()

		if (icebolt == true){

			fadeParam.setValue (0f);
			cast.start();

		}


		if (icebolt == false) {

			fadeParam.setValue (1f);

		}
	
	*/
}
