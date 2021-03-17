using System;
using UnityEngine;

namespace Enemies
{
    public abstract class Enemy : MonoBehaviour
    {
        public float Health;

        private Animator anim;
        float currHealth;
        private Rigidbody rb;
        private static readonly int IsDead = Animator.StringToHash("isDead");
        private Collider col;
        private bool alreadyDead;

        public void Start()
        {
            currHealth = Health;
            rb = GetComponent<Rigidbody>();
            anim = GetComponent<Animator>();
            col = GetComponent<Collider>();
        }

        public virtual void takeDamage(Vector3 knockbackDirection, float knockbackForce, float dmgAmount)
        {
            if (alreadyDead) return;
            currHealth -= dmgAmount;
            rb.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);

            if (currHealth <= 0)
            {
                alreadyDead = true;
                col.enabled = false;
                anim.SetTrigger(IsDead);
                Destroy(gameObject, 3);
            }
        }

        public float getCurrHealth()
        {
            return currHealth;
        }
    }
}