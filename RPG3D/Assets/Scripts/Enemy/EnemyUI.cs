using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TMP_Text enemyNameText;
    [SerializeField] private Slider healthBar;
    [SerializeField] private Image enemyCircle;

    private EnemyBase enemyBase;

    private void Start()
    {
        enemyBase = GetComponent<EnemyBase>();

        if (enemyBase)
        {
            enemyBase.OnHealthChanged += UpdateHealthUI;
            enemyBase.OnDeath += HideUI;
        }

        InitializeUI();
        HideUI();
    }

    private void OnDestroy()
    {
        if (enemyBase)
        {
            enemyBase.OnHealthChanged -= UpdateHealthUI;
            enemyBase.OnDeath -= HideUI;
        }
    }

    private void InitializeUI()
    {
        if (enemyNameText)
        {
            enemyNameText.text = enemyBase.enemyName;
        }

        if (healthBar)
        {
            healthBar.maxValue = enemyBase.maxHealth;
            healthBar.value = enemyBase.currentHealth;
        }
    }

    public void UpdateHealthUI(float currentHealth)
    {
        if (healthBar)
        {
            healthBar.value = currentHealth;
        }
    }

    public void ShowUI()
    {
        if (enemyNameText) enemyNameText.gameObject.SetActive(true);
        if (healthBar) healthBar.gameObject.SetActive(true);
        if (enemyCircle) enemyCircle.gameObject.SetActive(true);
    }

    public void HideUI()
    {
        if (enemyNameText) enemyNameText.gameObject.SetActive(false);
        if (healthBar) healthBar.gameObject.SetActive(false);
        if (enemyCircle) enemyCircle.gameObject.SetActive(false);
    }
}