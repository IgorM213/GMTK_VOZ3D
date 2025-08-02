using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{

    [SerializeField] float dmg;
    [SerializeField] float radius;
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private ParticleSystem muzzleFlash;

    private float fireCooldown = 0f;
    private List<Enemy> enemiesInRange = new List<Enemy>();

    private void Start()
    {
        SphereCollider colider = GetComponent<SphereCollider>();
        colider.radius = radius;
        colider.isTrigger = true;
    }

    private void Update()
    {
        fireCooldown -= Time.deltaTime;
        Enemy target = GetClosestEnemy();
        if (target != null)
        {
            Vector3 direction = target.transform.position - transform.position;
            direction.y = 0f; // horizontalno rotiranje samo
            transform.rotation = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 270, 0);
        }
        if (fireCooldown <= 0f && enemiesInRange.Count > 0)
        {
            Attack();
            fireCooldown = 1f / fireRate;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null && !enemiesInRange.Contains(enemy))
        {
            enemiesInRange.Add(enemy);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null && enemiesInRange.Contains(enemy))
        {
            enemiesInRange.Remove(enemy);
        }
    }

    private void Attack()
    {
        Enemy target = GetClosestEnemy();
        if (target != null)
        {
            if (muzzleFlash != null)
                muzzleFlash.Play();
            target.TakeDmg(dmg);
        }
    }
    private Enemy GetClosestEnemy()
    {
        Enemy closest = null;
        float minDistance = Mathf.Infinity;

        foreach (Enemy enemy in enemiesInRange)
        {
            if (enemy == null) continue;

            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = enemy;
            }
        }

        return closest;
    }

}
