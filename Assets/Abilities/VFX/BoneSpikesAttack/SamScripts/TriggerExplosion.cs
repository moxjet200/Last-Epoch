using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerExplosion : MonoBehaviour
{

	public ParticleSystem Splosion;
	public GameObject ImpactDecal;

	private void OnTriggerEnter()
	{
		Splosion.Play();
		ImpactDecal.SetActive(true);
		Invoke("TurnOff", 2);

	}

	void TurnOff()
	{
		ImpactDecal.SetActive(false);
	}

}
