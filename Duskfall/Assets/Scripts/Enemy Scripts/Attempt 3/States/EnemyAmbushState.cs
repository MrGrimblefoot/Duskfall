using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAmbushState : EnemyState
{
    [Header("Player Detection")]
    public float playerRecognitionTime;
    [SerializeField] private Transform target;
    private Transform secondaryTarget;
    private float playerDetectionTimer;

    [SerializeField] private EnemyChaseState chaseState;

    public override EnemyState Tick(EnemyAI stateManager, EnemySensor sensor, Animator anim, EnemyHealthManager healthManager)
    {
        if (playerDetectionTimer >= playerRecognitionTime)
        {
            //if (!flank.isOccupied)
            //{
            //    target = flank.transform;
            //    secondaryTarget = sensor.target;
            //    flank.isOccupied = true;
            //}
            //else { target = sensor.target; secondaryTarget = null; }
            //initiated = false;
            target = sensor.target;
        }
        else { playerDetectionTimer += Time.deltaTime; }

        if(target != null) { return chaseState; }
        return this;
    }
}
