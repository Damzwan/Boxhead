using System;
using Enemies;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Weapons
{
    public class AssaultRifle : Weapon
    {
        new  void Start()
        {
            base.Start();
        }

        public override void fire()
        {
            if (!canFire()) return;
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