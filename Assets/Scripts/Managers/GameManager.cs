using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private Coroutine timeScaleCoroutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        Time.timeScale = 1f;
    }

    public void StopGame()
    {
        LerpTimeScale(0f, 0.5f);
    }

    public void ContinueGame()
    {
        LerpTimeScale(1f, 0.5f);
    }

    private void LerpTimeScale(float target, float duration)
    {
        if (timeScaleCoroutine != null)
            StopCoroutine(timeScaleCoroutine);
        timeScaleCoroutine = StartCoroutine(TimeScaleRoutine(target, duration));
    }

    private IEnumerator TimeScaleRoutine(float target, float duration)
    {
        float start = Time.timeScale;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Lerp(start, target, elapsed / duration);
            yield return null;
        }
        Time.timeScale = target;
        timeScaleCoroutine = null;
    }
}