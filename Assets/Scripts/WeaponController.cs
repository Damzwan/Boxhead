using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Weapons;

public class WeaponController : MonoBehaviour
{
    private Weapon primaryWeapon;
    private GameObject rightHand;
    
    // Start is called before the first frame update
    void Start()
    {
        rightHand = transform.Find("Root/Hips/Spine_01/Spine_02/Spine_03/Clavicle_R/Shoulder_R/Elbow_R/Hand_R").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Fire1")) primaryWeapon.fire();
        if (Input.GetButtonUp("Fire1")) primaryWeapon.onFireRelease();
        if (Input.GetButton("Fire2")) primaryWeapon.altFire();
        if (Input.GetButtonUp("Fire2")) primaryWeapon.onAltFireRelease();

    }

    private void OnTriggerEnter(Collider other)
    {
        var weapon = other.gameObject;
        if (primaryWeapon && weapon == primaryWeapon.gameObject) return;
        var weaponScript = weapon.GetComponent<Weapon>();
        if (weaponScript is null) return;
        primaryWeapon = weaponScript;
        weapon.transform.parent = rightHand.transform;
        weapon.transform.localPosition = weaponScript.carryPosition;
        weapon.transform.localRotation = weaponScript.carryRotation;
    }
}
