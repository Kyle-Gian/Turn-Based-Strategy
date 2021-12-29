using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] int maxHealth = 5;
    private int currentHealth;
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0)
        {
            Debug.Log("Dead");
            currentHealth = 0;
            return;
        }
        Debug.Log($"{gameObject.name} was attacked for {damage}, current Health is {currentHealth}");
        currentHealth -= damage;
    }
    
}
