using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTowardsTarget : MonoBehaviour
{

	public float speed = 100;
	public Transform Target;
	private Animator anim;


	
	void Update ()
	{
		// movement
		
		float step = speed * Time.deltaTime;
		transform.position = Vector3.MoveTowards(transform.position, Target.position, step);
		
		// rotation
		
		Vector3 targetDir = Target.position - transform.position;
		// The step size is equal to speed times frame time.
		float rot = speed * Time.deltaTime;
		Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, rot, 0.0f);
		// Move our position a step closer to the target.
		transform.rotation = Quaternion.LookRotation(newDir);
		//float step = speed * Time.deltaTime;
	}
}
