using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DestroyAfterDuration))]
public class ChangeParameterWhenDurationIsLow : ParameterChanger
{
    DestroyAfterDuration destroyer = null;
    bool faded = false;

    public EnableEnum change = EnableEnum.NoChange;

	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    void Update()
    {
        if (destroyer == null) { destroyer = GetComponent<DestroyAfterDuration>(); }
        else
        {
            if (!faded && destroyer.duration - destroyer.age < 2)
            {
                faded = true;
                changeParam(change);
            }
        }
    }


}
