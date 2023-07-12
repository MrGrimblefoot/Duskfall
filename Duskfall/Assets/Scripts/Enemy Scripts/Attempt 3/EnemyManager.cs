using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(0)]
public class EnemyManager : MonoBehaviour
{
    public GameObject unitCharging;
    public List<GameObject> alertedUnits = new List<GameObject>();
    public List<GameObject> engagedUnits = new List<GameObject>();

    public void RemoveUnit(GameObject unitToRemove)
    {
        if (engagedUnits.Contains(unitToRemove)) { engagedUnits.Remove(unitToRemove); }
        alertedUnits.Remove(unitToRemove);
        foreach (GameObject unit in alertedUnits)
        {
            unit.GetComponentInChildren<EnemyAI>().ReevaluateAction();
        }
    }
}
