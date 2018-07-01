using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnCasterToOlderPosition : MonoBehaviour
{
    public enum StartOrEnd
    {
        Start, End
    }

    public float positionAge = 2f;
    public StartOrEnd whenToMoveCaster = StartOrEnd.Start;

    public bool restoreHealth = false;
    public bool restoreMana = false;

    public bool increasePositionAgeWithAge = false;

    public float additionalAgeForHealthRestoration = 0f;
    public float additionalAgeForManaRestoration = 0f;

    public void Start()
    {
        if (whenToMoveCaster == StartOrEnd.Start)
        {
            MoveCaster();
        }
        else
        {
            SelfDestroyer destroyer = GetComponent<SelfDestroyer>();
            if (destroyer) { destroyer.deathEvent += MoveCaster; }
        }
    }

    public void Update()
    {
        if (increasePositionAgeWithAge) { positionAge += Time.deltaTime; }
    }

    public void MoveCaster()
    {
        CreationReferences references = GetComponent<CreationReferences>();
        if (references && references.creator)
        {
            PositionRecorder recorder = references.creator.GetComponent<PositionRecorder>();
            if (recorder)
            {
                UsingAbility ua = references.creator.GetComponent<UsingAbility>();
                if (ua)
                {
                    if (ua.getController().currentState == ua.getController().dying)
                    {
                        return;
                    }

                    if (ua.movementFromAbility && ua.movementFromAbility.moving)
                    {
                        ua.movementFromAbility.onReachingDestination();
                    }

                    StateController controller = ua.getController();
                    if (controller)
                    {
                        controller.forceChangeState(controller.waiting);
                    }
                }

                Vector3 newPosition = recorder.getPositionFromDuration(positionAge);
                references.creator.transform.eulerAngles = recorder.getAngleFromDuration(positionAge);
                references.creator.transform.position = newPosition;

                if (restoreHealth && recorder.health)
                {
                    recorder.health.currentHealth = Mathf.Clamp(recorder.getHealthFromDuration(positionAge + additionalAgeForHealthRestoration), 1f, recorder.health.maxHealth);
                }

                if (restoreMana && recorder.mana)
                {
                    recorder.mana.currentMana = Mathf.Min(recorder.getManaFromDuration(positionAge + additionalAgeForManaRestoration), recorder.mana.maxMana);
                }
            }
        }
    }


}
