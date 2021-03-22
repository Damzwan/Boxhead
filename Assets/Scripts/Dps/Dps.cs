using System;
using System.Collections;
using System.Collections.Generic;
using Enemies;
using UnityEngine;

public abstract class Dps : MonoBehaviour
{
    
    public int dpsTickAmount = 3;
    public float maxDps = 15f;
    public float timeToMaxDps = 2f;

    public String particleName;
    public float particleMinSize = 0.05f;
    public float particleMaxSize = 0.2f;
    
    private Enemy enemy;
    private float dpsTimer = 0;
    private float dpsTicks = 0;
    private float dps;
    
    private ParticleSystem ps;
    private float timeExposed;
    
    void Start()
    {
        enemy = GetComponent<Enemy>();
        ps = transform.Find(particleName).GetComponent<ParticleSystem>();
    }
    
    void Update()
    {
        if (dpsTicks > 0) handleDamagePerSecond();
    }

    public void ApplyDamagePerSecond()
    {
        timeExposed += Time.deltaTime;
        var exposedRatio = timeExposed / timeToMaxDps;
        var psMain = ps.main;
        psMain.startSize = new ParticleSystem.MinMaxCurve(particleMinSize, particleMaxSize * exposedRatio);
        if (!ps.isEmitting) ps.Play();
        dpsTicks = dpsTickAmount;
        dps = exposedRatio * maxDps;
    }

    private void handleDamagePerSecond()
    {
        dpsTimer += Time.deltaTime;
        if (!(dpsTimer > 1)) return;
        dpsTimer = 0;
        dpsTicks -= 1;
        enemy.takeDamage(Vector3.zero, 0, dps);
        Debug.Log("damaging");
        if (dpsTicks == 0) stopDps();
    }

    private void stopDps()
    {
        timeExposed = 0;
        ps.Stop();
    }
    
}