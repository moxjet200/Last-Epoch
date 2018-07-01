using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetXRotationToZero : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, transform.eulerAngles.z);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

