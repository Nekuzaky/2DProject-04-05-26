using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float _moveSpeed = 3f;
    [SerializeField] private float _detectionRange = 8f;
    [SerializeField] private float _attackRange = 1.5f;
    [SerializeField] private int _attackDamage = 10;
    [SerializeField] private float _attackCooldown = 1f;

    [Header("Target")]
    [SerializeField] private string _targetTag = "Player";

    private Transform _target;
    private Rigidbody2D _rb;
    private EntityHealth _targetHealth;
    private float _nextAttackTime = 0f;

    private enum EnemyState { Idle, Chase, Attack }
    private EnemyState _currentState = EnemyState.Idle;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        
        if (_rb != null)
        {
            _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            _rb.gravityScale = 0f;
        }
        else
        {
            Debug.LogError("[EnemyAI] Rigidbody2D manquant sur " + gameObject.name);
        }
    }

    private void Start()
    {
        FindTarget();
    }

    private void Update()
    {
        if (_target == null)
        {
            FindTarget();
            return;
        }

        float distanceToTarget = Vector2.Distance(transform.position, _target.position);

        UpdateState(distanceToTarget);

        if (_currentState == EnemyState.Attack)
        {
            Attack();
        }
    }

    private void FixedUpdate()
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

    private void UpdateState(float distanceToTarget)
    {
        if (distanceToTarget <= _attackRange)
        {
            _currentState = EnemyState.Attack;
        }
        else if (distanceToTarget <= _detectionRange)
        {
            _currentState = EnemyState.Chase;
        }
        else
        {
            _currentState = EnemyState.Idle;
        }
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

    private void Attack()
    {
        if (Time.time >= _nextAttackTime)
        {
            _nextAttackTime = Time.time + _attackCooldown;

            if (_targetHealth != null)
            {
                _targetHealth.TakeDamage(_attackDamage);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRange);
    }
}
