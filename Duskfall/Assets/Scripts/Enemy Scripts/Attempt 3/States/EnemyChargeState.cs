using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyChargeState : EnemyState
{
    bool initiated;

    public override EnemyState Tick(EnemyAI stateManager, EnemySensor sensor, Animator anim, EnemyHealthManager healthManager)
    {
        if (!initiated)
        {
            initiated = true;
        }

        anim.SetFloat("Speed", 0);
        print("Charging");

        return this;
    }

    //private void RotateTowardsTarget(Transform target, float turnSpeed, Transform stateManager)
    //{
    //    Vector3 direction = (target.position - stateManager.position).normalized;
    //    Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
    //    stateManager.rotation = Quaternion.Slerp(stateManager.rotation, lookRotation, Time.deltaTime * turnSpeed);
    //}
}
