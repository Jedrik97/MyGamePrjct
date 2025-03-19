using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private Collider weaponCollider;
    [SerializeField] private int weaponDamage = 25;
    private float disableDelay = 2f; 

    private void Start()
    {
        if (weaponCollider)
            weaponCollider.enabled = false;
    }

    public void EnableCollider(bool enable)
    {
        if (weaponCollider)
        {
            if (enable)
            {
                weaponCollider.enabled = true;
            }
            else
            {
                Invoke(nameof(DisableCollider), disableDelay);
            }
        }
    }

    private void DisableCollider()
    {
        if (weaponCollider)
        {
            weaponCollider.enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        if (!other.CompareTag("Enemy")) return;

        if (other.TryGetComponent<EnemyBase>(out EnemyBase enemyBase))
        {
            enemyBase.TakeDamage(weaponDamage);
        }
    }
}