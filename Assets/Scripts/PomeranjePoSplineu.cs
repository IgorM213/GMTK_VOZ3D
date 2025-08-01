using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;

public class PomeranjePoSplineu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public SplineContainer spline;
    public float speed = 3f;
    float distancePercentage = 0f;

    private Rigidbody rb;

    private float movementX;
    private float movementY;
    [SerializeField] private float power;

    float splineLength;

    private void Start()
    {
        splineLength = spline.CalculateLength();
        rb = GetComponent<Rigidbody>();
    }
    void OnMove(InputValue movementValue)
    {
        // Convert the input value into a Vector2 for movement.
        Vector2 movementVector = movementValue.Get<Vector2>();

        // Store the X and Y components of the movement.
        movementX = movementVector.x;
        movementY = movementVector.y;
    }

    // Update is called once per frame
    void Update()
    {
        distancePercentage += speed * Time.deltaTime / splineLength;

        Vector3 currentPosition = spline.EvaluatePosition(distancePercentage);
        transform.position = currentPosition;

        if (distancePercentage > 1f)
        {
            distancePercentage = 0f;
        }

        //Vector3 movement = new Vector3(movementX, 0.0f, movementY);

        Vector3 nextPosition = spline.EvaluatePosition(distancePercentage + 0.05f);
        Vector3 direction = nextPosition - currentPosition;
        transform.rotation = Quaternion.LookRotation(direction, transform.up);
    }
    //private void FixedUpdate()
    //{
    //    if (Input.GetKey(KeyCode.W))
    //    {
    //        Throttle(power);
    //    }

    //    if (Input.GetKey(KeyCode.S))
    //    {
    //        Throttle(-power);
    //    }
    //}

    private void Throttle(float power)
    {
        Vector3 dir = power * transform.forward;
        rb.AddForce(dir);
    }
}
