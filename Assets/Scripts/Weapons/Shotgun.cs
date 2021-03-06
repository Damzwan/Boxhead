using System;
using System.Collections;
using System.Collections.Generic;
using Enemies;
using UnityEngine;
using Weapons;

public class Shotgun : Weapon
{
    public int bulletAmount = 5;
    public float spreadAngle = 15f;
    public float grapplingForwardSpeed = 10f;
    public float grapplingBackwardSpeed = 10f;
    public float grapplingRange = 15f;

    private LineRenderer lr;
    private Vector3 altFireDir;
    private Vector3 altFirePos;
    private bool altFireEnabled;
    private GameObject altFireHit;
    private bool shouldReleaseFirst;
    
    new void Start()
    {
        base.Start();
        lr = GetComponent<LineRenderer>();
        lr.enabled = false;
    }

    public override void fire()
    {
        if (!canFire()) return;
        getPS().Play();
        var angle = determineShootAngle();
        var startAngle = angle - (bulletAmount / 2) * spreadAngle;
        var firePos = getFireLocation().position;

        for (int i = 0; i < bulletAmount; i++)
        {
            shootBullet(firePos, angleToVector(startAngle + i * spreadAngle));
        }
    }

    public override void onFireRelease()
    {
    }

    public override void altFire()
    {
        if (shouldReleaseFirst) return;
        if (!canAltFire())
        {
            onAltFireReleaseWhenHold();
            return;
        }

        var firePos = getFireLocation().position;
        if (!altFireEnabled)
        {
            lr.enabled = true;
            altFireEnabled = true;
            altFirePos = firePos;
        }

        altFireDir = !(altFireHit is null) ? (firePos - altFirePos).normalized : determineShootDirection();
        var speed = altFireHit is null ? grapplingForwardSpeed : grapplingBackwardSpeed;
        altFirePos += altFireDir * (speed * Time.deltaTime);

        if ((altFirePos - firePos).magnitude > grapplingRange)
        {
            onAltFireReleaseWhenHold();
            return;
        }

        lr.SetPosition(0, firePos);
        lr.SetPosition(1, altFirePos);
        if (!(altFireHit is null))
        {
            if ((altFirePos - firePos).magnitude < 1)
            {
                onAltFireReleaseWhenHold();
                return;
            }

            var pos = altFirePos;
            var enemyPos = altFireHit.transform.position;
            altFireHit.transform.position = new Vector3(pos.x, enemyPos.y, pos.z);
        }
        else
        {
            int layerMask = 1 << 11; // focus on the layer with enemies
            RaycastHit hit;
            if (Physics.Raycast(firePos, altFireDir, out hit, (altFirePos - firePos).magnitude, layerMask))
            {
                altFireHit = hit.collider.gameObject;
            }
        }
    }

    public override void onAltFireRelease()
    {
        lr.enabled = false;
        altFireEnabled = false;
        altFireHit = null;
        shouldReleaseFirst = false;
    }

    private void onAltFireReleaseWhenHold()
    {
        onAltFireRelease();
        shouldReleaseFirst = true;
    }

    private void shootBullet(Vector3 firePos, Vector3 dir)
    {
        if (altFireHit) onAltFireReleaseWhenHold();

        int layerMask = 1 << 11; // focus on the layer with enemies
        RaycastHit hit;
        if (Physics.Raycast(firePos, dir, out hit, range, layerMask))
        {
            drawBulletTrail(firePos, hit.point);
            hit.collider.GetComponent<Enemy>().takeDamage(dir, impactForce, damage);
        }
        else drawBulletTrail(firePos, firePos + dir * range);
    }
}