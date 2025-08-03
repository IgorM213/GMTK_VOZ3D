using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Grad : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private float hp;
    [SerializeField] private float MaxHp = 100;
    public HealthBar healthBar;
    void Start()
    {
        hp = MaxHp;
        if(healthBar != null)
            healthBar.SetHealth(hp / MaxHp);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DealDmg(float enemyDmg)
{
    hp -= enemyDmg;
    Debug.Log(hp + " trenutni hp");

    if (hp <= 0)
    {
        UITimer.finalTime = UITimer.timer;
        SceneManager.LoadScene("End screen"); 
    }
}
}
