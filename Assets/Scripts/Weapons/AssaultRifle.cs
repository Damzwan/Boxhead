using System;
using Enemies;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Weapons
{
    public class AssaultRifle : Weapon
    {
        public GameObject bulletTrail;
        private ParticleSystem ps;

        new  void Start()
        {
            base.Start();
            ps = getFireLocation().Find("FX_Gunshot_01").GetComponent<ParticleSystem>();
        }

        public override void fire()
        {
            if (!canFire()) return;
            ps.Play();
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

        private void drawBulletTrail(Vector3 start, Vector3 end)
        {
            GameObject bulletTrailEffect = Instantiate(bulletTrail, start, Quaternion.identity);
            LineRenderer lineRenderer = bulletTrailEffect.GetComponent<LineRenderer>();
            lineRenderer.SetPosition(0, start);
            lineRenderer.SetPosition(1, end);
            Destroy(bulletTrailEffect, 1f);
        }

        public override void onFireRelease()
        {
            ps.Stop();
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