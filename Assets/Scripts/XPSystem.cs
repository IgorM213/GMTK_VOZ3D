using UnityEngine;
using System;

[Serializable]
public class XPSystem
{
    public int Level { get; private set; }
    public int CurrentXP { get; private set; }
    public int XPToNextLevel { get; private set; }

    // You can tweak this growth factor for difficulty curve
    [SerializeField] private float xpGrowthRate = 1.2f; // 20% more per level
    [SerializeField] private int baseXP = 100;

    public event Action<int> OnLevelUp; // Passes new level

    public XPSystem()
    {
        Level = 1;
        CurrentXP = 0;
        XPToNextLevel = baseXP;
    }

    // Call this to add XP
    public void AddXP(int amount)
    {
        CurrentXP += amount;
        while (CurrentXP >= XPToNextLevel)
        {
            CurrentXP -= XPToNextLevel;
            Level++;
            OnLevelUp?.Invoke(Level);
            XPToNextLevel = CalculateXPForLevel(Level);
        }
    }

    // XP requirement formula, can be changed for different curves
    private int CalculateXPForLevel(int level)
    {
        return Mathf.RoundToInt(baseXP * Mathf.Pow(xpGrowthRate, level - 1));
    }

    // Optional: to get percentage to next level for UI
    public float GetProgress()
    {
        return (float)CurrentXP / XPToNextLevel;
    }

    // Optional: reset XP system
    public void Reset()
    {
        Level = 1;
        CurrentXP = 0;
        XPToNextLevel = baseXP;
    }
}