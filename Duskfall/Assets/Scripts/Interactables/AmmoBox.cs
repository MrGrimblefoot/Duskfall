using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoBox : Interactable
{
    public bool weaponType;
    public int minAmmoToGive, maxAmmoToGive;

    public void AddAmmo()
    {
        WeaponSystem weapon = FindObjectOfType<WeaponSystem>();
        int bulletsToAddMultiplier = weaponType == false ? 1 : 8;
        weapon.ManageAmmo(Random.Range(minAmmoToGive * bulletsToAddMultiplier, maxAmmoToGive * bulletsToAddMultiplier), 1, weaponType == true ? 2 : 1);
        Destroy(gameObject);
    }
}
