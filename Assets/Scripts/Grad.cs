using System;
using UnityEngine;

public class Grad : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private float hp;
    [SerializeField] private float MaxHp = 100;
    public HealthBar healthBar;
    void Start()
    {
        hp = MaxHp;
        healthBar.SetHealth(hp / MaxHp);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DealDmg(float enemyDmg)
    {
        hp -= enemyDmg;
        healthBar.SetHealth(hp / MaxHp);
        Debug.Log(hp + "trenutni hp");
    }
}
