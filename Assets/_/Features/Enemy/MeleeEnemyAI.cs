using UnityEngine;

public class MeleeEnemyAI : EnemyAI
{
    #region Attacking
    protected override void Attack()
    {
        if (Time.time < _nextAttackTime) return;

        _nextAttackTime = Time.time + _attackCooldown;

        if (_targetHealth != null)
        {
            _targetHealth.TakeDamage(_attackDamage);
            GameLogger.Log("<color=red><b>MeleeEnemyAI:</b></color> Attacked " + _target.name + " for " + _attackDamage + " damage.");
        }
    }
    #endregion
}
