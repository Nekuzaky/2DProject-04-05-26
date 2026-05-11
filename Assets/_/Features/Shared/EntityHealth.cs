using UnityEngine;
using UnityEngine.Events;

public class EntityHealth : MonoBehaviour
{
    #region Inspector Settings
    [Header("<color=red><b><size=15>Stats</size></b></color>")]
    [SerializeField] private int _maxHealth = 100;

    [Header("<color=magenta><b><size=15>Effects</size></b></color>")]
    [SerializeField] private GameObject _deathEffect;
    #endregion

    #region Events
    [Header("Events")]
    public UnityEvent<int, int> OnHealthChanged;
    public UnityEvent OnDamageTaken;
    public UnityEvent OnDeath;
    #endregion

    #region State
    private int _currentHealth;
    private PlayerStats _playerStats;
    #endregion

    #region Lifecycle
    private void Awake()
    {
        _currentHealth = _maxHealth;
        _playerStats = GetComponent<PlayerStats>();
    }

    private void OnEnable()
    {
        if (_currentHealth <= 0)
            _currentHealth = _maxHealth;
    }
    #endregion

    #region Damage & Healing
    public void TakeDamage(int amount)
    {
        if (_playerStats != null)
            amount = Mathf.RoundToInt(amount * _playerStats.DamageMultiplier);
        _currentHealth = Mathf.Clamp(_currentHealth - amount, 0, _maxHealth);
        OnHealthChanged.Invoke(_currentHealth, _maxHealth);
        OnDamageTaken.Invoke();

        if (_currentHealth == 0)
            Die();
    }

    public void Heal(int amount)
    {
        _currentHealth = Mathf.Clamp(_currentHealth + amount, 0, _maxHealth);
        OnHealthChanged.Invoke(_currentHealth, _maxHealth);
    }

    public void ResetHealth()
    {
        _currentHealth = _maxHealth;
        OnHealthChanged.Invoke(_currentHealth, _maxHealth);
    }
    #endregion

    #region Death
    private void Die()
    {
        if (_deathEffect != null)
            Instantiate(_deathEffect, transform.position, Quaternion.identity);

        OnDeath.Invoke();

        if (gameObject.CompareTag("Player"))
        {
            GameLogger.Log("<color=red><b>EntityHealth:</b></color> Player has died.");
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
        GameLogger.Log("<color=red><b>EntityHealth:</b></color> Entity has died: " + gameObject.name);
    }
    #endregion

    #region Properties
    public int GetCurrentHealth() => _currentHealth;
    public int GetMaxHealth() => _maxHealth;
    #endregion
}
