using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;

    [SerializeField] AudioClip audioclip;

    [SerializeField] float dmg;
    [SerializeField] float radius;
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private ParticleSystem muzzleFlash;

    [Header("Bullet Pooling")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] Transform bulletSpawnPoint;
    [SerializeField] private int bulletPoolSize = 10;

    [Header("Radius Visual")]
    [SerializeField] private GameObject radiusPlane;
    [SerializeField] private Material radiusMaterial;

    [Header("Emission Charge Visual")]
    [SerializeField] private Renderer chargeRenderer;
    [SerializeField][ColorUsage(true, true)]  private Color _defaultEmissionColor = Color.black;
    [SerializeField][ColorUsage(true, true)]  private Color _chargedEmissionColor = Color.red;
    [SerializeField] private float _emissionLerpTime = 0.3f;

    [Header("Debug")]
    public bool debugShowRadius = false;

    private Material _emissionMatInstance;
    private Material radiusMatInstance;
    private Coroutine radiusFadeCoroutine;
    private Coroutine chargeCoroutine;
    private Coroutine fireCoroutine;

    private List<Enemy> enemiesInRange = new List<Enemy>();
    private List<GameObject> bulletPool = new List<GameObject>();

    private Sine sineInstance;

    private void Start()
    {
        SphereCollider colider = GetComponent<SphereCollider>();
        colider.radius = radius;
        colider.isTrigger = true;

        for (int i = 0; i < bulletPoolSize; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity, transform);
            bullet.SetActive(false);
            bulletPool.Add(bullet);
        }

        if (radiusPlane != null && radiusMaterial != null)
        {
            radiusMatInstance = Instantiate(radiusMaterial);
            var meshRenderer = radiusPlane.GetComponent<MeshRenderer>();
            if (meshRenderer != null) meshRenderer.material = radiusMatInstance;

            float scale = radius / 4.8f;
            radiusPlane.transform.localScale = new Vector3(scale, 1f, scale);
            SetRadiusAlpha(1f);
        }

        if (chargeRenderer != null)
        {
            _emissionMatInstance = Instantiate(chargeRenderer.material);
            chargeRenderer.material = _emissionMatInstance;
            _emissionMatInstance.EnableKeyword("_EMISSION");
            _emissionMatInstance.SetColor("_EmissionColor", _defaultEmissionColor);
        }

        sineInstance = Sine.Instance;
    }

    private void Update()
    {
        if (sineInstance != null && sineInstance.vratiVelocity() < 0.1f)
        {
            ShowRadiusVisual(true, 0.3f);
        }
        else
        {
            ShowRadiusVisual(false, 0.3f);
        }

        Enemy target = GetClosestEnemy();
        if (target != null)
        {
            Vector3 direction = target.transform.position - transform.position;
            direction.y = 0f;
            transform.rotation = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 270, 0);
        }
    }

    public void ShowRadiusVisual(bool show, float fadeTime = 0.3f)
    {
        if (radiusPlane == null || radiusMatInstance == null)
            return;

        if (radiusFadeCoroutine != null)
            StopCoroutine(radiusFadeCoroutine);
        radiusFadeCoroutine = StartCoroutine(LerpRadiusAlpha(show ? .01f : 0f, fadeTime));
    }

    private IEnumerator LerpRadiusAlpha(float targetAlpha, float duration)
    {
        if (radiusMatInstance == null) yield break;
        Color col = radiusMatInstance.color;
        float startAlpha = col.a;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float lerp = Mathf.Lerp(startAlpha, targetAlpha, t / duration);
            radiusMatInstance.color = new Color(col.r, col.g, col.b, lerp);
            yield return null;
        }
        radiusMatInstance.color = new Color(col.r, col.g, col.b, targetAlpha);
    }

    private void SetRadiusAlpha(float alpha)
    {
        if (radiusMatInstance != null)
        {
            Color col = radiusMatInstance.color;
            radiusMatInstance.color = new Color(col.r, col.g, col.b, alpha);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null && !enemiesInRange.Contains(enemy))
        {
            enemiesInRange.Add(enemy);
            if (fireCoroutine == null)
                fireCoroutine = StartCoroutine(FireRoutine());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null && enemiesInRange.Contains(enemy))
        {
            enemiesInRange.Remove(enemy);
            if (enemiesInRange.Count == 0 && fireCoroutine != null)
            {
                StopCoroutine(fireCoroutine);
                fireCoroutine = null;
            }
        }
    }

    private IEnumerator FireRoutine()
    {
        // Debug.Log("TEEEEEEEEEST");
        while (enemiesInRange.Count > 0)
        {
            enemiesInRange.RemoveAll(e => e == null || !e.gameObject.activeInHierarchy);

            Enemy target = GetClosestEnemy();
            if (target == null)
            {
                if (muzzleFlash != null) muzzleFlash.Stop();
                yield return null;
                continue;
            }

            float dist = Vector3.Distance(transform.position, target.transform.position);
            if (dist > radius)
            {
                if (muzzleFlash != null) muzzleFlash.Stop();
                yield return null;
                continue;
            }

            if (chargeCoroutine != null)
                StopCoroutine(chargeCoroutine);
            chargeCoroutine = StartCoroutine(AnimateChargeMaterial());

            yield return new WaitForSeconds(_emissionLerpTime);

            GameObject bullet = GetPooledBullet();
            if (bullet != null)
            {
                bullet.transform.position = transform.position + transform.forward;
                bullet.transform.rotation = Quaternion.LookRotation(target.transform.position - transform.position);
                bullet.SetActive(true);

                TurretBullet turretBullet = bullet.GetComponent<TurretBullet>();
                if (turretBullet != null)
                {
                    turretBullet.Initialize(target, dmg);
                    if (muzzleFlash != null) muzzleFlash.Play();
                    audioSource.PlayOneShot(audioclip);

                }
            }

            if (_emissionMatInstance != null)
                _emissionMatInstance.SetColor("_EmissionColor", _defaultEmissionColor);

            yield return new WaitForSeconds(1f / fireRate - _emissionLerpTime);

            if (target == null || !target.gameObject.activeInHierarchy || Vector3.Distance(transform.position, target.transform.position) > radius)
            {
                if (muzzleFlash != null) muzzleFlash.Stop();
            }
        }

        if (muzzleFlash != null) muzzleFlash.Stop();
        fireCoroutine = null;
    }

    private IEnumerator AnimateChargeMaterial()
    {
        if (_emissionMatInstance == null) yield break;

        float t = 0f;
        Color startColor = _defaultEmissionColor;
        Color endColor = _chargedEmissionColor;

        while (t < _emissionLerpTime)
        {
            t += Time.deltaTime;
            Color lerped = Color.Lerp(startColor, endColor, t / _emissionLerpTime);
            _emissionMatInstance.SetColor("_EmissionColor", lerped);
            yield return null;
        }

        _emissionMatInstance.SetColor("_EmissionColor", endColor);
    }

    private GameObject GetPooledBullet()
    {
        foreach (var bullet in bulletPool)
        {
            if (!bullet.activeInHierarchy)
                return bullet;
        }
        return null;
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

    public void UpgradeDamage(float percent)
    {
        dmg *= 1f + percent;
    }

    public void UpgradeFireRate(float percent)
    {
        fireRate *= 1f + percent;
        fireRate = Mathf.Max(0.1f, fireRate);
    }

    public void UpgradeRadius(float percent)
    {
        radius *= 1f + percent;
        var col = GetComponent<SphereCollider>();
        if (col != null) col.radius = radius;

        if (radiusPlane != null)
        {
            float scale = radius / 4.8f;
            radiusPlane.transform.localScale = new Vector3(scale, 1f, scale);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (debugShowRadius)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
#endif
}
