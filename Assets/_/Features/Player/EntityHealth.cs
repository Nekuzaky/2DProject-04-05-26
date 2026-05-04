using UnityEngine;
using UnityEngine.Events;

public class EntityHealth : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    [Header("Events")]
    public UnityEvent<int, int> onHealthChanged;
    public UnityEvent onDeath;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth = Mathf.Clamp(currentHealth - amount, 0, maxHealth);
        onHealthChanged.Invoke(currentHealth, maxHealth);

        if (currentHealth == 0)
            Die();
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        onHealthChanged.Invoke(currentHealth, maxHealth);
    }

   
    private void Die()
    {
        onDeath.Invoke();
        Destroy(gameObject);
    }
    public int GetCurrentHealth() => currentHealth;
    public int GetMaxHealth() => maxHealth;
}