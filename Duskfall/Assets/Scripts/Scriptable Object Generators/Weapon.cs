using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon")]
public class Weapon : ScriptableObject
{
    #region Variables
    #region Basics
    [Header("Basics")]
    [Tooltip("This is the name of the gun. It's useful in the scripts")]
    public string weaponName;
    [Tooltip("This is what model is Instantiated.")]
    public GameObject prefab;
    [Tooltip("This is not recoil. This is for things like shotguns. For shotguns: 12 full choke, 15 modified choke, 18 for improved cylinder choke")]
    public float bulletSpread;
    [Tooltip("This controls how fast the gun fires.")]
    public float fireRate;
    [Tooltip("This controls how far the weapon can hit.")]
    public float range;
    [Tooltip("Make this true if the weapon is melee. If not, ignore this variable.")]
    public bool isMelee;
    public bool isPistol;
    #endregion

    #region Ammo
    [Header("Ammo")]
    [Tooltip("This controls how many bullets you can fire before you need to reload.")]
    public int magazineSize; //This doesn't change during play
    [Tooltip("This controls how many bullets you can hold in all.")]
    public int maxAmmoStash; //This doesn't change during play
    [Tooltip("This controls how many bullets you currently hold that are not in the magazine.")]
    public int currentAmmoStash;
    [Tooltip("This controls how many bullets you currently have in the magazine.")]
    public int currentBulletsInMagazine;
    #endregion

    #region Reloading
    [Header("Reloading")]
    [Tooltip("This controls how long the Weapon script waits before calculating the ammo stuff.")]
    public float reloadTime;
    #endregion

    #region Fire Type
    [Header("Fire Type")]
    [Tooltip("Turn this on if the weapon is automatic.")]
    public bool isAutomatic;
    [Tooltip("Turn this on if the weapon is burst. This can mean shotgun or burst depending on the timeBetweenBullets variable.")]
    public bool isBurst;
    [Tooltip("This controls how long the Weapon script waits before shooting the next bullet. Keep this at 0 for shotguns.")]
    public float timeBetweenBullets;
    [Tooltip("Keep this at one if the weapon is not a burst weapon. This controls how many bullets get shot.")]
    public int bulletsPerTap;
    //[Tooltip("This controls how long the Weapon script waits before shooting the next bullet. Keep this at 0 for shotguns.")]
    public float timeBetweenFiring;
    [Tooltip("Use this for pump shotguns, bolt-action rifles, etc.")]
    public bool useRecovery;
    #endregion

    #region Aiming & Camera
    [Header("Aiming & Camera")]
    [Tooltip("This controls how fast the camera's FOV goes to it's ADS and Hipfire values.")]
    public float cameraZoomSpeed;
    [Tooltip("This controls how fast the gun goes to it's ADS and Hipfire positions.")]
    public float ADSSpeed;
    [Tooltip("This controls how much the player's camera zooms when you ADS.")]
    public float playerCamZoomMultiplier;
    [Tooltip("This controls how much the weapon camera zooms when you ADS.")]
    public float weaponCamZoomMultiplier;
    [Tooltip("This controls how much the camera is able to rotate while at hip. The higher the number, the lower the look speed.")]
    public float sensitivityMultiplier;
    [Tooltip("This controls how much the camera is able to rotate while ADSing. The higher the number, the lower the look speed.")]
    public float aimSensitivityMultiplier;
    [Tooltip("If there is a sight on the gun, change this. Otherwise leave it at 0.")]
    public float sightOffset;
    public GameObject sightGO;
    #endregion

    #region Bullet
    [Header("Bullet")]
    [Tooltip("This controls how much damage each bullet does.")]
    public int damage;
    [Tooltip("This controls how much a bullet pushes an object.")]
    public float bulletForce;
    [Tooltip("This controls what layers can be shot.")]
    public LayerMask canBeShot;
    [Tooltip("This controls how long the bullet trail is.")]
    public float trailTime;
    #endregion

    #region RecoilPattern
    [Header("Recoil Pattern")]
    public bool randomizeRecoil;
    public Vector2 randomRecoilConstraints;
    [Tooltip("Only use this if randomizeRecoil is false.                     X = Vertical, Y = Horizontal, Z = Forward/Backward")]
    public Vector3[] recoilPattern;
    #endregion

    #region RecoilControl
    [Header("Recoil Control")]
    [Tooltip("This controls how fast the camera goes from the normal position to the new position that is set when the RecoilFire function is called in the Recoil script when hipfiring.")]
    public float snappiness;
    [Tooltip("This controls how fast the camera returns to the normal position when hipfiring.")]
    public float returnSpeed;
    [Tooltip("This is the same as the snappiness variable, except for firing when ADS-ing. This effects where the bullet goes.")]
    public float aimSnappiness;
    [Tooltip("This is the same as the returnSpeed variable, except for firing when ADS-ing. This effects where the bullet goes.")]
    public float aimReturnSpeed;
    #endregion

    #region Kickback
    [Header("Kickback")]
    [Tooltip("This controls how much the weapon rotates to kicks up visually when at hip.")]
    public float rotKick;
    [Tooltip("This modifies how much the current weapon sways while while at hip.")]
    public float rotKickReturnSpeed;
    [Tooltip("This controls how much the weapon rotates to kicks up visually when aiming.")]
    public float aimRotKick;
    [Tooltip("This modifies how much the current weapon sways is while aiming")]
    public float aimRotKickReturnSpeed;
    [Tooltip("This controls how much the weapon moves to kick back visually when at hip.")]
    public float posKick;
    [Tooltip("This controls how fast the weapon returns from the moved position to the normal position when at hip.")]
    public float posKickReturnSpeed;
    [Tooltip("This controls how much the weapon moves to kick back visually when aiming.")]
    public float aimPosKick;
    [Tooltip("This controls how fast the weapon returns from the moved position to the normal position when aiming.")]
    public float aimPosKickReturnSpeed;
    #endregion

    #region Handling
    [Header("Handling Smoothing")]
    public float rotationalSmooth;
        #region Sway
        [Header("Sway")]
        [Tooltip("This controls how intense the positional sway is while at hip.")]
        public float positionalSwayIntensity;
        [Tooltip("This controls how intense the positional sway is while aiming.")]
        public float positionalAimSwayIntensity;
        [Tooltip("This controls how intense the rotational sway is while at hip.")]
        public float rotationalSwayIntensity;
        [Tooltip("This controls how intense the rotational sway is while aiming.")]
        public float rotationalAimSwayIntensity;
        [Tooltip("This controls how far the gun can positionally sway.")]
        public float maxSwayMovement;
        [Tooltip("This controls how far the gun can rotationally sway.")]
        public float maxSwayRotation;
        #endregion

        #region Bob
        [Header("Bob")]
        [Tooltip("This controls how intense the positional sway is while at hip.")]
        public float idleBobSpeed;
        [Tooltip("This controls how intense the positional sway is while at hip.")]
        public float movementBobSpeed;
        [Tooltip("This controls how intense the positional sway is while at hip.")]
        public float ADSPositionalBobDampening;
        [Tooltip("This controls how intense the positional sway is while at hip.")]
        public float ADSRotationalBobDampening;
        [Tooltip("This controls how intense the positional sway is while at hip.")]
        public Vector3 travelLimit = Vector3.one * 0.025f;
        [Tooltip("This controls how intense the positional sway is while at hip.")]
        public Vector3 bobLimit = Vector3.one * 0.01f;
        [Tooltip("This controls how intense the positional sway is while at hip.")]
        public Vector3 bobRotationalMultiplier;
        [Tooltip("This controls the speed at which the gun's rotation centers when the player stops moving.")]
        public float rotationalBobResetSpeed;
        #endregion
    #endregion

    #region CameraShake
    [Header("Camera Shake")]
    public float magnitude;
    public float roughness;
    public float fadeInTime;
    public float fadeOutTime;
    #endregion

    #region Decals
    [Header("Decals")]
    [Tooltip("This is the bullet hole prefab that is Instantiated.")]
    public GameObject bulletHolePrefab;
    public bool useMuzzleFlash;
    //[Tooltip("This is the muzzle flash effect that is Instantiated. Not implimented yet.")]
    //public ParticleSystem muzzleFlashPrefab;
    #endregion

    #region Audio
    [Header("Audio")]
    [Tooltip("This is the array of sounds that can be played when you shoot.")]
    public AudioClip[] gunshotSounds;
    [Tooltip("Only for Melee. This is the array of sounds that can be played when you hit with a melee.")]
    public AudioClip[] meleeHitSounds;
    [Tooltip("Only for Melee. This is the array of sounds that can be played when you miss with a melee.")]
    public AudioClip[] meleeMissSounds;
    [Tooltip("This is how much the gun shot sound can vary in pitch.")]
    public float pitchRandomization;
    [Tooltip("This is how loud the gun shot sound is.")]
    public float volume;
    public GameObject impactSFX;
    #endregion
    #endregion

    #region Functions
    public void Initialize()
    {
        currentAmmoStash = maxAmmoStash;
        currentBulletsInMagazine = magazineSize;
        WeaponSystem weapon = FindObjectOfType<WeaponSystem>().GetComponent<WeaponSystem>();
        if (isPistol)
        {
            weapon.pistolAmmoInMagazine = currentBulletsInMagazine;
            weapon.pistolAmmoInStash = currentAmmoStash;
        }
        else if(!isPistol & !isMelee)
        {
            weapon.shotgunAmmoInMagazine = currentBulletsInMagazine;
            weapon.shotgunAmmoInStash = currentAmmoStash;
        }
    }

    public bool UpdateMagazine()
    {
        if (currentBulletsInMagazine > 0)
        {
            currentBulletsInMagazine -= bulletsPerTap;
            return true;
        }
        else return false;
    }

    public void Reload()
    {
        currentAmmoStash += currentBulletsInMagazine;
        currentBulletsInMagazine = Mathf.Min(magazineSize, currentAmmoStash);
        currentAmmoStash -= currentBulletsInMagazine;
    }

    public void AssignAmmo(int magAmmoToAdd, int stashAmmoToAdd)
    {
        //Debug.Log("trying to assign ammo");
        if (!isMelee && currentAmmoStash != stashAmmoToAdd /*&& currentBulletsInMagazine != magAmmoToAdd*/)
        {
            //Debug.Log("assigning ammo");
            currentAmmoStash = stashAmmoToAdd;
            currentBulletsInMagazine = magAmmoToAdd;
            FindObjectOfType<WeaponSystem>().UpdateAmmoUI(false);
        }
    }

    public int GetStash() { return currentAmmoStash; }

    public int GetMag() { return currentBulletsInMagazine; }
    #endregion
}
