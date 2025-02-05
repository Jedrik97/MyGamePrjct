using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TargetingSystem : MonoBehaviour
{
    [Header("Настройка поиска цели")]
    public LayerMask enemyLayer;        
    public float tabSearchRadius = 15f; 

    [Header("UI элементы")]
    public Image targetFrame;          
    public TMP_Text enemyNameText;     
    public Slider enemyHealthBar;      

    [Header("Ссылка на игрока (для поиска по Tab)")]
    public Transform playerTransform;
    
    public Transform currentTarget { get; private set; }
    public bool autoAttackEnabled { get; private set; }

    void Start()
    {
        HideUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ClearTarget();
        }

        HandleMouseHover();
        HandleLeftClickSelection();
        HandleRightClickSelection();
        HandleTabSelection();
    }
    
    void HandleMouseHover()
    {
        if (currentTarget != null)
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f, enemyLayer))
        {
            Enemy enemy = hit.collider.GetComponent<Enemy>();
            if (enemy == null)
                enemy = hit.collider.GetComponentInParent<Enemy>();

            if (enemy != null)
            {
                targetFrame.transform.position = enemy.transform.position;
                targetFrame.enabled = true;
                return;
            }
        }
        targetFrame.enabled = false;
    }
    
    void HandleLeftClickSelection()
    {
        if (Input.GetMouseButtonDown(0)) 
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100f, enemyLayer))
            {
                Enemy enemy = hit.collider.GetComponent<Enemy>();
                if (enemy == null)
                    enemy = hit.collider.GetComponentInParent<Enemy>();

                if (enemy != null)
                {
                    SetTarget(enemy, false);
                }
            }
        }
    }
    
    void HandleRightClickSelection()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100f, enemyLayer))
            {
                Enemy enemy = hit.collider.GetComponent<Enemy>();
                if (enemy == null)
                    enemy = hit.collider.GetComponentInParent<Enemy>();

                if (enemy != null)
                {
                    SetTarget(enemy, true);
                }
            }
        }
    }
    
    void HandleTabSelection()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Collider[] hits = Physics.OverlapSphere(playerTransform.position, tabSearchRadius, enemyLayer);
            Enemy nearestEnemy = null;
            float nearestDistance = Mathf.Infinity;

            foreach (Collider col in hits)
            {
                Enemy enemy = col.GetComponent<Enemy>();
                if (enemy == null)
                    enemy = col.GetComponentInParent<Enemy>();

                if (enemy != null && enemy.currentHealth > 0)
                {
                    float dist = Vector3.Distance(playerTransform.position, enemy.transform.position);
                    if (dist < nearestDistance)
                    {
                        nearestDistance = dist;
                        nearestEnemy = enemy;
                    }
                }
            }

            if (nearestEnemy != null)
            {
                SetTarget(nearestEnemy, false);
            }
        }
    }
    
    public void SetTarget(Enemy enemy, bool autoAttack)
    {
        currentTarget = enemy.transform;
        autoAttackEnabled = autoAttack;

        enemyNameText.text = enemy.enemyName;
        enemyHealthBar.maxValue = enemy.maxHealth;
        enemyHealthBar.value = enemy.currentHealth;
        
        ShowUI();
    }
    
    public void ClearTarget()
    {
        currentTarget = null;
        autoAttackEnabled = false;
        HideUI();
    }
    
    void ShowUI()
    {
        targetFrame.enabled = true;
        enemyNameText.enabled = true;
        enemyHealthBar.gameObject.SetActive(true);
    }
    
    void HideUI()
    {
        targetFrame.enabled = false;
        enemyNameText.enabled = false;
        enemyHealthBar.gameObject.SetActive(false);
    }
}
