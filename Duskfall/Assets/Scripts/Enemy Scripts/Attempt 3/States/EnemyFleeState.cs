using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFleeState : EnemyState
{
    bool initiated;

    public override EnemyState Tick(EnemyAI stateManager, EnemySensor sensor, Animator anim, EnemyHealthManager healthManager)
    {
        if (!initiated)
        {
            initiated = true;
        }

        anim.SetFloat("Speed", 0);
        print("Fleeing");

        return this;
    }
}
