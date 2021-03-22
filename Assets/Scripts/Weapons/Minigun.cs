using System;
using Enemies;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Weapons
{
    public class Minigun : Weapon
    {
        public int minFireRate;
        public float chargeTime = 3f;

        private float maxFireRate;
        private float chargeTimer;
        private bool isCharging;

        new void Start()
        {
            base.Start();
            maxFireRate = fireRate;
            fireRate = minFireRate;
        }

        private void Update()
        {
            if (isCharging) chargeTimer += Time.deltaTime / chargeTime;
        }

        public override void fire()
        {
            if (!canFire()) return;

            if (!isCharging)
            {
                isCharging = true;
                chargeTimer = 0;
            }

            fireRate = (int) Mathf.Lerp(minFireRate, maxFireRate, chargeTimer);

            getPS().Play();
            var dir = determineShootDirection();
            var firePos = getFireLocation().position;

            int layerMask = 1 << 11; // focus on the layer with enemies
            RaycastHit hit;
            // Does the ray intersect any objects excluding the player layer
            if (Physics.Raycast(firePos, dir, out hit, range, layerMask))
            {
                drawBulletTrail(firePos, hit.point);
                hit.collider.GetComponent<Enemy>().takeDamage(dir, impactForce, damage);
            }
            else drawBulletTrail(firePos, firePos + dir * range);
        }

        public override void onFireRelease()
        {
            getPS().Stop();
            chargeTimer = 0;
            isCharging = false;
        }

        public override void altFire()
        {
            throw new NotImplementedException();
        }

        public override void onAltFireRelease()
        {
            throw new NotImplementedException();
        }
    }
}