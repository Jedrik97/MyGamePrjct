using System;
using System.Collections;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public static event Action<bool> OnAttackStateChanged; // Блокировка движения

    [Header("Attack Settings")]
    [SerializeField] private Animator animator;
    [SerializeField] private Collider weaponCollider;
    [SerializeField] private LayerMask enemyLayer;

    [Header("AOE Attack")]
    [SerializeField] private float aoeRadius = 3f;
    [SerializeField] private float aoeDamage = 50f;
    [SerializeField] private float aoeCooldown = 6f; // Кулдаун 6 сек
    [SerializeField] private GameObject magicEffect;
    [SerializeField] private float magicEffectDuration = 10f; // Эффект 10 сек

    private bool isAttacking = false;
    private bool isAoeActive = false; // ❗ Теперь АОЕ нельзя использовать, пока работает эффект

    void Start()
    {
        if (magicEffect != null)
            magicEffect.SetActive(false); // Скрываем эффект при старте
    }

    void Update()
    {
        if (!isAttacking)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) StartCoroutine(PerformAttack("Attack1"));
            if (Input.GetKeyDown(KeyCode.Alpha2)) StartCoroutine(PerformAttack("Attack2"));
            if (Input.GetKeyDown(KeyCode.Alpha3)) StartCoroutine(PerformAttack("Attack3"));
            if (Input.GetKeyDown(KeyCode.Alpha4)) StartCoroutine(PerformAttack("Attack4"));
            if (Input.GetKeyDown(KeyCode.Alpha5)) StartCoroutine(PerformAttack("Attack5"));
        }

        if (Input.GetKeyDown(KeyCode.F) && !isAoeActive) // ❗ Проверяем, не активен ли уже АОЕ
        {
            StartCoroutine(PerformAoeAttack());
        }
    }

    private IEnumerator PerformAttack(string attackName)
    {
        isAttacking = true;
        OnAttackStateChanged?.Invoke(true);

        animator.Play(attackName);

        yield return new WaitForSeconds(0.1f);
        EnableWeaponCollider(true);

        yield return new WaitForSeconds(GetAnimationLength(attackName) - 0.2f);
        EnableWeaponCollider(false);

        isAttacking = false;
        OnAttackStateChanged?.Invoke(false);
    }

    private IEnumerator PerformAoeAttack()
    {
        if (isAoeActive) yield break; // Блокируем повторное нажатие
        isAoeActive = true;
        isAttacking = true;
        OnAttackStateChanged?.Invoke(true);

        // 🔥 Запускаем анимацию (3 секунды)
        if (animator != null)
        {
            animator.SetTrigger("AoeAttack");
        }
        else
        {
            Debug.LogError("Animator не назначен в PlayerCombat!");
        }

        yield return new WaitForSeconds(3f); // Ждём завершение анимации перед уроном

        // 🔥 Включаем магический эффект
        if (magicEffect != null)
        {
            magicEffect.SetActive(true);
            StartCoroutine(DisableMagicEffectAfterDelay(10f)); // Скрыть через 10 секунд
        }

        PerformAoeDamage(); // 🔥 Наносим урон

        yield return new WaitForSeconds(0.5f);

        isAttacking = false;
        OnAttackStateChanged?.Invoke(false);

        yield return new WaitForSeconds(6f); // Кулдаун 6 секунд

        isAoeActive = false; // Теперь можно снова кастовать
    }

    private IEnumerator DisableMagicEffectAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        if (magicEffect != null)
            magicEffect.SetActive(false);

        isAoeActive = false; // ❗ Теперь АОЕ можно снова использовать
    }

    private void PerformAoeDamage()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, aoeRadius, enemyLayer);

        foreach (Collider enemy in hitEnemies)
        {
            EnemyBase enemyBase = enemy.GetComponent<EnemyBase>();
            if (enemyBase != null)
            {
                enemyBase.TakeDamage(aoeDamage);
            }
        }
    }

    private void EnableWeaponCollider(bool enable)
    {
        if (weaponCollider != null)
        {
            weaponCollider.enabled = enable;
        }
    }

    private float GetAnimationLength(string animationName)
    {
        RuntimeAnimatorController ac = animator.runtimeAnimatorController;
        foreach (AnimationClip clip in ac.animationClips)
        {
            if (clip.name == animationName)
            {
                return clip.length;
            }
        }
        return 0.5f;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aoeRadius);
    }
}
