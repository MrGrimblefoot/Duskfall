using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyChaseState : EnemyState
{
    bool initiated;
    //NavMeshAgent navMeshAgent;
    Transform target;
    public float attackRange, abandonRange;
    [SerializeField] private float chaseTurnSpeed;
    [SerializeField] private EnemyAttackState attackState;
    [SerializeField] private EnemyPatrolState patrolState;

    //private void Awake()
    //{
    //    navMeshAgent = GetComponentInParent<NavMeshAgent>();
    //}

    public override EnemyState Tick(EnemyAI stateManager, EnemySensor sensor, Animator anim, EnemyHealthManager healthManager)
    {
        if (!initiated)
        {
            //navMeshAgent.enabled = true;
            initiated = true;
            anim.SetFloat("Speed", 2); anim.SetBool("SeesPlayer", true);
            if(sensor.target != null) { target = sensor.target; attackState.target = target; }
            //if (isFlanking) { attackRange -= 1; }
        }

        //navMeshAgent.SetDestination(target.transform.position);
        RotateTowardsTarget(target, chaseTurnSpeed, stateManager.transform);

        if (Vector3.Distance(stateManager.transform.position, target.position) < attackRange) { initiated = false; return attackState; }
        else if(Vector3.Distance(stateManager.transform.position, target.position) > abandonRange) { initiated = false; return patrolState; }
        else { return this; }
    }

    private void RotateTowardsTarget(Transform target, float turnSpeed, Transform stateManager)
    {
        Vector3 direction = (target.position - stateManager.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        stateManager.rotation = Quaternion.Slerp(stateManager.rotation, lookRotation, Time.deltaTime * turnSpeed);
    }
}
