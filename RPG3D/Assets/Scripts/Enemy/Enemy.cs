using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public string enemyName = "Skeleton";
    public int maxHealth = 100;
    public int currentHealth = 100;
    public Slider healthBar;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0) 
            currentHealth = 0;
        
        UpdateHealthBar();

        if (currentHealth == 0)
        {
            Die();
        }
    }
    void Die()
    {
        Debug.Log(enemyName + " погиб!");
        gameObject.SetActive(false); 
    }
    void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }
    }
}