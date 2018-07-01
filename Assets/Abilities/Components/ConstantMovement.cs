using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantMovement : MonoBehaviour
{

    public Vector3 velocity = Vector3.zero;

    // Update is called once per frame
    void Update()
    {
        if (velocity != Vector3.zero)
        {
            transform.position += velocity * Time.deltaTime;
        }
    }

}
