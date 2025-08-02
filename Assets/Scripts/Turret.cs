using UnityEngine;

public class Turret :MonoBehaviour
{

    [SerializeField] float dmg;
    [SerializeField] float radius;


    private void Start()
    {
        SphereCollider colider = GetComponent<SphereCollider>();
        colider.radius = radius;
    }

    private void Update()
    {
        Debug.Log(radius);
    }

    public void Fire()
    {

    }
}
