using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    public GameObject zombie;
    public float spawnRange;
    public float spawnRate;
        
    private float nextTimeToSpawn;

    private void Update()
    {
        if (Time.time < nextTimeToSpawn) return;
        nextTimeToSpawn = Time.time + 1f / spawnRate;
        Instantiate(zombie,transform.position + (Random.insideUnitSphere * spawnRange), Quaternion.identity);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, spawnRange);
    }
}
