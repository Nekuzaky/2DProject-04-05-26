using System;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PickupItemDrugs : MonoBehaviour
{
    #region Nested Types
    public enum EffectTarget { Heal, MoveSpeed, FireRate, DamageReduction }

    [Serializable]
    public class PickupEffect
    {
        [Tooltip("Type of effect applied by this pickup.")]
        public EffectTarget Target;

        [Tooltip("Heal: Pv restored. Buffs: Multiplier (e.g., 0.2 for +20%).")]
        public float Value = 1f;

        [Tooltip("Delay before the effect starts in seconds. Ignored for Heal.")]
        public float Delay = 0f;

        [Tooltip("Duration of the effect in seconds. Ignored for Heal.")]
        public float Duration = 10f;
    }
    #endregion

    #region Inspector Settings
    [Header("<color=yellow><b><size=15>Pickup Effects</size></b></color>")]
    [SerializeField] private PickupEffect[] _effects;
    #endregion

    #region Lifecycle
    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }
    #endregion

    #region Trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        other.TryGetComponent(out EntityHealth health);
        other.TryGetComponent(out PlayerStats stats);

        if (_effects == null || _effects.Length == 0)
        {
            Destroy(gameObject);
            return;
        }

        foreach (PickupEffect effect in _effects)
        {
            if (effect.Target == EffectTarget.Heal)
            {
                if (health != null)
                    health.Heal(Mathf.RoundToInt(effect.Value));
            }
            else
            {
                if (stats == null) continue;

                PlayerStats.EffectType type = effect.Target switch
                {
                    EffectTarget.MoveSpeed      => PlayerStats.EffectType.MoveSpeed,
                    EffectTarget.FireRate        => PlayerStats.EffectType.FireRate,
                    EffectTarget.DamageReduction => PlayerStats.EffectType.DamageReduction,
                    _                            => PlayerStats.EffectType.MoveSpeed
                };

                stats.AddEffect(type, effect.Value, effect.Delay, effect.Duration);
            }
        }

        Destroy(gameObject);
    }
    #endregion
}
