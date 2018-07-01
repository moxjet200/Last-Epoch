using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepeatedlyDamageParent : DamageStatsHolder{

    float index = 0;
    public float damageInterval = 0f;

    GameObject parent;

    // Use this for initialization
    void Start()
    {
    }

    void Awake()
    {
        if (transform.parent) { parent = transform.parent.gameObject; }
    }

    // Update is called once per frame
    void Update()
    {
        if (parent == null && transform.parent) { parent = transform.parent.gameObject; }
        if (parent != null)
        {
            // only apply damage every damageInterval seconds
            index += Time.deltaTime;
            if (index > damageInterval)
            {
                index -= damageInterval;
                // damage the parent
                applyDamage(parent.gameObject);
            }
        }
    }
}