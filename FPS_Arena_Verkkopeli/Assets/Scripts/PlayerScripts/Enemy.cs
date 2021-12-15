using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Enemy : MonoBehaviour
{
    public int maxHealth = 50;
    public int currentHealth;
    public float TickRate = 3.0f;
    public int TickDamage = 10;
    public bool inZone = true;

    public HealthBar healthBar;

    public GameObject gameObjectToEnable;



    private void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        InvokeRepeating("ZoneDamage", 0.0f, TickRate);
    }

    void ZoneDamage()
    {
        if (!inZone)
        {
            currentHealth -= TickDamage;
        }
    }
    public void Update()
    {
        healthBar.SetHealth(currentHealth);
    }

    public void TakeDamage (int damage)
    {  
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);
        if (currentHealth < 1)
            Die();
    }
    private void OnTriggerExit(Collider col)
    {
        if(col.CompareTag("ZoneWall"))
        {
            inZone = false;
            currentHealth -= TickDamage;
            if (currentHealth <= 0)
                Die();
        }
    }
    private void OnTriggerEnter(Collider col)
    {
        if(col.CompareTag("ZoneWall"))
            inZone = true;
    }
    void Die()
    {
        gameObjectToEnable.SetActive(false);
    }
}
