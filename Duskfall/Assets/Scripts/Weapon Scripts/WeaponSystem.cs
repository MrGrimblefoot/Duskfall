using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using EZCameraShake;
using System;
using Random = UnityEngine.Random;

public class WeaponSystem : MonoBehaviour
{
    #region Variables
    #region Loadout
    [Header("Guns")]
    /*[SerializeField] private*/ public Weapon[] loadout;
    [HideInInspector] public Weapon currentWeaponData;
    public GameObject currentWeapon;
    [SerializeField] private GameObject sight;
    public bool hasSight;
    #endregion

    #region Shooting
    private int bulletsToShoot;
    [SerializeField] private Camera cam;
    [SerializeField] private Camera weaponCam;
    private GameObject sniperCam;
    private RaycastHit hit;
    public bool shooting;
    public bool canShoot;
    private bool readyToShoot;
    public bool isReloading;
    public bool canSwitch;
    public bool hasResetRecoilPattern;
    private float currentCooldown;
    //[SerializeField] private string damageTag1, damageTag2, damageTag3;
    [SerializeField] private int damageLayer1, damageLayer2, damageLayer3;
    #endregion

    #region Inputs
    private BasicInputActions basicInputActions;
    [SerializeField] private bool canReciveInput;
    [SerializeField] private bool isSwitching;
    [SerializeField] private float equipDelay;
    #endregion

    #region Aiming
    [HideInInspector]
    public bool aiming;
    private float targetFOV;
    private float weaponTargetFOV;
    [SerializeField] private float normalFOV = 60;
    [SerializeField] private CameraLook camLook;
    #endregion

    #region Recoil
    private Recoil recoilScript;
    #endregion

    #region Equiping
    [Header("Equiping")]
    [SerializeField] private Transform weaponParent;
    [HideInInspector] public int currentIndex;
    #endregion

    #region UI
    private GameObject cursor;
    private TextMeshProUGUI ammoCounterText;
    //private Image hitMarkerImage;
    //private float hitMarkerWaitTime;
    //private Color transparentWhite = new Color(1, 1, 1, 0);
    #endregion

    #region SFX
    [Header("SFX")]
    [SerializeField] private AudioSource sfx;
    [SerializeField] private AudioClip hitMarkerSound;
    #endregion

    #region VFX
    [Header("VFX")]
    private Transform firePoint;
    [SerializeField] private TrailRenderer bulletTrail;
    [SerializeField] private Color normalColor;
    [SerializeField] private Color headshotColor;
    [SerializeField] private ParticleSystem muzzleFlash;
    #endregion

    #region AmmoStorage
    [HideInInspector] public int shotgunAmmoInMagazine;
    [HideInInspector] public int shotgunAmmoInStash;
    [HideInInspector] public int pistolAmmoInMagazine;
    [HideInInspector] public int pistolAmmoInStash;
    private int weaponType;
    #endregion

    //Logic
    public bool hasHit;

    public static Action OnReplenishAmmo;
    #endregion

    #region MonoBehaviour Callbacks
    private void OnEnable()
    {
        basicInputActions.Player.Reload.Enable();
        basicInputActions.Player.Fire.Enable();
        basicInputActions.Player.Weapon1.Enable();
        basicInputActions.Player.Weapon2.Enable();
        basicInputActions.Player.Weapon3.Enable();
        basicInputActions.Player.Fire2.Enable();
        OnReplenishAmmo += ReplenishAmmo;
    }

    private void OnDisable()
    {
        basicInputActions.Player.Reload.Disable();
        basicInputActions.Player.Fire.Disable();
        basicInputActions.Player.Weapon1.Disable();
        basicInputActions.Player.Weapon2.Disable();
        basicInputActions.Player.Weapon3.Disable();
        basicInputActions.Player.Fire2.Disable();
        OnReplenishAmmo -= ReplenishAmmo;
    }

    private void Awake()
    {
        cursor = GameObject.Find("Crosshair"); cursor.SetActive(false);
        recoilScript = FindObjectOfType<Recoil>().GetComponent<Recoil>();
        readyToShoot = false;
        //hitMarkerImage = GameObject.Find("HUD/Hit Marker").GetComponent<Image>();
        //hitMarkerImage.color = transparentWhite;
        ammoCounterText = GameObject.Find("HUD/Ammo Display").GetComponent<TextMeshProUGUI>();
        canReciveInput = true;
        canSwitch = true;
        hasResetRecoilPattern = true;
        hasHit = false;

        #region InputActions
        basicInputActions = new BasicInputActions();
        basicInputActions.Player.Reload.performed += Reload;
        basicInputActions.Player.Weapon1.performed += Equip1;
        basicInputActions.Player.Weapon2.performed += Equip2;
        basicInputActions.Player.Weapon3.performed += Equip3;
        #endregion

        foreach (Weapon g in loadout) { g.Initialize(); }
        StartCoroutine(Equip(0));
        normalFOV = camLook.fieldOfView;
    }

    private void Update()
    {
        if (currentWeapon != null)
        {
            //if (currentGunData.isAutomatic) { shooting = basicInputActions.Player.Movement.ReadValue<bool>(); }
            //else { shooting = Input.GetKeyDown(shootButton); }

            Aim();

            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.deltaTime * currentWeaponData.cameraZoomSpeed);
            weaponCam.fieldOfView = Mathf.Lerp(weaponCam.fieldOfView, weaponTargetFOV, Time.deltaTime * currentWeaponData.cameraZoomSpeed);
        }

        if (currentCooldown > 0) { currentCooldown -= Time.deltaTime; }

        //if (hitMarkerWaitTime > 0) { hitMarkerWaitTime -= Time.deltaTime; }
        //else if (hitMarkerImage.color.a > 0) { hitMarkerImage.color = Color.Lerp(hitMarkerImage.color, transparentWhite, Time.deltaTime * 8); }

        if (canReciveInput) { HandleInput(); }
    }
    #endregion

    #region Private Methods
    #region Input
    private void HandleInput()
    {
        if (currentWeapon != null)
        {
            if (!currentWeaponData.isMelee)
            {
                if (readyToShoot && shooting && !isReloading && currentCooldown <= 0 && currentWeaponData.currentBulletsInMagazine > 0)
                {
                    if (currentWeaponData.UpdateMagazine()) { bulletsToShoot = currentWeaponData.bulletsPerTap; Shoot(); }
                }

                if (readyToShoot && shooting && !isReloading && currentCooldown <= 0 && currentWeaponData.currentBulletsInMagazine <= 0 && currentWeaponData.currentAmmoStash > 0)
                {
                    StartCoroutine(HandleReload());
                }
            }
            else { Attack(); }
        }
    }
    #endregion

    #region Equipping
    private void Equip1(InputAction.CallbackContext context) { StartCoroutine(Equip(0)); }
    private void Equip2(InputAction.CallbackContext context) { StartCoroutine(Equip(1)); }
    private void Equip3(InputAction.CallbackContext context) { StartCoroutine(Equip(2)); }

    public IEnumerator Equip(int p_ind)
    {
        if (!isSwitching && !isReloading && canSwitch)
        {
            isSwitching = true;
            if(sight != null) { sight = null; }
                
            Stow();

            yield return new WaitForSeconds(equipDelay);

            currentIndex = p_ind;

            GameObject t_newWeapon = Instantiate(loadout[currentIndex].prefab, weaponParent.position, weaponParent.rotation, weaponParent) as GameObject;
            t_newWeapon.transform.localPosition = Vector3.zero;
            t_newWeapon.transform.localEulerAngles = Vector3.zero;

            currentWeapon = t_newWeapon;
            currentWeaponData = loadout[currentIndex];
            recoilScript.gun = currentWeaponData;
            recoilScript.weaponAnchor = currentWeapon.transform.Find("Anchor/SecondaryAnchor").transform;
            recoilScript.originRotation = recoilScript.weaponAnchor.localRotation;

            targetFOV = 60;
            weaponTargetFOV = 60;

            firePoint = currentWeapon.transform.Find("Anchor/SecondaryAnchor/Fire Point");

            if (currentWeaponData.isMelee/*isAutomatic*/)
            {
                basicInputActions.Player.Fire.started += _ => shooting = true;
                basicInputActions.Player.Fire.canceled += _ => shooting = false;
                cursor.SetActive(true);
                weaponType = 0;
            }
            else
            {
                basicInputActions.Player.Fire.performed += _ => shooting = true;
                basicInputActions.Player.Fire2.started += _ => aiming = true;
                basicInputActions.Player.Fire2.canceled += _ => aiming = false;
                cursor.SetActive(false);
                if (currentWeapon.tag == "Pistol") { weaponType = 1; }
                else if(currentWeapon.tag == "Shotgun") { weaponType = 2; }
            }

            if (currentWeaponData.useMuzzleFlash) { muzzleFlash = FindObjectOfType<ParticleSystem>(); }
            else { ManageAmmo(0, 3, weaponType); }
            UpdateAmmoUI(currentWeaponData.isMelee);

            readyToShoot = true;

            HandleSightSpawning();

            isSwitching = false;
        }

        StopCoroutine(Equip(p_ind));
    }

    private void HandleSightSpawning()
    {
        if (currentWeaponData.isPistol)
        {
            GameObject sight = currentWeapon.transform.Find("Anchor/T77/Reflex Sight").gameObject;
            if (hasSight) { sight.SetActive(true); }
            else { sight.SetActive(false); }
        }
    }

    public void Stow()
    {
        if (currentWeapon != null)
        {
            if (currentWeaponData.isMelee) { currentWeapon.GetComponentInChildren<Animator>().SetTrigger("Stow"); }
            if (!currentWeaponData.isMelee)
            {
                Animator anim = currentWeapon.GetComponentInChildren<Animator>();
                anim.enabled = true;
                anim.SetBool("Stow", true);
            }
        }
    }

    public void Draw()
    {
        if (currentWeapon != null)
        {
            if (currentWeaponData.isMelee) { currentWeapon.GetComponentInChildren<Animator>().SetTrigger("Draw"); }
            if (!currentWeaponData.isMelee) { currentWeapon.GetComponentInChildren<Animator>().SetBool("Stow", false); }
        }
    }
    #endregion

    #region RememberAmmo
    public void ManageAmmo(int ammoToAdd, int addRememberAssign, int ammoType)
    {
        if (addRememberAssign == 1)
        {
            //print("1");
            //If pistol
            if (ammoType == 1)
            {
                pistolAmmoInStash = pistolAmmoInStash + ammoToAdd;
                ManageAmmo(0, 3, 1);
            }
            //if shotgun
            else if (ammoType == 2)
            {
                shotgunAmmoInStash = shotgunAmmoInStash + ammoToAdd;
                ManageAmmo(0, 3, 2);
            }
        }
        else if (addRememberAssign == 2)
        {
            //print("2");
            if (currentWeapon.tag == "Pistol")
            {
                //print("pistol");
                pistolAmmoInMagazine = currentWeaponData.currentBulletsInMagazine;
                pistolAmmoInStash = currentWeaponData.currentAmmoStash;
            }

            else if (currentWeapon.tag == "Shotgun")
            {
                //print("shotgun");
                shotgunAmmoInMagazine = currentWeaponData.currentBulletsInMagazine;
                shotgunAmmoInStash = currentWeaponData.currentAmmoStash;
            }
        }
        else if (addRememberAssign == 3)
        {
            //print("3");
            //Check if pistol ammo is being assigned && double check that the pistol is active
            if (ammoType == 1 && weaponType == 1)
            {
                //print("Pistol Ammo");
                currentWeaponData.AssignAmmo(pistolAmmoInMagazine, pistolAmmoInStash);
            }
            //Check if shotgun ammo is being assigned && double check that the shotgun is active
            if (ammoType == 2 && weaponType == 2)
            {
                //print("Shotgun Ammo");
                currentWeaponData.AssignAmmo(shotgunAmmoInMagazine, shotgunAmmoInStash);
            }
        }
    }
    #endregion

    #region Reload
    private void Reload(InputAction.CallbackContext context)
    {
        if (!isReloading && currentWeaponData.GetMag() <= currentWeaponData.magazineSize - 1 && currentWeaponData.GetStash() > 0) { StartCoroutine(HandleReload()); }
    }

    private IEnumerator HandleReload()
    {
        if(currentWeapon != null)
        {
            isReloading = true;
            //currentWeapon.SetActive(false);

            yield return new WaitForSeconds(currentWeaponData.reloadTime);

            currentWeaponData.Reload();
            ManageAmmo(0, 2, 0);
            UpdateAmmoUI(false);
            //currentWeapon.SetActive(true);
            isReloading = false;
            shooting = false;
        }
    }
    #endregion

    #region Aiming
    public void Aim()
    {
        if(!currentWeapon) { return; }

        //find the Anchor
        Transform tempAnchor = currentWeapon.transform.Find("Anchor");
        Transform tempStateADS = currentWeapon.transform.Find("States/ADS");
        Transform tempStateHip = currentWeapon.transform.Find("States/Hip");

        //determine gun position
        if (!currentWeaponData.isMelee)
        {
            if (aiming)
            {
                tempAnchor.position = Vector3.Lerp(tempAnchor.position,
                    new Vector3(tempStateADS.position.x, hasSight && currentWeaponData.isPistol ? tempStateADS.position.y + currentWeaponData.sightOffset : tempStateADS.position.y, tempStateADS.position.z),
                    Time.deltaTime * (currentWeaponData.ADSSpeed * currentWeaponData.aimPosKickReturnSpeed));
                targetFOV = normalFOV / currentWeaponData.playerCamZoomMultiplier;
                weaponTargetFOV = normalFOV / currentWeaponData.weaponCamZoomMultiplier;
                //cursor.SetActive(false);
                //if (currentGunData.name != "Sniper")
                //{
                //}
                //else { sniperCam.gameObject.SetActive(true); }
            }
            else
            {
                //cursor.SetActive(true);
                tempAnchor.position = Vector3.Lerp(tempAnchor.position, tempStateHip.position, Time.deltaTime * (currentWeaponData.ADSSpeed * currentWeaponData.posKickReturnSpeed));
                /*if (currentGunData.name != "Sniper")*/
                targetFOV = normalFOV; weaponTargetFOV = normalFOV;
                //else { sniperCam.gameObject.SetActive(false); }
            }
        }
        else { targetFOV = normalFOV; weaponTargetFOV = normalFOV; }
    }
    #endregion

    #region Attack/Shoot
    private void Attack()
    {
        if (shooting && aiming) { shooting = false; aiming = false; }
        currentWeapon.GetComponentInChildren<Animator>().SetBool("Light Attack", shooting);
        currentWeapon.GetComponentInChildren<Animator>().SetBool("Heavy Attack", aiming);
    }

    public void Shoot()
    {
        if (canShoot)
        {
            readyToShoot = false;

            //cooldown
            currentCooldown = currentWeaponData.fireRate;

            //bullet spread
            Vector3 tempSpread = /*cam.transform.position*/firePoint.transform.position + /*cam.transform.forward*/firePoint.transform.forward * currentWeaponData.range;
            tempSpread += Random.Range(-currentWeaponData.bulletSpread, currentWeaponData.bulletSpread) * /*cam.transform.up*/firePoint.transform.up;
            tempSpread += Random.Range(-currentWeaponData.bulletSpread, currentWeaponData.bulletSpread) * /*cam.transform.right*/firePoint.transform.right;
            tempSpread -= /*cam.transform.position*/firePoint.transform.position;
            tempSpread.Normalize();

            //raycast
            if (Physics.Raycast(/*cam.transform.position*/firePoint.transform.position, tempSpread, out hit, currentWeaponData.range, currentWeaponData.canBeShot))
            {
                //TrailRenderer trail = Instantiate(bulletTrail, firePoint.position, firePoint.rotation);
                //StartCoroutine(SpawnTrail(trail, hit));

                if (hit.collider.gameObject.layer == damageLayer1 || hit.collider.gameObject.layer == damageLayer2)
                {
                    hit.collider.GetComponent<EnemyBodyPartHealthManager>().DamageEnemyPart(currentWeaponData.damage);
                    //print(hit.collider.name);
                    //if (hit.collider.CompareTag("EnemyHead")) { HitmarkerEffect(true); }
                    //else { HitmarkerEffect(false); }
                    hasHit = true;
                }
                else if(hit.collider.gameObject.layer == damageLayer3)
                {
                    hit.collider.GetComponent<SimpleEnemyHealth>().TakeDamage(currentWeaponData.damage);
                    hasHit = true;
                }
                if (hit.rigidbody != null) { hit.rigidbody.AddForceAtPosition(cam.transform.forward * (currentWeaponData.bulletForce * 1000), hit.point); }
                ApplyBulletHole();
            }

            if (currentWeaponData.isMelee) { PlayMeleeSound(hasHit); }

            bulletsToShoot--;

            if (bulletsToShoot > 0) { Invoke("Shoot", currentWeaponData.timeBetweenBullets/* / 60*/); }

            if (!IsInvoking("ResetShot") && !readyToShoot) { Invoke("ResetShot", currentWeaponData.timeBetweenFiring); }

            if (!currentWeaponData.isMelee)
            {
                if (bulletsToShoot == 0)
                {
                    //gunshot sound
                    PlayGunshotSound();

                    if (muzzleFlash != null) { muzzleFlash.Play(); }
                    //recoil
                    recoilScript.ApplyRecoil();
                    CameraShaker.Instance.ShakeOnce(currentWeaponData.magnitude, currentWeaponData.roughness, currentWeaponData.fadeInTime, currentWeaponData.fadeOutTime);
                    ManageAmmo(0, 2, 0);
                    UpdateAmmoUI(currentWeaponData.isMelee);
                }
                if (hasHit) { PlayImpactSFX(); }
            }

            hasHit = false;

            if (currentWeaponData.useRecovery) { /*currentWeapon.GetComponent<Animator>().Play("Recovery", 0, 0);*/ }
        }
        else { shooting = false; }
    }

    private void ResetShot()
    {
        readyToShoot = true; /*hasResetRecoilPattern = true;*/
        if (!currentWeaponData.isAutomatic) { shooting = false; }
    }

    public void UpdateAmmoUI(bool isMelee)
    {
        if (currentWeapon != null)
        {
            if (!isMelee) { ammoCounterText.SetText(currentWeaponData.GetMag() / currentWeaponData.bulletsPerTap + " / " + currentWeaponData.GetStash() / currentWeaponData.bulletsPerTap); }
            else { ammoCounterText.SetText(" "); }
        }
    }
    #endregion

    #region Effects
    private void ApplyBulletHole()
    {
        GameObject bulletHole = Instantiate(currentWeaponData.bulletHolePrefab, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
        bulletHole.transform.parent = hit.collider.gameObject.transform;
        Destroy(bulletHole, 4f);
    }

    //public void HitmarkerEffect(bool heashot)
    //{
    //    if (heashot)
    //    {
    //        hitMarkerImage.color = headshotColor;
    //        //sfx.PlayOneShot(hitMarkerSound, 1);
    //        hitMarkerWaitTime = 0.25f;
    //    }
    //    else
    //    {
    //        hitMarkerImage.color = normalColor;
    //        //sfx.PlayOneShot(hitMarkerSound, 1);
    //        hitMarkerWaitTime = 0.25f;
    //    }
    //}

    public void PlayImpactSFX()
    {
        AudioSource hitSFX = Instantiate(currentWeaponData.impactSFX, hit.point, Quaternion.identity).GetComponent<AudioSource>();
        hitSFX.PlayOneShot(hitSFX.clip, 1);
    }

    private void PlayGunshotSound()
    {
        sfx.clip = currentWeaponData.gunshotSounds[Random.Range(0, currentWeaponData.gunshotSounds.Length/* - 1*/)];
        sfx.pitch = 1 - currentWeaponData.pitchRandomization + Random.Range(-currentWeaponData.pitchRandomization, currentWeaponData.pitchRandomization);
        sfx.PlayOneShot(sfx.clip, currentWeaponData.volume);
    }

    private void PlayMeleeSound(bool hasHit)
    {
        if (hasHit) { sfx.clip = currentWeaponData.meleeHitSounds[Random.Range(0, currentWeaponData.meleeHitSounds.Length/* - 1*/)]; }
        else { sfx.clip = currentWeaponData.meleeMissSounds[Random.Range(0, currentWeaponData.meleeMissSounds.Length/* - 1*/)]; }
        sfx.pitch = 1 - currentWeaponData.pitchRandomization + Random.Range(-currentWeaponData.pitchRandomization, currentWeaponData.pitchRandomization);
        sfx.PlayOneShot(sfx.clip, currentWeaponData.volume);
    }
    #endregion

    #region Cheats/Hacks
    private void ReplenishAmmo()
    {
        currentWeaponData.currentBulletsInMagazine = currentWeaponData.magazineSize;
        currentWeaponData.currentAmmoStash = currentWeaponData.maxAmmoStash;
    }
    #endregion
    #endregion
}
