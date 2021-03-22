using System;
using System.Collections;
using Enemies;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Weapons
{
    public class BurstSmg : Weapon
    {
        public int burstAmount;
        public float burstInterval;

        private float angle;
        private Vector3 firePos;
        private int bulletsShot;

        new void Start()
        {
            base.Start();
        }
        

        public override void fire()
        {
            if (!canFire()) return;
            getPS().Play();
            angle = getAccurateAngle();
            firePos = getFireLocation().position;
            bulletsShot = burstAmount;
            StartCoroutine(waitAndShoot());
        }
        
        private void shootBullet()
        {
            getPS().Play();
            int layerMask = 1 << 11; // focus on the layer with enemies
            RaycastHit hit;
            var dir = angleToVector(getInaccurateAngle(angle));
        
            if (Physics.Raycast(firePos, dir, out hit, range, layerMask))
            {
                drawBulletTrail(firePos, hit.point);
                hit.collider.GetComponent<Enemy>().takeDamage(dir, impactForce, damage);
            }
            else drawBulletTrail(firePos, firePos + dir * range);
        
            bulletsShot -= 1;
        }

        IEnumerator waitAndShoot()
        {
            shootBullet();
            yield return new WaitForSeconds(burstInterval);
            if (bulletsShot > 0) yield return StartCoroutine(waitAndShoot());
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