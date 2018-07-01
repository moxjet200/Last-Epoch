using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedGlobalRotation : MonoBehaviour
{
    
    public Vector3 eulerAngles = Vector3.zero;
    
    // Use this for initialization
    void Start()
    {
        transform.eulerAngles = eulerAngles;
    }
}
