using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    public static ScreenShake Instance { get; private set; }

    private Vector3      _basePosition;
    private float        _magnitude;
    private float        _duration;
    private float        _timer;
    private bool         _isShaking;
    private EntityHealth _playerHealth;

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

    public void Shake(float duration, float magnitude)
    {
        _duration  = duration;
        _magnitude = magnitude;
        _timer     = duration;
        _isShaking = true;
    }

    public void ShakeLight() => Shake(0.15f, 0.10f);
    public void ShakeHeavy() => Shake(0.30f, 0.30f);

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

    private void OnDestroy()
    {
        if (_playerHealth != null)
            _playerHealth.OnDamageTaken.RemoveListener(ShakeLight);

        if (UpdateManager.Instance != null)
            UpdateManager.Instance.OnUpdate -= OnUpdateTick;
    }
}
