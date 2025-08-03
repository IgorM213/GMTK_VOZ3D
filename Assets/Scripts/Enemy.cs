using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float maxHP;
    [SerializeField] private float hp;
    [SerializeField] private float atk;
    [SerializeField] private float mvSpeed;
    [SerializeField] private ParticleSystem blood;

    private Transform targetPoint;
    private Grad grad;

    // Called by EnemyManager after activation
    public void Initialize(Transform cityTransform)
    {
        hp = maxHP; // Reset health to max
        targetPoint = cityTransform;
        grad = cityTransform.GetComponent<Grad>();
    }

    void LateUpdate()
    {   
        if (targetPoint != null)
        {
            float step = mvSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, step);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("City") && grad != null)
        {
            grad.DealDmg(atk);
            gameObject.SetActive(false); // Pooling: just deactivate
        }
        if (other.CompareTag("Train"))
        {
            StartCoroutine(DieByTrain()); // instant kill
        }
    }
    private IEnumerator DieByTrain()
    {
        yield return new WaitForSeconds(1f); // delay od 1 sekunde
        blood.Play();
        TakeDmg(maxHP);
    }
    public void TakeDmg(float dmg)
    {
        if (blood != null)
            blood.Play();
        hp -= dmg;
        if (hp <= 0)
        {
            
            // Drop experience with chance
            if (ExperienceManager.Instance != null)
                ExperienceManager.Instance.TrySpawnExperience(transform.position);

            gameObject.SetActive(false);
        }
        Debug.Log(hp + " enemy health");
    }
}
