using System;
using UnityEngine;

public class Grad : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private float hp;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DealDmg(float enemyDmg)
    {
        hp -= enemyDmg;
        Debug.Log(hp + "trenutni hp");
    }
}
