using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private EnemyState startingState;
    [HideInInspector] public NavMeshAgent navMeshAgent;
    private EnemySensor sensor;
    private EnemyHealthManager healthManager;
    private Animator anim;
    private Flank flank;
    public EnemyState currentState;
    
    void Start()
    {
        sensor = GetComponentInChildren<EnemySensor>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        healthManager = GetComponent<EnemyHealthManager>();
        anim = GetComponent<Animator>();
        flank = FindObjectOfType<Flank>();
        currentState = startingState;
    }

    void FixedUpdate()
    {
        HandleStateMachine();
    }

    private void HandleStateMachine()
    {
        EnemyState nextState;

        if(currentState != null)
        {
            nextState = currentState.Tick(this, sensor, anim, healthManager);

            if(nextState != null)
            {
                currentState = nextState;
            }
        }
    }

    public void ReevaluateAction() { currentState = GetComponentInChildren<EnemyCombatState>(); }
}
