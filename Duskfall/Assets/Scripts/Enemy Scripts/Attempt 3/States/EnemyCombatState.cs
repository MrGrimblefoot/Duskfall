using UnityEngine;

public class EnemyCombatState : EnemyState
{
    [Header("Variables")]
    [SerializeField] private bool foolhardy;
    [SerializeField] private LayerMask isAlly;
    [SerializeField] private float allyCheckRadius = 20f;
    [SerializeField] GameObject parentObj;
    EnemyManager manager;
    public int aggression;
    public Collider[] allies;

    [Header("States")]
    [SerializeField] private EnemyPatrolState patrolState;
    [SerializeField] private EnemyChaseState chaseState;
    [SerializeField] private EnemyChargeState chargeState;
    [SerializeField] private EnemyFleeState fleeState;
    [SerializeField] private EnemyWaitState waitState;
    EnemyState stateToReturn;

    private void Start()
    {
        manager = FindObjectOfType<EnemyManager>().GetComponent<EnemyManager>(); 
    }

    public override EnemyState Tick(EnemyAI stateManager, EnemySensor sensor, Animator anim, EnemyHealthManager healthManager)
    {
        EvaluateAggression(healthManager, manager);

        DetermineAction();

        return stateToReturn;
    }

    void EvaluateAggression(EnemyHealthManager healthManager, EnemyManager manager)
    {
        if (!manager.alertedUnits.Contains(parentObj)) { manager.alertedUnits.Add(parentObj); }

        aggression = 2;

        allies = Physics.OverlapSphere(transform.position, allyCheckRadius, isAlly);

        //if has shield
        if (healthManager.hasShield && healthManager.currentHealthSimplified == 0) { aggression += 2; }
        //if at high health but shield is broken
        else if (healthManager.currentHealthSimplified == 1) { aggression += 1; }

        //if at medium health
        if (healthManager.currentHealthSimplified == 2) { aggression -= 1; }
        //if at low health and is not foolhardy
        else if (healthManager.currentHealthSimplified == 3 && !foolhardy) { aggression = 0; }
        //if at low health and is foolhardy
        else if (foolhardy) { aggression -= 1; }

        //if it has lots of living allies
        if (manager.alertedUnits.Count < 3) { aggression += 1; }

        //if it's the last one and it's not foolhardy
        if(allies.Length < 1)
        {
            if (!foolhardy) { aggression = 0; print("3"); }
            //if it's the last one and it is foolhardy
            else if (foolhardy) { aggression = 4; print("4"); }
        }

        print(parentObj.name + "'s aggression level is " + aggression + ".");
    }

    void DetermineAction()
    {
        switch (aggression)
        {
            case 0:
                stateToReturn = fleeState;
                break;

            case 1:
                stateToReturn = waitState;
                break;

            case 2:
                int randomNum = Random.Range(1, 101);
                if(randomNum > 50) { stateToReturn = waitState; }
                else { stateToReturn = chaseState; }
                break;

            case 3:
                stateToReturn = chaseState;
                break;

            case < 3:
                if (manager.unitCharging == null) { stateToReturn = chargeState; manager.unitCharging = parentObj; }
                else if(manager.unitCharging != parentObj) { stateToReturn = chaseState; }
                break;

            default:
                break;
        }
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(transform.position, allyCheckRadius);
    //}
}
