using UnityEngine;

public class PlayerXP : MonoBehaviour
{
    public XPSystem xpSystem = new XPSystem();

    public void AddXP(int xp)
    {
        xpSystem.AddXP(xp);
        Debug.Log($"Gained {xp} XP! Total: {xpSystem.CurrentXP}, Level: {xpSystem.Level}");
    }
}