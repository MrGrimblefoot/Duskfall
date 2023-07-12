using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SimpleEnemyHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth;
    [SerializeField] private float deathDelay;
    private EnemyAI ai;
    private NavMeshAgent agent;
    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
        ai = GetComponent<EnemyAI>();
        agent = GetComponent<NavMeshAgent>();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if(currentHealth <= 0) { Die(); }
    }

    void Die()
    {
        ai.enabled = false;
        agent.enabled = false;
        Destroy(gameObject, deathDelay);
    }
}
