using System;
using UnityEngine;
using UnityEngine.AI;

namespace Enemies
{
    public abstract class Enemy : MonoBehaviour
    {
        public float Health;
        public float healthbarDisableTime = 3f;

        private Animator anim;
        float currHealth;
        private Rigidbody rb;
        private static readonly int IsDead = Animator.StringToHash("isDead");
        private Collider col;
        private bool alreadyDead;

        private Canvas healthbarCanvas;
        private RectTransform healthbar;
        private float healthbarSize;
        private float healthbarDisableTimer;


        public void Start()
        {
            currHealth = Health;
            rb = GetComponent<Rigidbody>();
            anim = GetComponent<Animator>();
            col = GetComponent<Collider>();

            healthbar = transform.Find("Healthbar/Background/Foreground").GetComponent<RectTransform>();
            healthbarSize = healthbar.sizeDelta.x;
            healthbarCanvas = transform.Find("Healthbar").GetComponent<Canvas>();
            healthbarCanvas.enabled = false;
        }

        public void Update()
        {
            if (healthbarDisableTimer > 0) handleHealthbarTimer();
        }

        public virtual void takeDamage(Vector3 knockbackDirection, float knockbackForce, float dmgAmount)
        {
            if (alreadyDead) return;
            currHealth -= dmgAmount;
            if (currHealth < 0) currHealth = 0;
            healthbar.sizeDelta = new Vector2(healthbarSize * (currHealth / Health), healthbar.sizeDelta.y);
            rb.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);
            healthbarDisableTimer = healthbarDisableTime;
            healthbarCanvas.enabled = true;

            if (currHealth <= 0) die();

        }

        private void die()
        {
            alreadyDead = true;
            col.enabled = false;
            anim.SetTrigger(IsDead);
            
            var agent = GetComponent<NavMeshAgent>();
            if (agent) agent.enabled = false;
            
            Destroy(gameObject, 3);
        }

        public virtual void takeDamage(float dmgAmount)
        {
            takeDamage(Vector3.zero, 0, dmgAmount);
        }

        public float getCurrHealth()
        {
            return currHealth;
        }

        private void handleHealthbarTimer()
        {
            healthbarDisableTimer -= Time.deltaTime;
            if (healthbarDisableTimer <= 0)
            {
                healthbarCanvas.enabled = false;
            }
        }
    }
}