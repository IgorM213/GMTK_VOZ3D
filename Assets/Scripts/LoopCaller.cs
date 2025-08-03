using UnityEngine;

public class LoopCaller : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Train"))
        {
            if (ExperienceManager.Instance != null)
            {
                ExperienceManager.Instance.AttractAllExperience();
            }
        }
    }
}