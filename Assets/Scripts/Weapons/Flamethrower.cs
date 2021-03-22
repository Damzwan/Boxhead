using System;
using System.Collections.Generic;
using Enemies;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Weapons
{
    public class Flamethrower : Weapon
    {
        public float fireAngle;

        new void Start()
        {
            base.Start();
        }

        public override void fire()
        {
            if (!canFire()) return;
            if (!getPS().isEmitting) getPS().Play();

            var firePos = getFireLocation().position;

            var hits = Physics.OverlapSphere(firePos, range);

            foreach (var hit in hits)
            {
                if (hit.gameObject == gameObject ||
                    hit.transform.gameObject.layer != LayerMask.NameToLayer("Enemies")) continue;

                var hitDir = (hit.gameObject.transform.position - firePos).normalized;
                var angleDiff = Vector3.Angle(hitDir, determineShootDirection());

                if (angleDiff > fireAngle) continue;

                var enemy = hit.GetComponent<Enemy>();
                enemy.takeDamage(damage * Time.deltaTime);

                var dpsHandler = enemy.GetComponent<Fire>();
                if (!(dpsHandler is null)) dpsHandler.ApplyDamagePerSecond();
            }
        }

        public override void onFireRelease()
        {
            getPS().Stop();
        }

        public override void altFire()
        {
        }

        public override void onAltFireRelease()
        {
        }
    }
}