using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class ParameterChanger : MonoBehaviour {

    public string paramater = "";
    protected StudioEventEmitter emitter = null;

    public void changeParam(EnableEnum enable)
    {
        if (!emitter) { return; }
        if (enable == EnableEnum.NoChange) { return; }
        else if (enable == EnableEnum.Enable && emitter)
        {
            emitter.SetParameter(paramater, 1f);
        }
        else if (enable == EnableEnum.Disable && emitter)
        {
            emitter.SetParameter(paramater, 0f);
        }
    }
}
