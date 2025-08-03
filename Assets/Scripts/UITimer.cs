using UnityEngine;
using TMPro; // Use TMPro if using TextMeshPro

public class UITimer : MonoBehaviour
{
    public TMPro.TextMeshProUGUI timerText; // Use TMPro.TextMeshProUGUI if using TextMeshPro

    private float timer = 0f;
    private bool isRunning = true;

    void Start()
    {
        StartTimer();
    }

    void Update()
    {
        if (isRunning)
        {
            timer += Time.deltaTime;
            int minutes = Mathf.FloorToInt(timer / 60F);
            int seconds = Mathf.FloorToInt(timer % 60F);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    // Call this to stop the timer
    public void StopTimer()
    {
        isRunning = false;
    }

    // Call this to start/reset the timer
    public void StartTimer()
    {
        timer = 0f;
        isRunning = true;
    }
}