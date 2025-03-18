using UnityEngine;

public class WeaponDamage : MonoBehaviour
{
    public float damage = 25f; // Урон оружия
    public LayerMask enemyLayer; // Слой врагов

    private void OnTriggerEnter(Collider other)
    {
        if ((enemyLayer.value & (1 << other.gameObject.layer)) > 0) // Проверяем, что попали во врага
        {
            EnemyBase enemy = other.GetComponent<EnemyBase>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
    }
}