using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class ChangeParameterAtStartOrEnd : ParameterChanger{
    
    public EnableEnum start = EnableEnum.NoChange;
    public EnableEnum end = EnableEnum.NoChange;

    void Start () {
        emitter = GetComponent<StudioEventEmitter>();
        changeParam(start);
	}
	
	void OnDestroy () {
        changeParam(end);
    }
    
}
