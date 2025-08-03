using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Header("Enemy Spawning")]
    [SerializeField] private List<GameObject> enemyPrefabs;
    [SerializeField] private Transform cityCenter;
    [SerializeField] private float spawnRadius = 10f;
    [SerializeField] private int startEnemiesPerWave = 3;
    [SerializeField] private float timeBetweenWaves = 3f;
    [SerializeField] private float waveSpawnTime = 2f;

    [Header("Pooling")]
    [SerializeField] private int poolSizePerType = 10;

    [Header("Debug")]
    [SerializeField] private float secondsUntilNextWave = 0f;

    private List<List<GameObject>> enemyPools = new List<List<GameObject>>();
    private int currentWave = 0;
    private float waveTimer = 0f;

    void Awake()
    {
        if (enemyPrefabs == null || enemyPrefabs.Count == 0)
        {
            Debug.LogError("EnemyManager: No enemy prefabs assigned!");
            enabled = false;
            return;
        }
        if (cityCenter == null)
        {
            Debug.LogError("EnemyManager: No cityCenter assigned!");
            enabled = false;
            return;
        }

        foreach (var prefab in enemyPrefabs)
        {
            var pool = new List<GameObject>();
            for (int i = 0; i < poolSizePerType; i++)
            {
                GameObject enemyObj = Instantiate(prefab, Vector3.zero, Quaternion.identity, transform);
                enemyObj.SetActive(false);
                pool.Add(enemyObj);
            }
            enemyPools.Add(pool);
        }
    }

    void Start()
    {
        waveTimer = timeBetweenWaves; // So first wave spawns immediately
    }

    void Update()
    {
        waveTimer += Time.deltaTime;
        secondsUntilNextWave = Mathf.Max(0f, timeBetweenWaves - waveTimer);

        if (waveTimer >= timeBetweenWaves)
        {
            StartNextWave();
            waveTimer = 0f;
        }
    }

    void StartNextWave()
    {
        currentWave++;
        int enemiesThisWave = startEnemiesPerWave + currentWave;
        Debug.Log($"Spawning wave {currentWave} with {enemiesThisWave} enemies.");
        StartCoroutine(SpawnWave(enemiesThisWave));
    }

    IEnumerator SpawnWave(int enemiesThisWave)
    {
        float interval = (enemiesThisWave > 1) ? waveSpawnTime / (enemiesThisWave - 1) : 0f;

        for (int i = 0; i < enemiesThisWave; i++)
        {
            int prefabIndex = Random.Range(0, enemyPrefabs.Count);
            Vector3 spawnPos = GetRandomPointOnCircle(cityCenter.position, spawnRadius);

            GameObject enemyObj = GetPooledEnemy(prefabIndex);
            if (enemyObj != null)
            {
                enemyObj.transform.position = spawnPos;
                enemyObj.transform.rotation = Quaternion.LookRotation(cityCenter.position - spawnPos, Vector3.up);

                Rigidbody rb = enemyObj.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }

                var enemy = enemyObj.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.Initialize(cityCenter);
                }

                enemyObj.SetActive(true);
                Debug.Log($"Spawned enemy {enemyObj.name} at {spawnPos}");
            }
            else
            {
                Debug.LogWarning("EnemyManager: No pooled enemy available!");
            }

            if (i < enemiesThisWave - 1)
                yield return new WaitForSeconds(interval);
        }
    }

    GameObject GetPooledEnemy(int prefabIndex)
    {
        foreach (var enemyObj in enemyPools[prefabIndex])
        {
            if (!enemyObj.activeInHierarchy)
                return enemyObj;
        }
        // Optionally expand pool if needed
        GameObject newEnemyObj = Instantiate(enemyPrefabs[prefabIndex], Vector3.zero, Quaternion.identity, transform);
        newEnemyObj.SetActive(false);
        enemyPools[prefabIndex].Add(newEnemyObj);
        return newEnemyObj;
    }

    Vector3 GetRandomPointOnCircle(Vector3 center, float radius)
    {
        float angle = Random.Range(0f, Mathf.PI * 2f);
        float x = Mathf.Cos(angle) * radius;
        float z = Mathf.Sin(angle) * radius;
        return new Vector3(center.x + x, center.y, center.z + z);
    }

    void OnDrawGizmosSelected()
    {
        if (cityCenter != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(cityCenter.position, spawnRadius);
        }
    }
}
