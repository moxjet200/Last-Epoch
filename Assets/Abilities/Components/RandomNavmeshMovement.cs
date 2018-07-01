using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RandomNavmeshMovement : MonoBehaviour {

    public float updateRate = 1f;
    NavMeshAgent agent = null;
    float index = float.MaxValue;

    public float randomisationDistance = 2f;
    public float randomDestinationDistance = 5f;

	// Use this for initialization
	void Start () {
        agent = GetComponent<NavMeshAgent>();
        agent.destination = transform.position;
        chooseRandomDestination();
	}
	
	// Update is called once per frame
	void Update () {
        if (index > updateRate)
        {
            index = 0f;
            chooseRandomDestination();
        }
        index += Time.deltaTime;
    }

    void chooseRandomDestination()
    {
        Vector3 newDestination = agent.destination;
        float randomAngle = Random.Range(0f, 2*Mathf.PI);
        newDestination = new Vector3(newDestination.x + randomisationDistance * Mathf.Cos(randomAngle), newDestination.y, newDestination.z + randomisationDistance * Mathf.Sin(randomAngle));
        newDestination = transform.position + Vector3.Normalize(newDestination - transform.position) * randomDestinationDistance;
        agent.destination = newDestination;
    }
}
