using UnityEngine;

public class EnemyShooterAI : EnemyAI
{
    #region Inspector Settings
    [Header("<color=cyan><b><size=15>Shooter Settings</size></b></color>")]
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private Transform  _firePoint;
    [SerializeField] private float      _shootRange = 6f;
    #endregion

    #region State
    private float _shootRangeSqr;
    #endregion

    #region Lifecycle
    protected override void Awake()
    {
        base.Awake();
        _shootRangeSqr = _shootRange * _shootRange;
    }
    #endregion

    #region AI Logic
    // distSqr is squared distance — compare against pre-squared ranges (no Sqrt)
    protected override void UpdateState(float distSqr)
    {
        if (distSqr <= _shootRangeSqr)
        {
            CurrentState = EnemyState.Attack;
        }
        else if (distSqr <= _detectionRangeSqr)
        {
            CurrentState = EnemyState.Chase;
        }
        else
        {
            CurrentState = EnemyState.Idle;
        }
    }
    #endregion

    #region Attacking
    protected override void Attack()
    {
        if (Time.time >= _nextAttackTime)
        {
            _nextAttackTime = Time.time + _attackCooldown;
            Shoot();
        }
    }

    private void Shoot()
    {
        if (_projectilePrefab == null || _target == null) return;

        Transform origin    = _firePoint != null ? _firePoint : transform;
        Vector2   direction = ((Vector2)(_target.position - origin.position)).normalized;

        GameObject projectileObj = ProjectilePool.Instance != null
            ? ProjectilePool.Instance.Get(_projectilePrefab, origin.position, Quaternion.identity)
            : Instantiate(_projectilePrefab, origin.position, Quaternion.identity);

        if (projectileObj.TryGetComponent(out Projectile projectile))
        {
            projectile.SetDamage(_attackDamage);
            projectile.Launch(direction);
        }
    }
    #endregion

    #region Debug
    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _shootRange);
    }
    #endregion
}
