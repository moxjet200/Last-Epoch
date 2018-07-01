using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityEventListener : MonoBehaviour
{

    public delegate void OnHitAction(Ability ability, GameObject target);
    public event OnHitAction onHitEvent;

    public delegate void OnCritAction(Ability ability, GameObject target);
    public event OnCritAction onCritEvent;

    public delegate void OnKillAction(Ability ability, GameObject target);
    public event OnKillAction onKillEvent;


    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void HitEvent(Ability ability, GameObject target)
    {
        if (onHitEvent != null) { onHitEvent.Invoke(ability, target); }
    }

    public void CritEvent(Ability ability, GameObject target)
    {
        if (onCritEvent != null) { onCritEvent.Invoke(ability, target); }
    }

    public void KillEvent(Ability ability, GameObject target)
    {
        if (onKillEvent != null) { onKillEvent.Invoke(ability, target); }
    }

    public static AbilityEventListener GetOrAdd(GameObject _gameObject)
    {
        AbilityEventListener ael = _gameObject.GetComponent<AbilityEventListener>();
        if (ael) { return ael; }
        else
        {
            return _gameObject.AddComponent<AbilityEventListener>();
        }
    }

}
