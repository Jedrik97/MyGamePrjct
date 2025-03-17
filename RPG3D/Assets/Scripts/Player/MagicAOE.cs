/*
using UnityEngine;

public class MagicAOE : MonoBehaviour
{
    public float radius = 10f;
    private float duration = 10f;
    private float tickRate = 0.5f; // Damage every 0.5 seconds
    private float baseDamage = 10f;
    private float timer;
    private bool isActive;

    // Serialized private references
    [SerializeField] private PlayerStats playerStats;  // Serialized private reference to PlayerStats
    [SerializeField] private LayerMask enemyLayer;     // Serialized private reference to the enemy layer

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) // Example activation key
        {
            Activate();
        }

        if (isActive)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                ApplyAOEDamage();
                timer = tickRate;
            }

            duration -= Time.deltaTime;
            if (duration <= 0)
            {
                isActive = false;
            }
        }
    }

    public void Activate()
    {
        // Use the direct reference for PlayerStats
        if (playerStats != null)
        {
            isActive = true;
            duration = 10f;
            timer = tickRate;
        }
        
    }

    private void ApplyAOEDamage()
    {
        // Use the direct reference for PlayerStats
        if (playerStats != null)
        {
            float damageMultiplier = playerStats.level;
            float totalDamage = baseDamage * damageMultiplier;

            Collider[] hitEnemies = Physics.OverlapSphere(transform.position, radius, enemyLayer);
            foreach (Collider enemyCol in hitEnemies)
            {
                Enemy enemy = enemyCol.GetComponent<Enemy>();
                if (enemy != null)
                {
                    // Casting totalDamage to int to avoid the type mismatch error
                    enemy.TakeDamage((int)(totalDamage * tickRate));
                }
            }
        }
        else
        {
            // Fallback behavior when playerStats is not assigned
            Debug.LogWarning("PlayerStats is not assigned.");
        }
    }
}
*/
