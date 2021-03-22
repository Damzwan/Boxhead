using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Weapons;

public class WeaponController : MonoBehaviour
{
    public int maxWeaponSize = 3;

    private HashSet<String> weaponsEquiped = new HashSet<string>();
    private Weapon[] weaponArray;
    private int currWeaponIndex;
    private GameObject rightHand;

    // Start is called before the first frame update
    void Start()
    {
        weaponArray = new Weapon[maxWeaponSize];
        rightHand = transform.Find("Root/Hips/Spine_01/Spine_02/Spine_03/Clavicle_R/Shoulder_R/Elbow_R/Hand_R")
            .gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Fire1")) weaponArray[currWeaponIndex]?.fire();
        if (Input.GetButtonUp("Fire1")) weaponArray[currWeaponIndex]?.onFireRelease();
        if (Input.GetButton("Fire2")) weaponArray[currWeaponIndex]?.altFire();
        if (Input.GetButtonUp("Fire2")) weaponArray[currWeaponIndex]?.onAltFireRelease();
        if (Input.GetKeyUp("f")) dropWeapon(); //TODO change with buttons i guess

        if (Input.GetAxis("Mouse ScrollWheel") > 0f) switchWeapon(currWeaponIndex + 1);
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f) switchWeapon(currWeaponIndex - 1);
    }

    private int findFreeWeaponSpot()
    {
        for (int i = 0; i < weaponArray.Length; i++)
        {
            if (weaponArray[i] is null) return i;
        }

        return -1;
    }

    private void dropWeapon()
    {
        var weapon = weaponArray[currWeaponIndex];
        if (weapon is null) return;

        var t = weapon.transform;
        t.parent = null;
        weaponArray[currWeaponIndex] = null;
        weaponsEquiped.Remove(weapon.item.title);
        weapon.onUnEquip();
    }

    private void switchWeapon(int number)
    {
        var prevWeapon = weaponArray[currWeaponIndex];
        if (!(prevWeapon is null)) prevWeapon.gameObject.SetActive(false);
        

        if (number < 0) currWeaponIndex = maxWeaponSize - 1;
        else if (number >= maxWeaponSize) currWeaponIndex = 0;
        else currWeaponIndex = number;

        var currWeapon = weaponArray[currWeaponIndex];
        if (!(currWeapon is null))
        {
            currWeapon.gameObject.SetActive(true);
            currWeapon.onEquip();
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        var weapon = other.GetComponent<Weapon>();
        if (weapon is null || weaponsEquiped.Contains(weapon.item.title)) return;

        int freeSpot = findFreeWeaponSpot();
        if (freeSpot == -1) return;

        weaponArray[freeSpot] = weapon;
        weaponsEquiped.Add(weapon.item.title);
        switchWeapon(freeSpot);

        var weaponTransform = weapon.transform;
        weaponTransform.parent = rightHand.transform;
        weaponTransform.localPosition = weapon.carryPosition;
        weaponTransform.localRotation = weapon.carryRotation;
    }
}