using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopConstantRotationOnAbilityUse : MonoBehaviour
{
    public ConstantRotation constantRotation = null;
    public GameObject usingAbilityGo = null;
    UsingAbility usingAbility = null;
    bool initialised = false;
    float index = 0f;
    public float delay = 0.3f;
    float degreesPerSecond = 0f;

    public bool fixPosition = false;
    Vector3 stopPosition = Vector3.zero;

    public void Update()
    {
        if (!initialised) { initialise(); }
        if (initialised)
        {
            if (index > 0)
            {
                index -= Time.deltaTime;
                if (index <= 0 && constantRotation)
                {
                    constantRotation.degreesPerSecond = degreesPerSecond;
                }
                if (fixPosition)
                {
                    transform.position = stopPosition;
                }
            }
        }
    }

    public void LateUpdate()
    {
        if (index > 0)
        {
            if (fixPosition)
            {
                transform.position = stopPosition;
            }
        }
    }


    public void initialise()
    {
        initialised = true;
        usingAbility = usingAbilityGo.GetComponent<UsingAbility>();
        if (usingAbility)
        {
            usingAbility.usedAbilityManaEvent += pause;
        }
        if (constantRotation)
        {
            degreesPerSecond = constantRotation.degreesPerSecond;
        }
    }

    public void OnDestroy()
    {
        if (usingAbility) { usingAbility.usedAbilityManaEvent -= pause; }
    }

    public void pause()
    {
        if (constantRotation)
        {
            constantRotation.degreesPerSecond = 0.01f;
            index = delay;
            stopPosition = transform.position;
        }
    }

}
