using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockPortal : MonoBehaviour
{

	public void OnUse(){
		if (PlayerFinder.getPlayer()){
			PlayerFinder.getPlayer().GetComponent<CharacterDataTracker>().charData.portalUnlocked = true;
		}

	}

}
