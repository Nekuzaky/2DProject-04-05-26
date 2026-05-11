using UnityEngine;

// Pool-safe projectile. Never calls Destroy() — always returns itself to ProjectilePool.
// Lifecycle: OnEnable resets state → Launch() sets velocity → timer or collision → ReturnToPool().
[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    #region Inspector Settings
    [Header("<color=cyan><b><size=15>Settings</size></b></color>")]
    [SerializeField] private int    _damage    = 10;
    [SerializeField] private float  _lifetime  = 5f;
    [SerializeField] private float  _speed     = 10f;
    [SerializeField] private string _targetTag = "Enemy";
    #endregion

    #region State
    private Rigidbody2D _rb;
    private float       _elapsed;
    private bool        _returned;      // guard against double-return on same frame
    private GameObject  _sourcePrefab;
    private bool        _usingUpdateManager;
    #endregion

    #region Lifecycle
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        _elapsed  = 0f;
        _returned = false;

        if (UpdateManager.Instance != null)
        {
            UpdateManager.Instance.OnUpdate += OnUpdateTick;
            _usingUpdateManager = true;
        }
        else
        {
            _usingUpdateManager = false;
        }
    }

    private void OnDisable()
    {
        if (_usingUpdateManager && UpdateManager.Instance != null)
            UpdateManager.Instance.OnUpdate -= OnUpdateTick;

        _usingUpdateManager = false;

        // Clear velocity so the projectile is inert while waiting in the pool
        if (_rb != null) _rb.linearVelocity = Vector2.zero;
    }

    private void Update()
    {
        if (!_usingUpdateManager) OnUpdateTick();
    }
    #endregion

    #region Tick
    private void OnUpdateTick()
    {
        _elapsed += Time.deltaTime;
        if (_elapsed >= _lifetime)
            ReturnToPool();
    }
    #endregion

    #region Public API
    public void SetDamage(int damage)              => _damage       = damage;
    public void SetSourcePrefab(GameObject prefab) => _sourcePrefab = prefab;

    public void Launch(Vector2 direction)
    {
        if (_rb != null)
            _rb.linearVelocity = direction * _speed;
    }
    #endregion

    #region Collision
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Projectile _)) return;
        if (!collision.CompareTag(_targetTag)) return;

        if (collision.TryGetComponent(out EntityHealth health))
            health.TakeDamage(_damage);

        ReturnToPool();
    }
    #endregion

    #region Pool
    private void ReturnToPool()
    {
        if (_returned) return;
        _returned = true;

        if (ProjectilePool.Instance != null)
            ProjectilePool.Instance.Return(gameObject, _sourcePrefab);
        else
            Destroy(gameObject);
    }
    #endregion
}
