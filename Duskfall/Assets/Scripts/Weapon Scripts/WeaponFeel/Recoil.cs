using UnityEngine;

public class Recoil : MonoBehaviour
{
    private bool isAiming;

    private Vector3 currentRotation;
    private Vector3 targetRotation;
    [HideInInspector] public Quaternion originRotation;
    [HideInInspector] public Transform returnPosition;

    [SerializeField] private WeaponSystem weaponScript;
    [SerializeField] private Transform weaponHolder;
    [HideInInspector] public Weapon gun;
    [HideInInspector] public Transform weaponAnchor;

    private int currentStep;
    private bool hasResetRecoilPattern;
    private float recoilResetTimer;

    void Update()
    {
        isAiming = weaponScript.aiming;

        UpdateTargetRotation();
        HandleResetTimer();
        if(weaponAnchor == null) { return; }
        //Brings the weapon back to it's proper rotation
        weaponAnchor.localRotation = Quaternion.Lerp(weaponAnchor.localRotation, originRotation, Time.deltaTime * (isAiming ? gun.aimRotKickReturnSpeed : gun.rotKickReturnSpeed));
    }

    void UpdateTargetRotation()
    {
        if (gun == null) { return; }

        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, (isAiming ? gun.aimReturnSpeed : gun.returnSpeed) * Time.deltaTime);
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, (isAiming ? gun.aimSnappiness : gun.snappiness) * Time.deltaTime);
        transform.localRotation = Quaternion.Euler(currentRotation);
        //Keeps the weapon pointing where the camera is pointing
        weaponHolder.localRotation = transform.localRotation;
    }

    void HandleResetTimer()
    {
        recoilResetTimer += Time.deltaTime;
        if (recoilResetTimer >= 0.5f)
        {
            if (!weaponScript.shooting && !hasResetRecoilPattern) { currentStep = 0; hasResetRecoilPattern = true; }
            recoilResetTimer = 0;
        }
    }

    public void ApplyRecoil()
    {
        if (gun == null) { return; }

        if (gun.randomizeRecoil)
        {
            float xRecoil = Random.Range(-gun.randomRecoilConstraints.x, gun.randomRecoilConstraints.x);
            float yRecoil = Random.Range(-gun.randomRecoilConstraints.y, gun.randomRecoilConstraints.y);
            targetRotation += new Vector3(xRecoil, yRecoil, 0);
        }
        else
        {
            if (!hasResetRecoilPattern)
            {
                if (currentStep >= gun.magazineSize) { currentStep = gun.magazineSize; }
                else { currentStep++; }
            }
            else { currentStep = 0; hasResetRecoilPattern = false; }

            currentStep = Mathf.Clamp(currentStep, 0, gun.recoilPattern.Length - 1);

            targetRotation += gun.recoilPattern[currentStep];
        }

        weaponAnchor.Rotate(isAiming ? gun.aimRotKick : gun.rotKick, 0, 0);
        weaponAnchor.position -= weaponScript.currentWeapon.transform.forward * (isAiming ? gun.aimPosKick : gun.posKick) / 10f;
    }
}
