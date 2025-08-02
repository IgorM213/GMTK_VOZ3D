using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(Rigidbody))]
public class Sine : MonoBehaviour
{
    public SplineContainer splineContainer;
    public float moveSpeed = 1f;
    [SerializeField] private float power;
    private Rigidbody rb;

    private float velocity = 0f;

    [SerializeField] private float acceleration = 0.05f;
    [SerializeField] private float deceleration = 0.001f;
    [SerializeField] private float brakingForce = 0.01f;
    [SerializeField] private float maxSpeed = 2f;


    private float currentSplinePosition = 0f;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (splineContainer == null)
        {
            Debug.Log("zavsrio se");

            return;
        }

        // Ako držiš W ? ubrzaj
        if (Input.GetKey(KeyCode.W))
        {
            velocity += acceleration/2 * Time.deltaTime;
        }

        // Ako držiš S ? ko?i
        else if (Input.GetKey(KeyCode.S))
        {
            if (velocity > 0f)
            {
                velocity -= brakingForce/2 * Time.deltaTime;
            }
        }
        // Ako ništa ne držiš ? prirodno usporavaj (trenje)
        else
        {
            if (velocity > 0)
                velocity -= deceleration/3 * Time.deltaTime;
            else if (velocity < 0)
                velocity += deceleration/3 * Time.deltaTime;
        }

        // Ograni?i brzinu
        velocity = Mathf.Clamp(velocity, -maxSpeed, maxSpeed);

        // Pomeri voz po spline-u
        currentSplinePosition += velocity * Time.deltaTime / splineContainer.Splines[0].GetLength();
        currentSplinePosition = (currentSplinePosition + 1f) % 1f;



        // Get position and tangent (direction) from the spline
        Vector3 position = splineContainer.EvaluatePosition(currentSplinePosition);
        //Debug.Log(position);

        Vector3 tangent = splineContainer.EvaluateTangent(currentSplinePosition);

        // Update player's position and rotation
        transform.position = position;
        transform.rotation = Quaternion.LookRotation(tangent);
    }

    private void Throttle(float power)
    {
        Vector3 dir = power * transform.forward;
        rb.AddForce(dir);
    }
    public float vratiVelocity()
    {
        return velocity;
    }
    public float vratiTrenutniT()
    {
        return currentSplinePosition;
    }
}
