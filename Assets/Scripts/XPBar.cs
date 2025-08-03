using UnityEngine;
using UnityEngine.UI;

public class XPBar : MonoBehaviour
{
    [SerializeField] private Image fillImage;

    // Call this to update health bar value (0.0 to 1.0)
    public void SetXP(float xpNormalized)
    {
        // Debug.Log("xpnormalized: " + xpNormalized);
        fillImage.fillAmount = Mathf.Clamp01(xpNormalized);
    }
}