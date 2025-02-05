using UnityEngine;

public class AutoAttack : MonoBehaviour
{
    public TargetingSystem targetingSystem; 
    public float attackRange = 2f;          
    public float attackCooldown = 1.5f;     
    public int attackDamage = 10;           

    private float lastAttackTime = 0f;

    void Update()
    {
        if (targetingSystem.currentTarget != null && targetingSystem.autoAttackEnabled)
        {
            AttackTarget();
        }
    }

    void AttackTarget()
    {
        Enemy enemy = targetingSystem.currentTarget.GetComponent<Enemy>();
        if (enemy == null)
        {
            enemy = targetingSystem.currentTarget.GetComponentInParent<Enemy>();
        }
        
        if (enemy == null || enemy.currentHealth <= 0)
        {
            targetingSystem.ClearTarget();
            return;
        }
        
        float distance = Vector3.Distance(transform.position, targetingSystem.currentTarget.position);
        if (distance <= attackRange && Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;
            enemy.TakeDamage(attackDamage);
        }
    }
}