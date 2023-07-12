using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySensor : MonoBehaviour
{
    public float radius;
    [Range(0, 360)]
    public float angle;
    [SerializeField] private float scanInterval;
    public GameObject playerRef;
    [SerializeField] private LayerMask targetLayer, obstructionLayer;
    public bool canSeePlayer;
    public Transform target;

    private void Start()
    {
        playerRef = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(ScanRoutine());
    }

    private IEnumerator ScanRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(scanInterval);

        while (true)
        {
            yield return wait;
            Scan();
        }
    }

    private void Scan()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetLayer);

        if (rangeChecks.Length != 0)
        {
            target = rangeChecks[0].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, dirToTarget) < angle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, dirToTarget, distanceToTarget, obstructionLayer))
                {
                    canSeePlayer = true;
                }
                else { canSeePlayer = false; }
            }
            else { canSeePlayer = false; }
        }
        else if (canSeePlayer) { canSeePlayer = false; target = null; }
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    if(target != null)
    //    Gizmos.DrawLine(transform.position, target.position);
    //}
}