using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Weapons
{
    public abstract class Weapon : MonoBehaviour
    {
        public float damage = 10f;
        public float standInaccuracy = 10f;
        public float moveInaccuracy = 30f;
        public float range = 20f;
        public float impactForce = 30f;
        public float fireRate = 15f;

        public Vector3 carryPosition;
        public Quaternion carryRotation;

        private CharController player;
        private Transform fireLocation;
        private float nextTimeToFire;

        public abstract void fire();
        public abstract void onFireRelease();
        public abstract void altFire();
        public abstract void onAltFireRelease();

        private Camera cam;

        public void Start()
        {
            cam = Camera.main;
            player = FindObjectOfType<CharController>();
            fireLocation = transform.Find("FireLocation").transform;
        }

        public Vector3 determineShootDirection()
        {
            return angleToVector(determineShootAngle());
        }

        public float determineShootAngle()
        {
            var firePos = getFireLocation().position;
            var ray = cam.ScreenPointToRay(Input.mousePosition);
            Plane aimPlane = new Plane(Vector3.up, Vector3.up * firePos.y);
            var loc = aimPlane.Raycast(ray, out float distance) ? ray.GetPoint(distance) : Vector3.zero;

            var dir = loc - firePos;
            var angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
            var inaccuracy = player.isMoving() ? moveInaccuracy : standInaccuracy;
            var inAccurateAngle = Random.Range(angle - inaccuracy / 2, angle + inaccuracy / 2);
            return inAccurateAngle;
        }

        public Vector3 angleToVector(float angle)
        {
            return (Quaternion.Euler(0f, angle, 0f) * Vector3.forward).normalized;
        }

        public CharController getPlayer()
        {
            return player;
        }

        public Transform getFireLocation()
        {
            return fireLocation;
        }

        public bool canFire()
        {
            if (Time.time < nextTimeToFire) return false;
            nextTimeToFire = Time.time + 1f / fireRate;
            if (getPlayer().isPlayerRolling())
            {
                onFireRelease();
                return false;
            }

            return true;
        }

        public bool canAltFire()
        {
            return !getPlayer().isPlayerRolling();
        }
    }
}