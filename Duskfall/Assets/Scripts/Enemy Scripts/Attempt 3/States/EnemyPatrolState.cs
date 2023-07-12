using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrolState : EnemyState
{
    [Header("Patroling")]
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float waitTime;
    [SerializeField] private float patrolMoveSpeed;
    [SerializeField] private float patrolTurnSpeed;
    [SerializeField] private float patrolStoppingDistance = 1;
    private float patrolTimer;
    private int _currentWaypointIndex = 0;
    bool isWaiting;

    [Header("Player Detection")]
    [SerializeField] private float playerRecognitionTime;
    //[SerializeField] private Transform target;
    //private Transform secondaryTarget;
    private float playerDetectionTimer;
    bool recognizesPlayer = false;

    bool initiated;
    public EnemyCombatState combatState;

    public override EnemyState Tick(EnemyAI stateManager, EnemySensor sensor, Animator anim, EnemyHealthManager healthManager)
    {
        if (!initiated)
        {
            anim.SetFloat("Speed", 1); anim.SetBool("SeesPlayer", false);
            isWaiting = false;
            initiated = true;
            recognizesPlayer = false;
            //stateManager.GetComponent<NavMeshAgent>().enabled = false;
        }

        if (!sensor.canSeePlayer)
        {
            //waiting
            if (isWaiting)
            {
                patrolTimer += Time.deltaTime;

                //if the timer waits long enough
                if (patrolTimer >= waitTime)
                {
                    anim.SetFloat("Speed", 1);
                    isWaiting = false;
                }
            }
            //not waiting any more
            else
            {
                //sets the waypoint to walk to
                Transform wp = waypoints[_currentWaypointIndex];

                //if the enemy is not at the waypoint yet
                if (Vector3.Distance(stateManager.transform.position, wp.position) > patrolStoppingDistance)
                {
                    stateManager.transform.position = Vector3.MoveTowards(stateManager.transform.position, wp.position, patrolMoveSpeed * Time.deltaTime);
                    //Rotate towards the target
                    RotateTowardsTarget(wp, patrolTurnSpeed, stateManager.transform);

                }
                //if the enemy has reached the waypoint
                else
                {
                    if (waitTime != 0) { anim.SetFloat("Speed", 0); }
                    patrolTimer = 0;
                    isWaiting = true;
                    _currentWaypointIndex = (_currentWaypointIndex + 1) % waypoints.Length;
                }
            }
        }
        else
        {
            TryToRecognizePlayer();
        }

        if (recognizesPlayer) { initiated = false; return combatState; }
        else { return this; }
    }

    private void RotateTowardsTarget(Transform target, float turnSpeed, Transform stateManager)
    {
        Vector3 direction = (target.position - stateManager.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        stateManager.rotation = Quaternion.Slerp(stateManager.rotation, lookRotation, Time.deltaTime * turnSpeed);
    }

    void TryToRecognizePlayer()
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
            recognizesPlayer = true;
            print("I see you!");
        }
        else { playerDetectionTimer += Time.deltaTime; }
    }
}
