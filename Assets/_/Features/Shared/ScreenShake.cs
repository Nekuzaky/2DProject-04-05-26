using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    #region Singleton
    public static ScreenShake Instance { get; private set; }
    #endregion

    #region State
    private Vector3      _basePosition;
    private float        _magnitude;
    private float        _duration;
    private float        _timer;
    private bool         _isShaking;
    private EntityHealth _playerHealth;
    #endregion

    #region Lifecycle
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        _basePosition = transform.localPosition;
    }

    private void Start()
    {
        PlayerController player = FindAnyObjectByType<PlayerController>();
        if (player != null && player.TryGetComponent(out _playerHealth))
            _playerHealth.OnDamageTaken.AddListener(ShakeLight);

        if (UpdateManager.Instance != null)
            UpdateManager.Instance.OnUpdate += OnUpdateTick;
    }
    #endregion

    #region Shake Control
    public void Shake(float duration, float magnitude)
    {
        _duration  = duration;
        _magnitude = magnitude;
        _timer     = duration;
        _isShaking = true;
    }

    public void ShakeLight() => Shake(0.15f, 0.10f);
    public void ShakeHeavy() => Shake(0.30f, 0.30f);
    #endregion

    #region Update
    private void OnUpdateTick()
    {
        if (!_isShaking) return;

        _timer -= Time.deltaTime;

        if (_timer > 0f)
        {
            float strength = (_timer / _duration) * _magnitude;
            Vector2 offset = Random.insideUnitCircle * strength;
            transform.localPosition = _basePosition + new Vector3(offset.x, offset.y, 0f);
        }
        else
        {
            transform.localPosition = _basePosition;
            _isShaking = false;
        }
    }
    #endregion

    #region Cleanup
    private void OnDestroy()
    {
        if (_playerHealth != null)
            _playerHealth.OnDamageTaken.RemoveListener(ShakeLight);

        if (UpdateManager.Instance != null)
            UpdateManager.Instance.OnUpdate -= OnUpdateTick;
    }
    #endregion
}
