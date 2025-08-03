using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private CanvasGroup upgradeCanvasGroup;
    [SerializeField] private Button[] cardButtons; // Assign 3 buttons in Inspector
    [SerializeField] private TMP_Text[] cardTitles; // Assign 3 TMP_Texts for titles
    [SerializeField] private TMP_Text[] cardDescriptions; // Assign 3 TMP_Texts for descriptions

    [Header("Card Animation")]
    [SerializeField] private AnimationCurve cardAppearCurve = null;
    [SerializeField] private float cardAppearDuration = 0.4f;
    [SerializeField] private float cardStartY = -200f;
    [SerializeField] private float cardEndY = 0f;
    [SerializeField] private float cardStartScale = 0.8f;
    [SerializeField] private float cardEndScale = 1f;
    [SerializeField] private Color cardDefaultTint = new Color(0, 0, 0, 0.5f);
    [SerializeField] private Color cardNoTint = new Color(0, 0, 0, 0f);

    [Header("Rarity Tints")]
    [SerializeField] private Color rarityCommonTint = new Color(0.5f, 0.5f, 0.5f, 1f); // Grey
    [SerializeField] private Color rarityRareTint = new Color(0.2f, 0.4f, 1f, 1f);     // Blue
    [SerializeField] private Color rarityEpicTint = new Color(0.6f, 0.2f, 0.8f, 1f);   // Purple
    [SerializeField] private Color rarityLegendaryTint = new Color(1f, 0.65f, 0.1f, 1f); // Gold-Orange

    public enum TurretUpgradeType
    {
        Damage,
        FireRate,
        Radius,
        NewTurret1,
        NewTurret2
    }

    public enum UpgradeRarity
    {
        Common,
        Rare,
        Epic,
        Legendary
    }

    [System.Serializable]
    public class TurretUpgrade
    {
        public TurretUpgradeType type;
        public string displayName;
        public string description;
        public float basePercent; // base percent for this upgrade (e.g. 0.05 for 5%)
        public UpgradeRarity rarity; // Now set in the inspector or by code
    }

    [Header("Possible Upgrades (base values set as common)")]
    [SerializeField] private TurretUpgrade[] possibleUpgrades;

    private TurretUpgrade[] currentChoices = new TurretUpgrade[3];

    // Rarity ratios: 10:5:2:1 (common:rare:epic:legendary)
    private readonly int[] rarityWeights = { 10, 5, 2, 1 };

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            TrainManager.Instance.ActivateNextTurret(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            TrainManager.Instance.ActivateNextTurret(1);
        }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        upgradeCanvasGroup.alpha = 0f;
        upgradeCanvasGroup.interactable = false;
        upgradeCanvasGroup.blocksRaycasts = false;
    }

    public void ShowUpgradeScreen()
    {
        PickRandomUpgrades();
        for (int i = 0; i < cardButtons.Length; i++)
        {
            int idx = i;
            cardButtons[i].onClick.RemoveAllListeners();
            cardButtons[i].onClick.AddListener(() => OnCardChosen(idx));

            // Set title and description
            if (cardTitles != null && cardTitles.Length > i && cardTitles[i] != null)
                cardTitles[i].text = currentChoices[i].displayName;
            if (cardDescriptions != null && cardDescriptions.Length > i && cardDescriptions[i] != null)
                cardDescriptions[i].text = GetUpgradeDescription(currentChoices[i]);

            // Set initial state
            RectTransform rt = cardButtons[i].GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, cardStartY);
            rt.localScale = Vector3.one * cardStartScale;

            // Set tint based on rarity
            Image img = cardButtons[i].GetComponent<Image>();
            if (img != null)
                img.color = GetTintForRarity(currentChoices[i].rarity);

            // Add hover events (no tints passed)
            CardHoverHandler hover = cardButtons[i].GetComponent<CardHoverHandler>();
            if (hover == null)
                hover = cardButtons[i].gameObject.AddComponent<CardHoverHandler>();
            hover.Init(this, cardButtons[i], cardEndScale, cardStartScale);
        }
        StartCoroutine(LerpCanvasAlpha(1f, 0.5f));
        StartCoroutine(AnimateCardsIn());
    }

    private void OnCardChosen(int index)
    {
        ApplyUpgrade(currentChoices[index]);
        StartCoroutine(LerpCanvasAlpha(0f, 0.5f));
        upgradeCanvasGroup.interactable = false;
        upgradeCanvasGroup.blocksRaycasts = false;
        GameManager.Instance.ContinueGame();
    }

    private void PickRandomUpgrades()
    {
        // Build pool with all rarities for each upgrade
        List<TurretUpgrade> pool = new List<TurretUpgrade>();
        foreach (var upgrade in possibleUpgrades)
        {
            int minRarity = (int)upgrade.rarity;
            for (int r = minRarity; r < 4; r++)
            {
                TurretUpgrade clone = new TurretUpgrade
                {
                    type = upgrade.type,
                    displayName = upgrade.displayName + " [" + ((UpgradeRarity)r).ToString() + "]",
                    description = upgrade.description,
                    basePercent = upgrade.basePercent,
                    rarity = (UpgradeRarity)r
                };
                pool.Add(clone);
            }
        }

        currentChoices = new TurretUpgrade[3];
        HashSet<string> used = new HashSet<string>();
        for (int i = 0; i < 3; i++)
        {
            TurretUpgrade chosen;
            int safety = 0;
            do
            {
                chosen = GetRandomUpgradeWithRarity(pool);
                safety++;
            } while (used.Contains(chosen.type + "_" + chosen.rarity) && safety < 20);

            used.Add(chosen.type + "_" + chosen.rarity);
            currentChoices[i] = chosen;
        }
    }

    private TurretUpgrade GetRandomUpgradeWithRarity(List<TurretUpgrade> pool)
    {
        // Weighted random for rarity
        int totalWeight = 0;
        foreach (var w in rarityWeights) totalWeight += w;
        int roll = Random.Range(0, totalWeight);
        UpgradeRarity rarity = UpgradeRarity.Common;
        int acc = 0;
        for (int i = 0; i < rarityWeights.Length; i++)
        {
            acc += rarityWeights[i];
            if (roll < acc)
            {
                rarity = (UpgradeRarity)i;
                break;
            }
        }
        // Pick random upgrade of that rarity
        List<TurretUpgrade> upgradesOfType = new List<TurretUpgrade>();
        foreach (var upgrade in pool)
            if (upgrade.rarity == rarity)
                upgradesOfType.Add(upgrade);
        if (upgradesOfType.Count == 0)
            upgradesOfType.Add(pool[Random.Range(0, pool.Count)]);
        return upgradesOfType[Random.Range(0, upgradesOfType.Count)];
    }

    private string GetUpgradeDescription(TurretUpgrade upgrade)
    {
        float percent = GetPercentForRarity(upgrade.rarity, upgrade.basePercent);
        string percentStr = $" <color=#{ColorUtility.ToHtmlStringRGBA(GetTintForRarity(upgrade.rarity))}>+{Mathf.RoundToInt(percent * 100f)}%</color>";
        if(percent == 0)
        {
            percentStr = "";
        }
        return upgrade.description + percentStr;
    }

    private float GetPercentForRarity(UpgradeRarity rarity, float basePercent)
    {
        switch (rarity)
        {
            case UpgradeRarity.Common: return basePercent;
            case UpgradeRarity.Rare: return basePercent * 1.5f;
            case UpgradeRarity.Epic: return basePercent * 2.4f;
            case UpgradeRarity.Legendary: return basePercent * 4f;
            default: return basePercent;
        }
    }

    private Color GetTintForRarity(UpgradeRarity rarity)
    {
        switch (rarity)
        {
            case UpgradeRarity.Common: return rarityCommonTint;
            case UpgradeRarity.Rare: return rarityRareTint;
            case UpgradeRarity.Epic: return rarityEpicTint;
            case UpgradeRarity.Legendary: return rarityLegendaryTint;
            default: return cardDefaultTint;
        }
    }

    private void ApplyUpgrade(TurretUpgrade upgrade)
    {
        float percent = GetPercentForRarity(upgrade.rarity, upgrade.basePercent);

        switch (upgrade.type)
        {
            case TurretUpgradeType.Damage:
                TrainManager.Instance.UpgradeTurretDamage(percent);
                break;
            case TurretUpgradeType.FireRate:
                TrainManager.Instance.UpgradeTurretFireRate(percent);
                break;
            case TurretUpgradeType.Radius:
                TrainManager.Instance.UpgradeTurretRadius(percent);
                break;
            case TurretUpgradeType.NewTurret1:
                TrainManager.Instance.ActivateNextTurret(0);
                break;
            case TurretUpgradeType.NewTurret2:
                TrainManager.Instance.ActivateNextTurret(1);
                break;
        }
    }

    private IEnumerator LerpCanvasAlpha(float target, float duration)
    {
        float start = upgradeCanvasGroup.alpha;
        float elapsed = 0f;
        upgradeCanvasGroup.interactable = target > 0.5f;
        upgradeCanvasGroup.blocksRaycasts = target > 0.5f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            upgradeCanvasGroup.alpha = Mathf.Lerp(start, target, elapsed / duration);
            yield return null;
        }
        upgradeCanvasGroup.alpha = target;
    }

    private IEnumerator AnimateCardsIn()
    {
        float t = 0f;
        while (t < cardAppearDuration)
        {
            t += Time.unscaledDeltaTime;
            float eval = cardAppearCurve != null ? cardAppearCurve.Evaluate(Mathf.Clamp01(t / cardAppearDuration)) : Mathf.Clamp01(t / cardAppearDuration);
            for (int i = 0; i < cardButtons.Length; i++)
            {
                RectTransform rt = cardButtons[i].GetComponent<RectTransform>();
                float y = Mathf.Lerp(cardStartY, cardEndY, eval);
                rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, y);
            }
            yield return null;
        }
        for (int i = 0; i < cardButtons.Length; i++)
        {
            RectTransform rt = cardButtons[i].GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, cardEndY);
        }
    }
}