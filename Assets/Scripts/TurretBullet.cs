using UnityEngine;
using System;
using System.Collections.Generic;

public class TurretBullet : MonoBehaviour
{
    [SerializeField] private float speed = 20f;
    [SerializeField] public bool isTeslaBullet = false; // Set this to true for tesla bullets
    [SerializeField] private float teslaBounceRadius = 5f; // Radius to search for next enemy
    private Enemy target;
    private float damage;
    private Action<GameObject> returnToPoolCallback;
    private bool hasBounced = false;

    public void Initialize(Enemy targetEnemy, float dmg)
    {
        target = targetEnemy;
        damage = dmg;
        hasBounced = false;
        gameObject.SetActive(true);
    }

    private void ResetBullet()
    {
        target = null;
        damage = 0f;
        returnToPoolCallback = null;
        hasBounced = false;
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (target == null || !target.gameObject.activeInHierarchy)
        {
            ReturnToPool();
            return;
        }

        Vector3 dir = (target.transform.position - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;

        // Check if close enough to hit
        if (Vector3.Distance(transform.position, target.transform.position) < 0.5f)
        {
            target.TakeDmg(damage);
            if (isTeslaBullet && !hasBounced)
            {
                TryBounceToNearbyEnemy();
            }
            else
            {
                ReturnToPool();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null && enemy == target)
        {
            enemy.TakeDmg(damage);
            if (isTeslaBullet && !hasBounced)
            {
                TryBounceToNearbyEnemy();
            }
            else
            {
                ReturnToPool();
            }
        }
    }

    private void TryBounceToNearbyEnemy()
    {
        hasBounced = true;
        Collider[] hits = Physics.OverlapSphere(transform.position, 5f); // Static radius of 5 units
        Enemy closest = null;
        float minDist = float.MaxValue;

        foreach (var hit in hits)
        {
            Enemy enemy = hit.GetComponent<Enemy>();
            if (enemy == null || enemy == target || !enemy.gameObject.activeInHierarchy)
                continue;

            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = enemy;
            }
        }

        if (closest != null)
        {
            target = closest;
        }
        else
        {
            ReturnToPool();
        }
    }

    private void ReturnToPool()
    {
        ResetBullet();
        returnToPoolCallback?.Invoke(gameObject);
    }
}