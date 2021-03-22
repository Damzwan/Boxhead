using System;
using Enemies;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Weapons
{
    public class Sniper : Weapon
    {
        private LineRenderer lr;
        private Vector3 firePos;

        private bool shouldAim;

        new void Start()
        {
            base.Start();
            lr = GetComponent<LineRenderer>();
            lr.enabled = false;
        }

        private void Update()
        {
            firePos = getFireLocation().position;
            if (!shouldAim) return;
            var aimDir = determineShootDirection(0, 0);

            lr.SetPosition(0, firePos);
            int layerMask = 1 << 11; // focus on the layer with enemies
            RaycastHit hit;
            if (Physics.Raycast(firePos, aimDir, out hit, range, layerMask)) lr.SetPosition(1, hit.point);
            else lr.SetPosition(1, firePos + aimDir * range);
        }


        public override void fire()
        {
            if (!canFire()) return;
            getPS().Play();

            var dir = determineShootDirection();
            int layerMask = 1 << 11; // focus on the layer with enemies
            RaycastHit hit;
            if (Physics.Raycast(firePos, dir, out hit, range, layerMask))
            {
                drawBulletTrail(firePos, hit.point);
                hit.collider.GetComponent<Enemy>().takeDamage(dir, impactForce, damage);
            }
            else drawBulletTrail(firePos, firePos + dir * range);
        }

        public override void onFireRelease()
        {
        }

        public override void altFire()
        {
        }

        public override void onAltFireRelease()
        {
            shouldAim = !shouldAim;
            lr.enabled = shouldAim;
        }
    }
}