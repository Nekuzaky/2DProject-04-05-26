using UnityEngine;
using UnityEngine.Events;

public class EntityHealth : MonoBehaviour
    
{
    [Header("<color=red><b><size=15>Stats</size></b></color>")]
    [SerializeField] private int _maxHealth = 100;
    private int _currentHealth;

    [Header("Events")]
    public UnityEvent<int, int> OnHealthChanged; // Passes current health and max health as parameters
    public UnityEvent OnDeath;

    private void Awake()
    {
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
        OnDeath.Invoke();
        Destroy(gameObject);
        if (gameObject.CompareTag("Player"))
        {
            Debug.Log("<color=red><b>EntityHealth:</b></color> Player has died.");
        }
        else
        {
            Debug.Log("<color=red><b>EntityHealth:</b></color> Entity has died: " + gameObject.name); // Log the name of the entity that died
        }

    }
    
    public int GetCurrentHealth() => _currentHealth;
    public int GetMaxHealth() => _maxHealth;
}