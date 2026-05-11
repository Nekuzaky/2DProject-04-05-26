using UnityEngine;
using UnityEngine.Events;

public class PlayerShooter : MonoBehaviour
{
    #region Inspector Settings
    [Header("<color=cyan><b><size=15>References</size></b></color>")]
    [SerializeField] private Transform   _firePoint;
    [SerializeField] private GameObject  _projectilePrefab;
    [SerializeField] private WeaponStats _weaponStats;
    #endregion

    #region State
    private int         _maxAmmo;
    private int         _currentAmmo;
    private float       _nextFireTime    = 0f;
    private bool        _isReloading     = false;
    private float       _reloadStartTime;
    private float       _reloadDuration;
    private PlayerStats _playerStats;
    // Cached once — avoids GetComponent on prefab every shot
    private bool        _prefabHasProjectile;
    private bool        _subscribedToUpdateManager;
    #endregion

    #region Events & Properties
    public float ReloadProgress => _isReloading
        ? Mathf.Clamp01((Time.time - _reloadStartTime) / _reloadDuration)
        : 0f;

    public UnityEvent<int, int>       OnAmmoChanged;
    public event System.Action<float> OnReloadStart;
    public event System.Action        OnReloadFinished;
    #endregion

    #region Lifecycle
    private void Start()
    {
        _playerStats = GetComponent<PlayerStats>();
        _maxAmmo     = _weaponStats != null ? _weaponStats.FinalAmmoCapacity : 30;
        _currentAmmo = _maxAmmo;

        // Cache whether the prefab has a Projectile component (needed by ProjectilePool)
        _prefabHasProjectile = _projectilePrefab != null
            && _projectilePrefab.TryGetComponent<Projectile>(out _);

        OnAmmoChanged?.Invoke(_currentAmmo, _maxAmmo);

        if (UpdateManager.Instance != null)
        {
            UpdateManager.Instance.OnUpdate += OnUpdateTick;
            _subscribedToUpdateManager = true;
        }
    }

    // Fallback: lazy-subscribe or tick directly if UpdateManager wasn't ready at Start.
    private void Update()
    {
        if (!_subscribedToUpdateManager)
        {
            if (UpdateManager.Instance != null)
            {
                UpdateManager.Instance.OnUpdate += OnUpdateTick;
                _subscribedToUpdateManager = true;
                return;
            }
            OnUpdateTick(); // direct fallback
        }
    }

    private void OnDestroy()
    {
        if (_subscribedToUpdateManager && UpdateManager.Instance != null)
            UpdateManager.Instance.OnUpdate -= OnUpdateTick;
    }
    #endregion

    #region Input & Firing
    private void OnUpdateTick()
    {
        bool reloadPressed = InputHandler.Instance != null
            ? InputHandler.Instance.ReloadPressed
            : Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.JoystickButton3);

        if (reloadPressed && !_isReloading && _currentAmmo < _maxAmmo)
            StartReload();

        if (_isReloading) return;

        float fireRate = _weaponStats != null ? _weaponStats.FinalFireRate : 0.2f;
        if (_playerStats != null)
            fireRate *= _playerStats.FireRateMultiplier;

        bool fire = InputHandler.Instance != null
            ? InputHandler.Instance.IsFiring
            : Input.GetMouseButton(0);

        if (fire && Time.time > _nextFireTime)
        {
            if (_currentAmmo > 0)
            {
                _nextFireTime = Time.time + fireRate;
                Shoot();
            }
            else
            {
                StartReload();
            }
        }
    }
    #endregion

    #region Reload System
    private void StartReload()
    {
        if (_isReloading) return;

        _isReloading     = true;
        _reloadDuration  = _weaponStats != null ? _weaponStats.FinalReloadTime : 2f;
        _reloadStartTime = Time.time;

        Invoke(nameof(FinishReload), _reloadDuration);
        OnReloadStart?.Invoke(_reloadDuration);
        GameLogger.Log("<color=yellow><b>PlayerShooter:</b></color> Reloading...");
    }

    private void FinishReload()
    {
        _maxAmmo     = _weaponStats != null ? _weaponStats.FinalAmmoCapacity : _maxAmmo;
        _currentAmmo = _maxAmmo;
        _isReloading = false;

        OnAmmoChanged?.Invoke(_currentAmmo, _maxAmmo);
        OnReloadFinished?.Invoke();
        GameLogger.Log("<color=green><b>PlayerShooter:</b></color> Reloaded");
    }
    #endregion

    #region Shooting
    private void Shoot()
    {
        _currentAmmo--;
        OnAmmoChanged?.Invoke(_currentAmmo, _maxAmmo);

        float   spread    = _weaponStats != null ? _weaponStats.FinalSpreadAngle : 0f;
        float   angle     = Random.Range(-spread, spread);
        Vector2 direction = Quaternion.Euler(0f, 0f, angle) * _firePoint.right;

        // Pool only works with prefabs that have a Projectile component (pool needs it to track return)
        GameObject bullet = (_prefabHasProjectile && ProjectilePool.Instance != null)
            ? ProjectilePool.Instance.Get(_projectilePrefab, _firePoint.position, _firePoint.rotation)
            : Instantiate(_projectilePrefab, _firePoint.position, _firePoint.rotation);

        if (bullet.TryGetComponent(out Projectile projectile))
        {
            projectile.SetDamage(_weaponStats != null ? _weaponStats.FinalDamage : 10);
            projectile.Launch(direction);
        }
        else if (bullet.TryGetComponent(out Rigidbody2D rb))
        {
            float speed = _weaponStats != null ? _weaponStats.ProjectileSpeed : 10f;
            rb.linearVelocity = direction * speed;
        }
    }
    #endregion
}
