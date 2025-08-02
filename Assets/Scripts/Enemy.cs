using UnityEngine;

public class Enemy : MonoBehaviour
{

    [SerializeField] private float hp;
    [SerializeField] private float atk;
    [SerializeField] private float mvSpeed;
    [SerializeField] GameObject city;
    private Transform targetPoint;
    private Grad grad;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        targetPoint = city.GetComponent<Transform>();
        grad = city.GetComponent<Grad>();
    }

    // Update is called once per frame
    void Update()
    {
        if (targetPoint != null)
        {
            // Calculate the step size for this frame
            float step = mvSpeed * Time.deltaTime;

            // Move the enemy towards the target point
            transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, step);
        }
        //DealDmg(city);
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("udario1");
        if (other.gameObject.CompareTag("City"))
        {

            //other.gameObject.SetActive(false);
            //Debug.Log("udario");
            grad.DealDmg(atk);
            this.gameObject.SetActive(false);
            
        }
    }
}
