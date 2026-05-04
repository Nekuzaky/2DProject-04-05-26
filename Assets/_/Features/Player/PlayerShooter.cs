using UnityEngine;
using UnityEngine.Events;

public class PlayerShooter : MonoBehaviour
{
    [Header("<color=cyan><b><size=15>References</size></b></color>")]
    [SerializeField] private Transform _firePoint;
    [SerializeField] private GameObject _projectilePrefab;

    [Header("<color=cyan><b><size=15>Settings</size></b></color>")]
    [SerializeField] private float _fireRate = 0.2f;
    [SerializeField] private int _maxAmmo = 100;
    [SerializeField] private int _currentAmmo = 100;
    [SerializeField] private float _reloadTime = 2f;

    private float _nextFireTime = 0f;
    private bool _isReloading = false;

    public UnityEvent<int, int> OnAmmoChanged;

    private void Start()
    {
        _currentAmmo = _maxAmmo;
        OnAmmoChanged?.Invoke(_currentAmmo, _maxAmmo);
    }

    private void Update()
    {
        bool reloadPressed = Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.JoystickButton3);
        if (reloadPressed && !_isReloading && _currentAmmo < _maxAmmo)
            StartReload();

        if (_isReloading)
            return;

        bool fire = Input.GetMouseButton(0) || Input.GetAxisRaw("RightTrigger") > 0.1f;
        if (fire && Time.time > _nextFireTime)
        {
            if (_currentAmmo > 0)
            {
                _nextFireTime = Time.time + _fireRate;
                Shoot();
            }
            else
            {
                StartReload();
            }
        }
    }

    private void StartReload()
    {
        if (_isReloading) return;
        
        _isReloading = true;
        Invoke(nameof(FinishReload), _reloadTime);
        Debug.Log("<color=yellow><b>PlayerShooter:</b></color> Reloading...");
    }

    private void FinishReload()
    {
        _currentAmmo = _maxAmmo;
        _isReloading = false;
        OnAmmoChanged?.Invoke(_currentAmmo, _maxAmmo);
        Debug.Log("<color=green><b>PlayerShooter:</b></color> Reloaded");

    }

    private void Shoot()
    {
        _currentAmmo--;
        OnAmmoChanged?.Invoke(_currentAmmo, _maxAmmo);

        GameObject bullet = Instantiate(_projectilePrefab, _firePoint.position, _firePoint.rotation);

        if (bullet.TryGetComponent(out Projectile projectile))
        {
            projectile.Launch(_firePoint.right);
        }
        else if (bullet.TryGetComponent(out Rigidbody2D rb))
        {
            rb.linearVelocity = _firePoint.right * 10f;
        }
    }
}