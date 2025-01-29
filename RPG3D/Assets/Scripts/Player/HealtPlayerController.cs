using UnityEngine;

public class HealthPlayerController : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    public delegate void HealthChanged(int currentHealth, int maxHealth);
    public event HealthChanged OnHealthChanged;

    private void Start()
    {
        currentHealth = maxHealth;

        // Вызываем событие при старте, чтобы обновить UI или другие элементы
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log($"Player took {damage} damage. Current health: {currentHealth}");

        // Вызываем событие изменения здоровья
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // Вызываем событие изменения здоровья
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    private void Die()
    {
        Debug.Log("Player has died!");
        // Здесь можно добавить логику смерти игрока (перезагрузка уровня, завершение игры и т.д.)
    }
}