using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(EntityHealth))]
public class EnemyAI : MonoBehaviour
{
    [Header("<color=cyan><b><size=15>Stats</size></b></color>")]
    [SerializeField] protected float _moveSpeed = 3f;
    [SerializeField] protected float _detectionRange = 12f;
    [SerializeField] protected float _attackRange = 1.5f;
    [SerializeField] protected int _attackDamage = 10;
    [SerializeField] protected float _attackCooldown = 1f;

    [Header("Target")]
    [SerializeField] private string _targetTag = "Player";

    protected Transform _target;
    protected Rigidbody2D _rb;
    protected EntityHealth _targetHealth;
    protected float _nextAttackTime = 0f;

    protected enum EnemyState { Idle, Chase, Attack }
    private EnemyState _currentState = EnemyState.Idle;
    protected EnemyState CurrentState { get => _currentState; set => _currentState = value; }

    protected virtual void Awake()
    {
        gameObject.tag = "Enemy";

        _rb = GetComponent<Rigidbody2D>();
        _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        _rb.gravityScale = 0f;
    }

    protected virtual void Start()
    {
        FindTarget();

        if (UpdateManager.Instance != null)
        {
            UpdateManager.Instance.OnUpdate      += OnUpdateTick;
            UpdateManager.Instance.OnFixedUpdate += OnFixedUpdateTick;
        }
    }

    protected virtual void OnUpdateTick()
    {
        if (_target == null)
        {
            FindTarget();
            return;
        }

        float distanceToTarget = Vector2.Distance(transform.position, _target.position);

        UpdateState(distanceToTarget);

        if (_currentState == EnemyState.Attack)
            Attack();
    }

    private void OnFixedUpdateTick()
    {
        Move();
    }

    private void FindTarget()
    {
        GameObject targetObject = GameObject.FindWithTag(_targetTag);

        if (targetObject != null)
        {
            _target = targetObject.transform;
            _targetHealth = targetObject.GetComponent<EntityHealth>();
        }
    }

    protected virtual void UpdateState(float distanceToTarget)
    {
        if (distanceToTarget <= _attackRange)
            _currentState = EnemyState.Attack;
        else if (distanceToTarget <= _detectionRange)
            _currentState = EnemyState.Chase;
        else
            _currentState = EnemyState.Idle;
    }

    private void Move()
    {
        if (_rb == null) return;

        if (_currentState == EnemyState.Chase && _target != null)
        {
            Vector2 direction = (_target.position - transform.position).normalized;
            _rb.linearVelocity = direction * _moveSpeed;

            Debug.DrawRay(transform.position, direction * 2f, Color.green);
        }
        else
        {
            _rb.linearVelocity = Vector2.zero;
        }
    }

    protected virtual void Attack() { }

    protected virtual void OnDestroy()
    {
        if (UpdateManager.Instance != null)
        {
            UpdateManager.Instance.OnUpdate      -= OnUpdateTick;
            UpdateManager.Instance.OnFixedUpdate -= OnFixedUpdateTick;
        }
    }

    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRange);
    }
}
