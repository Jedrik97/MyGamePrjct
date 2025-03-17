/*// TargetingSystem.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TargetingSystem : MonoBehaviour
{
    [Header("Target Search Settings")]
    public LayerMask enemyLayer;
    public float tabSearchRadius = 15f;

    [Header("Player Reference")]
    public Transform playerTransform;

    private Image targetFrame;
    private TMP_Text enemyNameText;
    private Slider enemyHealthBar;
    private Transform lastHoveredTarget;

    public Transform currentTarget { get; private set; }
    public bool autoAttackEnabled { get; private set; }

    void Awake()
    {
        targetFrame = GameObject.Find("TargetFrame")?.GetComponent<Image>();
        enemyNameText = GameObject.Find("EnemyNameText")?.GetComponent<TMP_Text>();
        enemyHealthBar = GameObject.Find("EnemyHealthBar")?.GetComponent<Slider>();
    }

    void Start()
    {
        HideUI();
    }

    void Update()
    {
        HandleTargeting();
        HandleTabTargeting();
        HandleTargetClearing();
    }

    void HandleTargeting()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, tabSearchRadius, enemyLayer))
        {
            Transform hitTarget = hit.transform;
            Enemy enemy = hitTarget.GetComponent<Enemy>();

            if (hitTarget != lastHoveredTarget && currentTarget != hitTarget)
            {
                if (lastHoveredTarget != null)
                {
                    lastHoveredTarget.GetComponent<Enemy>()?.HideUI();
                }

                enemy?.ShowUI();
                lastHoveredTarget = hitTarget;
            }

            if (Input.GetMouseButtonDown(0)) // Left Click
            {
                SetTarget(hitTarget, false);
                enemy?.ShowUI();
            }
            else if (Input.GetMouseButtonDown(1)) // Right Click
            {
                SetTarget(hitTarget, true);
                enemy?.ShowUI();
            }
        }
        else if (lastHoveredTarget != null && currentTarget != lastHoveredTarget)
        {
            lastHoveredTarget.GetComponent<Enemy>()?.HideUI();
            lastHoveredTarget = null;
        }
    }

    void HandleTabTargeting()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Transform closestEnemy = FindClosestEnemy();
            if (closestEnemy != null)
            {
                if (currentTarget != null)
                {
                    currentTarget.GetComponent<Enemy>()?.HideUI();
                }
                
                SetTarget(closestEnemy, false);
                closestEnemy.GetComponent<Enemy>()?.ShowUI();
            }
        }
    }

    void HandleTargetClearing()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && currentTarget != null)
        {
            currentTarget.GetComponent<Enemy>()?.HideUI();
            ClearTarget();
        }
    }

    Transform FindClosestEnemy()
    {
        Collider[] hitColliders = Physics.OverlapSphere(playerTransform.position, tabSearchRadius, enemyLayer);
        Transform closestTarget = null;
        float closestDistance = float.MaxValue;

        foreach (Collider col in hitColliders)
        {
            float distance = Vector3.Distance(playerTransform.position, col.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = col.transform;
            }
        }

        return closestTarget;
    }

    void SetTarget(Transform target, bool enableAutoAttack)
    {
        if (currentTarget != target)
        {
            if (currentTarget != null)
            {
                currentTarget.GetComponent<Enemy>()?.HideUI();
            }

            currentTarget = target;
            autoAttackEnabled = enableAutoAttack;

            if (autoAttackEnabled)
            {
                Debug.Log("Auto Attack Enabled");
            }
            else
            {
                Debug.Log("Auto Attack Disabled");
            }

            ShowUI();
            target.GetComponent<Enemy>()?.ShowUI();
        }
    }

    void ShowTargetUI(Transform target)
    {
        Enemy enemy = target.GetComponent<Enemy>();
        if (enemy != null)
        {
            if (targetFrame) targetFrame.enabled = true;
            if (enemyNameText) enemyNameText.text = enemy.enemyName;
            if (enemyHealthBar)
            {
                enemyHealthBar.maxValue = enemy.maxHealth;
                enemyHealthBar.value = enemy.currentHealth;
            }

            if (targetFrame) targetFrame.transform.position = target.position;
        }
    }

    void ShowUI()
    {
        if (targetFrame) targetFrame.enabled = true;
        if (enemyNameText) enemyNameText.enabled = true;
        if (enemyHealthBar) enemyHealthBar.gameObject.SetActive(true);
    }

    void HideUI()
    {
        if (targetFrame) targetFrame.enabled = false;
        if (enemyNameText) enemyNameText.enabled = false;
        if (enemyHealthBar) enemyHealthBar.gameObject.SetActive(false);
    }

    public void ClearTarget()
    {
        if (currentTarget != null)
        {
            currentTarget.GetComponent<Enemy>()?.HideUI();
        }
        currentTarget = null;
        autoAttackEnabled = false;
        HideUI();
    }
}*/