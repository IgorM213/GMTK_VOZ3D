using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image fillImage;

    // Call this to update health bar value (0.0 to 1.0)
    public void SetHealth(float healthNormalized)
    {
        fillImage.fillAmount = Mathf.Clamp01(healthNormalized);
    }
}