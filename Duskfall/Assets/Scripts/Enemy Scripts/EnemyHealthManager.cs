using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyHealthManager : MonoBehaviour
{
    [Header("Health")]
    public bool isDead;
    public bool hasShield;
    [SerializeField] private float deathDelay;
    [SerializeField] private float enemyCurrentHealth;
    [SerializeField] private float enemyMaxHealth;
    public int currentHealthSimplified;
    //public float bodyPartHealth = 100;

    [Header("Lights Variables")]
    [SerializeField] private float highHealthPercentage;
    [SerializeField] private float halfHealthPercentage;
    [SerializeField] private float lowHealthPercentage;
    [SerializeField] private Color shieldHealthColor, highHealthColor, halfHealthColor, lowHealthColor, deadHealthColor;
    [SerializeField] private float shieldHealthIntensity, highHealthIntensity, halfHealthIntensity, lowHealthIntensity, deadHealthIntensity;

    [Header("Hit Weights")]
    [SerializeField] private float headHitWeight;
    [SerializeField] private float chestHitWeight;
    [SerializeField] private float stomachHitWeight;

    [Header("Hit Weights Modifiers")]
    [SerializeField] private float headHitWeightModifier;
    [SerializeField] private float chestHitWeightModifier;
    [SerializeField] private float stomachHitWeightModifier;

    [Header("Body Parts")]
    public List<Collider> RagdollParts = new List<Collider>();

    [Header("SFX")]

    Material mat1;
    Material mat2;
    //Variables
    //NavMeshAgent navMeshAgent;
    private Animator anim;
    private EnemyAI enemyAI;
    private EnemyManager enemyManager;
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        enemyAI = GetComponent<EnemyAI>();
        enemyManager = FindObjectOfType<EnemyManager>();
        //navMeshAgent = GetComponentInChildren<NavMeshAgent>();
        mat1 = GetComponentInChildren<SkinnedMeshRenderer>().materials[0];
        mat2 = GetComponentInChildren<SkinnedMeshRenderer>().materials[1];
        anim.enabled = true;
        isDead = false;
        enemyCurrentHealth = enemyMaxHealth;
        SetRagdollParts();
    }

    private void Start() { HandleColorChange(); }

    void SetRagdollParts()
    {
        Collider[] colliders = this.gameObject.GetComponentsInChildren<Collider>();

        foreach (Collider c in colliders)
        {
            if (c.gameObject != this.gameObject && c.gameObject.layer != 2)
            {
                RagdollParts.Add(c);
                c.attachedRigidbody.isKinematic = true;
            }
        }
    }

    public void DamageEnemy(float damage, string bodyPart)
    {
        if (!isDead)
        {
            enemyCurrentHealth -= damage;
                
            //print("I took " + damage + " damage!");

            if (enemyCurrentHealth <= 0)
            {
                enemyCurrentHealth = 0;
                Die();
            }
            else
            {
                if (bodyPart == "mixamorig:Head") { StopAllCoroutines(); StartCoroutine(HeadHit(anim.GetFloat("Speed") > 0 ? headHitWeightModifier : 1)); }
                else if (bodyPart == "mixamorig:Spine1") { StopAllCoroutines(); StartCoroutine(ChestHit(anim.GetFloat("Speed") > 0 ? chestHitWeightModifier : 1)); }
                else if (bodyPart == "mixamorig:Hips") { StopAllCoroutines(); StartCoroutine(StomachHit(anim.GetFloat("Speed") > 0 ? stomachHitWeightModifier : 1)); }
            }

            HandleColorChange();
        }
    }

    private void HandleColorChange()
    {
        mat1.EnableKeyword("_EMISSION");
        mat2.EnableKeyword("_EMISSION");

        if(enemyCurrentHealth == enemyMaxHealth && hasShield)
        {
            //print("Shield");
            mat1.SetColor("_EmissionColor", shieldHealthColor * shieldHealthIntensity);
            mat2.SetColor("_EmissionColor", shieldHealthColor * shieldHealthIntensity);
            currentHealthSimplified = 0;
        }
        else if (enemyCurrentHealth == enemyMaxHealth && !hasShield)
        {
            mat1.SetColor("_EmissionColor", highHealthColor * highHealthIntensity);
            mat2.SetColor("_EmissionColor", highHealthColor * highHealthIntensity);
        }
        else if (enemyCurrentHealth <= (enemyMaxHealth * highHealthPercentage) && enemyCurrentHealth > (enemyMaxHealth * halfHealthPercentage))
        {
            //print("High Health");
            mat1.SetColor("_EmissionColor", highHealthColor * highHealthIntensity);
            mat2.SetColor("_EmissionColor", highHealthColor * highHealthIntensity);
            currentHealthSimplified = 1;
        }
        else if(enemyCurrentHealth <= (enemyMaxHealth * halfHealthPercentage) && enemyCurrentHealth > (enemyMaxHealth * lowHealthPercentage))
        {
            //print("Mid Health");
            mat1.SetColor("_EmissionColor", halfHealthColor * halfHealthIntensity);
            mat2.SetColor("_EmissionColor", halfHealthColor * halfHealthIntensity);
            currentHealthSimplified = 2;
        }
        else if(enemyCurrentHealth <= (enemyMaxHealth * lowHealthPercentage) && enemyCurrentHealth > 0)
        {
            //print("Low Health");
            mat1.SetColor("_EmissionColor", lowHealthColor * lowHealthIntensity);
            mat2.SetColor("_EmissionColor", lowHealthColor * lowHealthIntensity);
            currentHealthSimplified = 3;
        }
        else if(enemyCurrentHealth <= 0)
        {
            //print("Dead");
            mat1.SetColor("_EmissionColor", deadHealthColor * deadHealthIntensity);
            mat2.SetColor("_EmissionColor", deadHealthColor * deadHealthIntensity);
            currentHealthSimplified = 4;
        }

        DynamicGI.UpdateEnvironment();
    }

    public void Die()
    {
        rb.useGravity = false;
        //this.gameObject.GetComponent<CapsuleCollider>().enabled = false;
        anim.enabled = false;
        foreach (Collider c in RagdollParts)
        {
            c.isTrigger = false;
            c.attachedRigidbody.isKinematic = false;
            c.attachedRigidbody.velocity = Vector3.zero;
            c.GetComponent<EnemyBodyPartHealthManager>().canDie = false;
        }

        rb.velocity = Vector3.zero;

        //enemyAI.enabled = false;
        //enemyAI.navMeshAgent.enabled = false;
        enemyManager.RemoveUnit(this.gameObject);
        isDead = true;
        Destroy(gameObject, deathDelay);
    }

    #region HitAnimations
    private IEnumerator HeadHit(float modifier)
    {
        anim.SetLayerWeight(1, headHitWeight * modifier);
        anim.SetTrigger("TakeHeadHit");
        yield return new WaitForSeconds(0.75f);
        anim.SetLayerWeight(1, 0);
        anim.ResetTrigger("TakeHeadHit");
    }

    private IEnumerator ChestHit(float modifier)
    {
        anim.SetLayerWeight(2, chestHitWeight * modifier);
        anim.SetTrigger("TakeChestHit");
        yield return new WaitForSeconds(0.75f);
        anim.SetLayerWeight(2, 0);
        anim.ResetTrigger("TakeChestHit");
    }

    private IEnumerator StomachHit(float modifier)
    {
        anim.SetLayerWeight(3, stomachHitWeight * modifier);
        anim.SetTrigger("TakeStomachHit");
        yield return new WaitForSeconds(0.75f);
        anim.SetLayerWeight(3, 0);
        anim.ResetTrigger("TakeStomachHit");
    }
    #endregion
    //private void ReadyToDissolve() { readyToDissolve = true; }
}
