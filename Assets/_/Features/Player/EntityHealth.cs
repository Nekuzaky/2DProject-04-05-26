using UnityEngine;
using UnityEngine.Events;

public class EntityHealth : MonoBehaviour
    
{
    [Header("<color=red><b><size=15>Stats</size></b></color>")]
    [SerializeField] private int _maxHealth = 100;
    private int _currentHealth;

    [Header("<color=magenta><b><size=15>Effects</size></b></color>")]
    [SerializeField] private GameObject _deathEffect;

    [Header("Events")]
    public UnityEvent<int, int> OnHealthChanged;
    public UnityEvent OnDeath;

    private void Awake()
    {
        _currentHealth = _maxHealth;
    }

    private void OnEnable()
    {
        if (_currentHealth <= 0)
            _currentHealth = _maxHealth;
    }

    public void TakeDamage(int amount)
    {
        _currentHealth = Mathf.Clamp(_currentHealth - amount, 0, _maxHealth); // Ensure health doesn't go below 0 or above max
        OnHealthChanged.Invoke(_currentHealth, _maxHealth);

        if (_currentHealth == 0)
            Die();
    }

    public void Heal(int amount)
    {
        _currentHealth = Mathf.Clamp(_currentHealth + amount, 0, _maxHealth); // Ensure health doesn't go below 0 or above max
        OnHealthChanged.Invoke(_currentHealth, _maxHealth);
    }

   
    private void Die()
    {
        if (_deathEffect != null)
            Instantiate(_deathEffect, transform.position, Quaternion.identity);

        OnDeath.Invoke();

        if (gameObject.CompareTag("Player"))
        {
            Debug.Log("<color=red><b>EntityHealth:</b></color> Player has died.");
            Destroy(gameObject);
            return;
        }

        EnemyPoolMember poolMember = GetComponent<EnemyPoolMember>();
        if (poolMember != null)
        {
            poolMember.ReturnToPool();
            return;
        }

        Destroy(gameObject);
        Debug.Log("<color=red><b>EntityHealth:</b></color> Entity has died: " + gameObject.name); // Log the name of the entity that died

    }

    public void ResetHealth()
    {
        _currentHealth = _maxHealth;
        OnHealthChanged.Invoke(_currentHealth, _maxHealth);
    }
    
    public int GetCurrentHealth() => _currentHealth;
    public int GetMaxHealth() => _maxHealth;
}