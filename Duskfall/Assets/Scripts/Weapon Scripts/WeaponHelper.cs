using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponHelper : MonoBehaviour
{
    [SerializeField] private GameObject objToDestroy;
    private WeaponSystem weapon;
    private Transform verticalCrosshair, horizontalCrosshair;
    private GameObject UI;
    Animator anim;
    PhoneManager phone;

    private void OnEnable()
    {
        weapon = FindObjectOfType<WeaponSystem>();
        anim = GetComponent<Animator>();
        phone = FindObjectOfType<PhoneManager>();
        //UI = GameObject.Find("UI");
        //verticalCrosshair = UI.transform.Find("Canvas/HUD/Crosshairs/Vertical Crosshair");
        //horizontalCrosshair = UI.transform.Find("Canvas/HUD/Crosshairs/Horizontal Crosshair");
        //NeutralCrosshair();
    }

    void DisableAnimator() { anim.SetBool("Stow", false); anim.enabled = false; }

    public void Stow() { if (!phone.isUsing) { Destroy(objToDestroy); } }

    public void TriggerAttack() { weapon.Shoot(); }

    public void ResetLightAttack() { anim.ResetTrigger("Light Attack"); }

    //public void NeutralCrosshair() { verticalCrosshair.gameObject.SetActive(false); horizontalCrosshair.gameObject.SetActive(false); }
    //public void VerticalCrosshair() { verticalCrosshair.gameObject.SetActive(true);  horizontalCrosshair.gameObject.SetActive(false);  }
    //public void HorizontalCrosshair() { verticalCrosshair.gameObject.SetActive(false); horizontalCrosshair.gameObject.SetActive(true); }
}
