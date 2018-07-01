using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyMeAfterSeconds : MonoBehaviour
{


	public float time =3;
	public Animator anims;
	
	void Start ()
	{
		DestroyMe();
	}

	void Update()
	{
		if (Input.GetKeyUp(KeyCode.Q))
		{
			print("q is up");
			anims.SetFloat("ShootMe", 1);
		}
	}

	void DestroyMe()
	{
		Destroy(gameObject, time);
	}
}
