using UnityEngine;

public class EnemyShooterAI : EnemyAI
{
    [Header("<color=cyan><b><size=15>Shooter Settings</size></b></color>")]
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private float _shootRange = 6f;

    protected override void UpdateState(float distanceToTarget)
    {
        if (distanceToTarget <= _attackRange)
        {
            CurrentState = EnemyState.Attack;
        }
        else if (distanceToTarget <= _shootRange)
        {
            CurrentState = EnemyState.Attack;
        }
        else if (distanceToTarget <= _detectionRange)
        {
            CurrentState = EnemyState.Chase;
        }
        else
        {
            CurrentState = EnemyState.Idle;
        }
    }

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

        Transform origin = _firePoint != null ? _firePoint : transform;
        Vector2 direction = (_target.position - origin.position).normalized;

        GameObject projectileObj = Instantiate(_projectilePrefab, origin.position, Quaternion.identity);

        if (projectileObj.TryGetComponent(out Projectile projectile))
        {
            projectile.SetDamage(_attackDamage);
            projectile.Launch(direction);
        }

        Debug.Log("<color=red><b>EnemyShooterAI:</b></color> Shot at " + _target.name);
    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _shootRange);
    }
}
