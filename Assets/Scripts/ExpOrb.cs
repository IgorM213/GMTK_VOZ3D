using UnityEngine;

public class ExpOrb : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private AnimationCurve attractionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private int expValue = 1;

    private Transform attractor;
    private float attractionTime;
    private Vector3 startPos;
    private bool isAttracting = false;

    public void StartAttraction(Transform attractorTransform)
    {
        attractor = attractorTransform;
        startPos = transform.position;
        attractionTime = 0f;
        isAttracting = true;
    }

    void Update()
    {
        if (isAttracting && attractor != null)
        {
            attractionTime += Time.deltaTime * moveSpeed;
            float t = attractionCurve.Evaluate(attractionTime);

            // Move towards attractor using curve
            transform.position = Vector3.Lerp(startPos, attractor.position, t);

            // If close enough, collect
            if (Vector3.Distance(transform.position, attractor.position) < 0.2f || t >= 1f)
            {
                isAttracting = false;
                ExperienceManager.Instance.CollectExperience(expValue);
                gameObject.SetActive(false);
            }
        }
    }
}