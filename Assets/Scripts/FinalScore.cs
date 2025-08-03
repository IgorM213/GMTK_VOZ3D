using UnityEngine;

public class FinalScore : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] public TMPro.TextMeshProUGUI timerText;
    void Start()
    {
        // timerText.text = UITimer.timer;
        int minutes = Mathf.FloorToInt(UITimer.finalTime / 60F);
        int seconds = Mathf.FloorToInt(UITimer.finalTime % 60F);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
