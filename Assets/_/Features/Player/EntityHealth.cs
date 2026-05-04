using UnityEngine;
using UnityEngine.Events;

public class EntityHealth : MonoBehaviour
    
{
    [Header("Stats")]
    [SerializeField] private int _maxHealth = 100;
    private int _currentHealth;

    [Header("Events")]
    public UnityEvent<int, int> OnHealthChanged;
    public UnityEvent OnDeath;

    private void Awake()
    {
        _currentHealth = _maxHealth;
    }

    public void TakeDamage(int amount)
    {
        _currentHealth = Mathf.Clamp(_currentHealth - amount, 0, _maxHealth);
        OnHealthChanged.Invoke(_currentHealth, _maxHealth);

        if (_currentHealth == 0)
            Die();
    }

    public void Heal(int amount)
    {
        _currentHealth = Mathf.Clamp(_currentHealth + amount, 0, _maxHealth);
        OnHealthChanged.Invoke(_currentHealth, _maxHealth);
    }

   
    private void Die()
    {
        OnDeath.Invoke();
        Destroy(gameObject);
        if (gameObject.CompareTag("Player"))
        {
            Debug.Log("<color=red><b>EntityHealth:</b></color> Player has died.");
        }
        else
        {
            Debug.Log("<color=red><b>EntityHealth:</b></color> Entity has died: " + gameObject.name);
        }

    }
    
    public int GetCurrentHealth() => _currentHealth;
    public int GetMaxHealth() => _maxHealth;
}