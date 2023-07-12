using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyState : MonoBehaviour
{
    public abstract EnemyState Tick(EnemyAI stateManager, EnemySensor sensor, Animator anim, EnemyHealthManager healthManager);
}
