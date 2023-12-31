using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class PlayerPolishManager : MonoBehaviour
{
    #region Variables
    [Header("Health Parameters")]
    public int maxHealth = 100;
    public int currentHealth;
    [SerializeField] private float timeBeforeRegenStarts = 3f;
    [SerializeField] private int healthValueIncrement = 1;
    [SerializeField] private float healthTimeIncrement = 0.1f;
    public bool isDead;
    public bool canTakeDamage;
    //private Coroutine regeneratingHealth;
    public static Action<int> OnDamage;
    public static Action<int> OnHeal;
    public static Action OnPlayerDie;
    public static Action OnHealToMax;
    public static Action OnInvulnerablity;
    HealthBarComplex healthBar;

    [Header("Headbob Parameters")]
    [SerializeField] private Transform weaponParent;
    [SerializeField] private float idleBobMultiplier;
    [SerializeField] private float idleXMultiplier;
    [SerializeField] private float idleYMultiplier;
    [SerializeField] private float idleTransitionSmooth;
    [SerializeField] private float movementBobMultiplier;
    [SerializeField] private float movementXMultiplier;
    [SerializeField] private float movementYMultiplier;
    [SerializeField] private float movementTransitionSmooth;
    [SerializeField] private float sprintBobMultiplier;
    [SerializeField] private float sprintXMultiplier;
    [SerializeField] private float sprintYMultiplier;
    [SerializeField] private float sprintTransitionSmooth;
    [SerializeField] private float crouchBobMultiplier;
    [SerializeField] private float crouchXMultiplier;
    [SerializeField] private float crouchYMultiplier;
    [SerializeField] private float crouchTransitionSmooth;
    private Vector3 weaponParentOrigin;
    private Vector3 targetWeaponBob;

    [Header("UI Parameters")]
    private Transform uiHealthBar;
    private Image damageRecievedIndicator;
    private float damageRecievedIndicatorWaitTime;
    private Color transparentRed = new Color(1, 0, 0, 0);
    #endregion

    private void OnEnable()
    {
        OnPlayerDie += PlayerDie;
        OnHealToMax += HealToMax;
        OnInvulnerablity += Invulnerablity;
    }

    private void OnDisable()
    {
        OnPlayerDie -= PlayerDie;
        OnHealToMax -= HealToMax;
        OnInvulnerablity -= Invulnerablity;
    }

    void Awake()
    {

        weaponParentOrigin = weaponParent.localPosition;
        currentHealth = maxHealth;
        healthBar = FindObjectOfType<HealthBarComplex>();
        uiHealthBar = GameObject.Find("HUD/Healthbar/Health").transform;
        damageRecievedIndicator = GameObject.Find("HUD/Recieve Hit Effect").GetComponent<Image>();
        damageRecievedIndicator.color = Color.clear;
        UpdateHealthBar();
    }

    void Update()
    {
        //if (useFootsteps) { HandleFootsteps(); }

        UpdateHealthBar();

        if (damageRecievedIndicatorWaitTime > 0) { damageRecievedIndicatorWaitTime -= Time.deltaTime; }
        else if (damageRecievedIndicator.color.a > 0) { damageRecievedIndicator.color = Color.Lerp(damageRecievedIndicator.color, transparentRed, Time.deltaTime * 8); }
        if (transform.position.y <= -10) { PlayerDie(); }
    }

    private void UpdateHealthBar()
    {
        float tempHealthRatio = (float)currentHealth / (float)maxHealth;
        uiHealthBar.localScale = Vector3.Lerp(uiHealthBar.localScale, new Vector3(tempHealthRatio, 1, 1), 3f);
    }

    public void ApplyDamage(int damage)
    {
        if (canTakeDamage)
        {
            currentHealth -= damage;
            UpdateHealthBar();

            //if (no one is listening for OnDamage) { return; } else { Invoke(currentHealth); }
            OnDamage?.Invoke(currentHealth);

            if (currentHealth <= 0) { PlayerDie(); }
            //else if (regeneratingHealth != null) { StopCoroutine(regeneratingHealth); }

            //regeneratingHealth = StartCoroutine(RegenerateHealth());

            //healthBar.UpdateHealthUI();
            //healthBar.ResetHealthLerpTimer();
            damageRecievedIndicator.color = Color.red;
            damageRecievedIndicatorWaitTime += 0.2f;

            Debug.Log("I took " + damage + " damage.");
        }
    }

    private void PlayerDie()
    {
        currentHealth = 0;
        damageRecievedIndicator.color = transparentRed;
        UpdateHealthBar();
        isDead = true;

        //if(regeneratingHealth != null) { StopCoroutine(regeneratingHealth); }

        //Destroy(gameObject);
    }

    //private void HandleFootsteps()
    //{
    //    if (!movement.grounded || movement.currentInput == Vector2.zero) { return; }

    //    footstepTimer -= Time.deltaTime;

    //    if (footstepTimer <= 0)
    //    {
    //        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 3))
    //        {
    //            switch (hit.collider.tag)
    //            {
    //                case "Footsteps/Wood":
    //                    footstepAudioSource.PlayOneShot(woodClips[UnityEngine.Random.Range(0, woodClips.Length - 1)]);
    //                    break;
    //                case "Footsteps/Metal":
    //                    footstepAudioSource.PlayOneShot(metalClips[UnityEngine.Random.Range(0, metalClips.Length - 1)]);
    //                    break;
    //                case "Footsteps/Grass":
    //                    footstepAudioSource.PlayOneShot(grassClips[UnityEngine.Random.Range(0, grassClips.Length - 1)]);
    //                    break;
    //                case "Footsteps/Concrete":
    //                    footstepAudioSource.PlayOneShot(concreteClips[UnityEngine.Random.Range(0, concreteClips.Length - 1)]);
    //                    break;
    //                default:
    //                    footstepAudioSource.PlayOneShot(concreteClips[UnityEngine.Random.Range(0, concreteClips.Length - 1)]);
    //                    break;
    //            }
    //        }

    //        footstepTimer = GetCurrentOffset;
    //    }
    //}

    private IEnumerator RegenerateHealth()
    {
        yield return new WaitForSeconds(timeBeforeRegenStarts);
        WaitForSeconds timeToWait = new WaitForSeconds(healthTimeIncrement);

        while (currentHealth < maxHealth)
        {
            currentHealth += healthValueIncrement;
            healthBar.ResetHealthLerpTimer();

            if (currentHealth > maxHealth) { currentHealth = maxHealth; }

            //if (no one is listening for OnHeal) { return; } else { Invoke(currentHealth); }
            OnHeal?.Invoke(currentHealth);
            yield return timeToWait;
        }

        //regeneratingHealth = null;
    }

    private void ChangeLayersRecursively(GameObject p_target, int p_layer)
    {
        p_target.layer = p_layer;
        foreach (Transform a in p_target.transform) { ChangeLayersRecursively(a.gameObject, p_layer); }
    }

    private void HealToMax() { currentHealth = maxHealth; isDead = false; }

    private void Invulnerablity() { if (!isDead) { canTakeDamage = !canTakeDamage; } }
}
