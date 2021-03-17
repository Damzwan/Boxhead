using System;
using System.Collections;
using System.Collections.Generic;
using Enemies;
using UnityEngine.AI;
using UnityEngine;

public class Zombie : Enemy
{
    public float knockbackDuration = 0.2f;

    private GameObject target;
    private NavMeshAgent agent;
    private float knockbackTimer;

    new void Start()
    {
        base.Start();
        agent = GetComponent<NavMeshAgent>();
        target = GameObject.FindGameObjectWithTag("Player");
    }
    
    void Update()
    {
        if (agent.enabled) agent.SetDestination(target.transform.position);
        else if (getCurrHealth() > 0)
        {
            knockbackTimer += Time.deltaTime;
            if (knockbackTimer >= knockbackDuration) agent.enabled = true;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject == target)
        {
            Debug.Log("player hit!");
        }
    }
    
    public override void takeDamage(Vector3 knockbackDirection, float knockbackForce, float dmgAmount)
    {
        agent.enabled = false;
        base.takeDamage(knockbackDirection, knockbackForce, dmgAmount);
        knockbackTimer = 0;
    }
}
