using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
public class BaseEnemyAI : MonoBehaviour
{
    [Header("<color=cyan><b><size=15>Stats</size></b></color>")]
    [SerializeField] protected float _moveSpeed = 3f;
    [SerializeField] protected float _detectionRange = 8f;
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
    protected EnemyState _currentState = EnemyState.Idle;

    protected virtual void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        
        if (_rb != null)
        {
            _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            _rb.gravityScale = 0f;
        }
        else
        {
            Debug.LogError("[EnemyAI] Rigidbody Missing " + gameObject.name);
        }
    }

    protected virtual void Start()
    {
        FindTarget();
    }

    protected virtual void Update()
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

    protected virtual void FixedUpdate()
    {
        Move();
    }

    protected void FindTarget()
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

    protected virtual void Move()
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

    protected virtual void Attack()
    {
        if (Time.time >= _nextAttackTime)
        {
            _nextAttackTime = Time.time + _attackCooldown;

            if (_targetHealth != null)
            {
                _targetHealth.TakeDamage(_attackDamage);
                Debug.Log("<color=red><b>EnemyAI:</b></color> Attacked " + _target.name + " for " + _attackDamage + " damage.");
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
