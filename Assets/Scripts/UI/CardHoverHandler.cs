using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class CardHoverHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private UpgradeManager manager;
    private Button button;
    private float targetScale;
    private float defaultScale;
    private Coroutine scaleRoutine;
    [SerializeField] private Image blackTintImage;
    [SerializeField] Color defaultTint;
    [SerializeField] Color targetTint;

    public void Init(UpgradeManager mgr, Button btn, float tScale, float dScale)
    {
        manager = mgr;
        button = btn;
        targetScale = tScale;
        defaultScale = dScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (scaleRoutine != null) StopCoroutine(scaleRoutine);
        // scaleRoutine = StartCoroutine(LerpScaleAndTint(targetScale, targetTint));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (scaleRoutine != null) StopCoroutine(scaleRoutine);
        // scaleRoutine = StartCoroutine(LerpScaleAndTint(defaultScale, defaultTint));
    }

    private IEnumerator LerpScaleAndTint(float toScale, Color toTint)
    {
        float duration = 0.2f;
        float t = 0f;
        Vector3 startScale = button.transform.localScale;
        Color startTint = blackTintImage != null ? blackTintImage.color : Color.white;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float lerp = t / duration;
            button.transform.localScale = Vector3.Lerp(startScale, Vector3.one * toScale, lerp);
            if (blackTintImage != null)
                blackTintImage.color = Color.Lerp(startTint, toTint, lerp);
            yield return null;
        }
        button.transform.localScale = Vector3.one * toScale;
        if (blackTintImage != null)
            blackTintImage.color = toTint;
    }
}