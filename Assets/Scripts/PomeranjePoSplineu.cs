using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;

public class PomeranjePoSplineu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Rigidbody rb;

    [SerializeField] private float power;
    //public float Speed = 5f;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.W))
        {
            Throttle(power);
        }

        if (Input.GetKey(KeyCode.S))
        {
            Throttle(-power);
        }
    }

    private void Throttle(float power)
    {
        Vector3 dir = power * transform.forward;
        rb.AddForce(dir);
    }
}
