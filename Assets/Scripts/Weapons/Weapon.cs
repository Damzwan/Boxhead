using System;
using Items;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Weapons
{
    public abstract class Weapon : MonoBehaviour
    {
        public GameObject bulletTrail;
        public float damage = 10f;
        public float standInaccuracy = 10f;
        public float moveInaccuracy = 30f;
        public float range = 20f;
        public float impactForce = 30f;
        public float fireRate = 15;
        public Item item;

        public Vector3 carryPosition;
        public Quaternion carryRotation;

        private CharController player;
        private Transform fireLocation;
        private float nextTimeToFire;

        private BoxCollider hitCollider;
        private BoxCollider trigger;
        private Rigidbody rb;
        private ParticleSystem ps;
        
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
            
            var colliders = GetComponents<BoxCollider>();
            trigger = colliders[0].isTrigger ? colliders[0] : colliders[1]; 
            hitCollider = colliders[0].isTrigger ? colliders[1] : colliders[0];

            rb = GetComponent<Rigidbody>();
            ps = GetComponentInChildren<ParticleSystem>();
        }

        public Vector3 determineShootDirection()
        {
            return angleToVector(determineShootAngle());
        }

        public Vector3 determineShootDirection(float moveInaccuracy, float standInaccuracy)
        {
            return angleToVector(determineShootAngle(moveInaccuracy, standInaccuracy));
        }

        public float determineShootAngle()
        {
            return determineShootAngle(moveInaccuracy, standInaccuracy);
        }

        public float determineShootAngle(float moveInaccuracy, float standInaccuracy)
        {
            var angle = getAccurateAngle();
            var inaccuracy = player.isMoving() ? moveInaccuracy : standInaccuracy;
            return getInaccurateAngle(angle, inaccuracy);
        }

        public float getAccurateAngle()
        {
            var firePos = getFireLocation().position;
            var ray = cam.ScreenPointToRay(Input.mousePosition);
            Plane aimPlane = new Plane(Vector3.up, Vector3.up * firePos.y);
            var loc = aimPlane.Raycast(ray, out float distance) ? ray.GetPoint(distance) : Vector3.zero;

            var dir = loc - firePos;
            var angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
            return angle;
        }

        public float getInaccurateAngle(float angle, float inaccuracy)
        {
            return Random.Range(angle - inaccuracy / 2, angle + inaccuracy / 2);
        }

        public float getInaccurateAngle(float angle)
        {
            var inaccuracy = player.isMoving() ? moveInaccuracy : standInaccuracy;
            return getInaccurateAngle(angle, inaccuracy);
        }

        public void drawBulletTrail(Vector3 start, Vector3 end)
        {
            GameObject bulletTrailEffect = Instantiate(bulletTrail, start, Quaternion.identity);
            LineRenderer lineRenderer = bulletTrailEffect.GetComponent<LineRenderer>();
            lineRenderer.SetPosition(0, start);
            lineRenderer.SetPosition(1, end);
            Destroy(bulletTrailEffect, 1f);
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

        public virtual void onEquip()
        {
            trigger.enabled = false;
            hitCollider.enabled = false;
            rb.isKinematic = true;
        }

        public virtual void onUnEquip()
        {
            rb.isKinematic = false;
            rb.AddForce(determineShootDirection() * 10, ForceMode.Impulse);
            hitCollider.enabled = true;
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

        public ParticleSystem getPS()
        {
            return ps;
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Ground")) trigger.enabled = true;
        }
    }
}