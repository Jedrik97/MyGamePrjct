using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Stats")]
    public string enemyName;
    public int maxHealth = 100;
    public int currentHealth = 100;
    public int attackDamage = 10;

    [Header("Attack Parameters")]
    public float attackRange = 1.5f;
    public float attackDelay = 0.5f;
    public float chaseSpeed = 3.5f;
    public float attackSpeed = 0f;

    [Header("UI Elements")]
    [SerializeField] private TMP_Text enemyNameText;
    [SerializeField] private Slider healthBar;
    [SerializeField] private Image targetFrame;

    [Header("Player Reference")]
    [SerializeField] private PlayerStats playerStats;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
        HideUI();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        UpdateHealthBar();
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        UpdateHealthBar();
    }

    private void Die()
    {
        if (playerStats != null)
        {
            playerStats.EnemyKilled();
        }
        Destroy(gameObject);
    }

    void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    public void ShowUI()
    {
        if (enemyNameText != null)
        {
            enemyNameText.gameObject.SetActive(true);
            enemyNameText.text = enemyName;
        }
        if (healthBar != null)
        {
            healthBar.gameObject.SetActive(true);
            UpdateHealthBar();
        }
        if (targetFrame != null)
        {
            targetFrame.enabled = true;
        }
    }

    public void HideUI()
    {
        if (enemyNameText != null)
        {
            enemyNameText.gameObject.SetActive(false);
        }
        if (healthBar != null)
        {
            healthBar.gameObject.SetActive(false);
        }
        if (targetFrame != null)
        {
            targetFrame.enabled = false;
        }
    }
}
