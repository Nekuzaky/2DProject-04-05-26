using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    #region Inspector Settings
    [Header("<color=red><size=15>References</size></b></color>")]
    [SerializeField] private Slider _healthSlider; 
    [SerializeField] private EntityHealth _entityHealth;
    #endregion

    #region Lifecycle
    private void Start()
    {
        if (_entityHealth == null)
        {
            _entityHealth = FindAnyObjectByType<PlayerController>()?.GetComponent<EntityHealth>();
        }

        if (_healthSlider == null)
        {
            _healthSlider = GetComponent<Slider>();
        }

        if (_entityHealth != null)
        {
            _entityHealth.OnHealthChanged.AddListener(UpdateHealthBar);
            InitializeHealthBar();
        }
    }

    private void OnDestroy()
    {
        if (_entityHealth != null)
        {
            _entityHealth.OnHealthChanged.RemoveListener(UpdateHealthBar);
        }
    }
    #endregion

    #region UI Updates
    private void InitializeHealthBar()
    {
        if (_healthSlider == null) return;

        _healthSlider.maxValue = _entityHealth.GetMaxHealth();
        _healthSlider.value = _entityHealth.GetCurrentHealth();
    }

    private void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        if (_healthSlider == null) return;

        _healthSlider.maxValue = maxHealth;
        _healthSlider.value = currentHealth;
    }
    #endregion
}
