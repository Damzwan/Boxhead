using System;
using System.Collections;
using System.Collections.Generic;
using Enemies;
using UnityEngine;
using Weapons;

public class Shotgun : Weapon
{
    public GameObject bulletTrail;
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

    private ParticleSystem ps;

    new void Start()
    {
        base.Start();
        ps = getFireLocation().Find("FX_Gunshot_01").GetComponent<ParticleSystem>(); //TODO generalize
        lr = GetComponent<LineRenderer>();
        lr.enabled = false;
    }

    public override void fire()
    {
        if (!canFire()) return;
        ps.Play();
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
            altFireDir = determineShootDirection();
            altFirePos = firePos;
        }

        if (!(altFireHit is null)) altFireDir = (transform.position - altFirePos).normalized;
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

    private void drawBulletTrail(Vector3 start, Vector3 end)
    {
        GameObject bulletTrailEffect = Instantiate(bulletTrail, start, Quaternion.identity);
        LineRenderer lineRenderer = bulletTrailEffect.GetComponent<LineRenderer>();
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
        Destroy(bulletTrailEffect, 1f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, grapplingRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, range); 
    }
}