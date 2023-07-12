using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class EnemyAttackState : EnemyState
{
    [Header("Attacking")]
    [SerializeField] private Vector2 timeBetweenAttacksMinMax;
    private float timeBetweenAttacks;
    [FormerlySerializedAs("attackAnimationDelay")]
    [SerializeField] private float attackAnimationDuration;
    [SerializeField] private float combatTurnSpeed;
    bool isFlanking, canMove, initiated;
    public Transform target;
    bool gottenAttackTime;
    private float attackTimer;
    //NavMeshAgent navMeshAgent;

    [SerializeField] private EnemyChaseState chaseState;

    //private void Awake()
    //{
    //    navMeshAgent = GetComponentInParent<NavMeshAgent>();
    //}

    public override EnemyState Tick(EnemyAI stateManager, EnemySensor sensor, Animator anim, EnemyHealthManager healthManager)
    {
        if (initiated == false)
        {
            anim.SetFloat("Speed", 0);
            canMove = true;
            initiated = true;
            //navMeshAgent.enabled = false;
        }

        if (canMove)
        {
            if (Vector3.Distance(transform.position, target.position) <= chaseState.attackRange)
            {
                if (!gottenAttackTime) { GetRandomAttackTime(); gottenAttackTime = true; }
                if (attackTimer >= timeBetweenAttacks) { StartCoroutine(PerformAttack(anim)); attackTimer = 0; gottenAttackTime = false; }
                else { RotateTowardsTarget(target, combatTurnSpeed); attackTimer += Time.deltaTime; }
            }
            else
            {
                //state = 2;
                attackTimer = 0;
                initiated = false;
            }

            if (Vector3.Distance(stateManager.transform.position, sensor.target.position) < chaseState.attackRange) { return this; }
            else { initiated = false; return chaseState; }
        }

        return this;
    }

    void GetRandomAttackTime()
    {
        timeBetweenAttacks = Random.Range(timeBetweenAttacksMinMax.x, timeBetweenAttacksMinMax.y);
    }

    //this is so that the player can dodge out of the way of the attack
    private IEnumerator PerformAttack(Animator anim)
    {
        canMove = false;
        anim.SetTrigger("Attack");
        yield return new WaitForSeconds(attackAnimationDuration);
        //navMeshAgent.enabled = false;
        canMove = true;
    }

    private void RotateTowardsTarget(Transform target, float turnSpeed)
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * turnSpeed);
    }
}
