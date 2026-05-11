using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(EntityHealth))]
public class EnemyAI : MonoBehaviour
{
    #region Inspector Settings
    [Header("<color=cyan><b><size=15>Stats</size></b></color>")]
    [SerializeField] protected float _moveSpeed       = 3f;
    [SerializeField] protected float _detectionRange  = 12f;
    [SerializeField] protected float _attackRange     = 1.5f;
    [SerializeField] protected int   _attackDamage    = 10;
    [SerializeField] protected float _attackCooldown  = 1f;
    #endregion

    #region Enums
    protected enum EnemyState { Idle, Chase, Attack }
    #endregion

    #region State
    protected Transform    _target;
    protected Rigidbody2D  _rb;
    protected EntityHealth _targetHealth;
    protected float        _nextAttackTime    = 0f;
    // Pre-squared ranges — avoids Mathf.Sqrt every frame per enemy
    protected float        _detectionRangeSqr;
    protected float        _attackRangeSqr;
    private   EnemyState   _currentState      = EnemyState.Idle;
    #endregion

    #region Properties
    protected EnemyState CurrentState { get => _currentState; set => _currentState = value; }
    #endregion

    #region Lifecycle
    protected virtual void Awake()
    {
        gameObject.tag = "Enemy";

        _rb = GetComponent<Rigidbody2D>();
        _rb.constraints  = RigidbodyConstraints2D.FreezeRotation;
        _rb.gravityScale = 0f;

        _detectionRangeSqr = _detectionRange * _detectionRange;
        _attackRangeSqr    = _attackRange    * _attackRange;
    }

    protected virtual void OnEnable()
    {
        FindTarget();

        if (UpdateManager.Instance != null)
        {
            UpdateManager.Instance.OnUpdate      += OnUpdateTick;
            UpdateManager.Instance.OnFixedUpdate += OnFixedUpdateTick;
        }
    }
    #endregion

    #region AI Logic
    protected virtual void OnUpdateTick()
    {
        if (_target == null)
        {
            FindTarget();
            return;
        }

        // sqrMagnitude avoids Sqrt — compare squared distances to squared radii
        Vector2 toTarget = _target.position - transform.position;
        float   distSqr  = toTarget.sqrMagnitude;

        UpdateState(distSqr);

        if (_currentState == EnemyState.Attack)
            Attack();
    }

    private void FindTarget()
    {
        if (EnemyManager.Instance == null) return;

        _target       = EnemyManager.Instance.PlayerTarget;
        _targetHealth = EnemyManager.Instance.PlayerHealth;
    }

    // Parameter is squared distance — compare against _detectionRangeSqr / _attackRangeSqr
    protected virtual void UpdateState(float distSqr)
    {
        if (distSqr <= _attackRangeSqr)
            _currentState = EnemyState.Attack;
        else if (distSqr <= _detectionRangeSqr)
            _currentState = EnemyState.Chase;
        else
            _currentState = EnemyState.Idle;
    }
    #endregion

    #region Movement
    private void OnFixedUpdateTick()
    {
        Move();
    }

    private void Move()
    {
        if (_rb == null) return;

        if (_currentState == EnemyState.Chase && _target != null)
        {
            Vector2 direction = ((Vector2)(_target.position - transform.position)).normalized;
            _rb.linearVelocity = direction * _moveSpeed;

#if UNITY_EDITOR
            Debug.DrawRay(transform.position, direction * 2f, Color.green);
#endif
        }
        else
        {
            _rb.linearVelocity = Vector2.zero;
        }
    }
    #endregion

    #region Attacking
    protected virtual void Attack() { }
    #endregion

    #region Debug
    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRange);
    }
    #endregion

    #region Cleanup
    protected virtual void OnDisable()
    {
        if (UpdateManager.Instance != null)
        {
            UpdateManager.Instance.OnUpdate      -= OnUpdateTick;
            UpdateManager.Instance.OnFixedUpdate -= OnFixedUpdateTick;
        }
    }

    protected virtual void OnDestroy()
    {
        OnDisable();
    }
    #endregion
}
