using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExperienceManager : MonoBehaviour
{
    [SerializeField] private GameObject experiencePrefab;
    [SerializeField] private int poolSize = 20;
    [SerializeField, Range(0f, 1f)] private float dropChance = 0.5f;

    private List<GameObject> experiencePool = new List<GameObject>();
    public static ExperienceManager Instance { get; private set; }

    [Header("Collected Experience")]
    [SerializeField] private int collectedExperience = 0;
    public int CollectedExperience => collectedExperience;
    [SerializeField] private int currentLevel = 1;
    public int CurrentLevel => currentLevel;
    [SerializeField] private int currentLevelExp = 0;
    public int CurrentLevelExp => currentLevelExp;
    [SerializeField] private float expBase = 5f;
    [SerializeField] private float expGrowth = 1.2f;
    private int expToNextLevel = 5;

    [SerializeField] Transform attractor;

    [Header("Attractor Light")]
    [SerializeField] private Light attractorLight;
    [SerializeField] private Color defaultColor = Color.red;
    [SerializeField] private Color calledColor = Color.cyan;
    [SerializeField] private float defaultIntensity = 0f;
    [SerializeField] private float calledIntensity = 5f;

    private Coroutine emissionCoroutine;

    private void Awake()
    {
        Instance = this;
        for (int i = 0; i < poolSize; i++)
        {
            GameObject exp = Instantiate(experiencePrefab, Vector3.zero, Quaternion.identity, transform);
            exp.SetActive(false);
            experiencePool.Add(exp);
        }
        SetLight(defaultColor, defaultIntensity);
        expToNextLevel = Mathf.RoundToInt(expBase); // Start with base value
    }

    public void TrySpawnExperience(Vector3 position)
    {
        if (Random.value <= dropChance)
        {
            SpawnExperience(position);
        }
    }

    private void SpawnExperience(Vector3 position)
    {
        foreach (var exp in experiencePool)
        {
            if (!exp.activeInHierarchy)
            {
                exp.transform.position = position;
                exp.SetActive(true);
                return;
            }
        }
        GameObject newExp = Instantiate(experiencePrefab, position, Quaternion.identity, transform);
        newExp.SetActive(true);
        experiencePool.Add(newExp);
    }

    // Attract all active experience orbs to a point
    public void AttractAllExperience()
    {
        foreach (var exp in experiencePool)
        {
            if (exp.activeInHierarchy)
            {
                ExpOrb orb = exp.GetComponent<ExpOrb>();
                if (orb != null)
                {
                    orb.StartAttraction(attractor);
                }
            }
        }
        TriggerLightEffect();
    }

    // Called by ExpOrb when collected
    public void CollectExperience(int amount = 1)
    {
        collectedExperience += amount;
        currentLevelExp += amount;
        if (currentLevelExp >= expToNextLevel)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        currentLevel++;
        currentLevelExp = 0;
        expToNextLevel = Mathf.RoundToInt(expToNextLevel * expGrowth); // Multiply by 1.2 each level
        if (GameManager.Instance != null)
            GameManager.Instance.StopGame();
        if (UpgradeManager.Instance != null)
            UpgradeManager.Instance.ShowUpgradeScreen();
    }

    private void SetLight(Color color, float intensity)
    {
        if (attractorLight != null)
        {
            attractorLight.color = color;
            attractorLight.intensity = intensity;
        }
    }

    private void TriggerLightEffect()
    {
        if (emissionCoroutine != null)
            StopCoroutine(emissionCoroutine);
        emissionCoroutine = StartCoroutine(LightEffectRoutine());
    }

    private IEnumerator LightEffectRoutine()
    {
        // Set to called color and intensity
        SetLight(calledColor, calledIntensity);

        // Wait 1 second
        yield return new WaitForSeconds(1f);

        // Lerp both color and intensity from called to default over 1 second
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime;
            Color lerpedColor = Color.Lerp(calledColor, defaultColor, t);
            float lerpedIntensity = Mathf.Lerp(calledIntensity, defaultIntensity, t);

            SetLight(lerpedColor, lerpedIntensity);

            yield return null;
        }
        SetLight(defaultColor, defaultIntensity);
        emissionCoroutine = null;
    }
}