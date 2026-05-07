using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    #region Nested Types
    public enum EffectType { MoveSpeed, FireRate, DamageReduction }

    private struct ActiveEffect
    {
        public EffectType Type;
        public float Multiplier;
        public float StartTime;
        public float Delay;
        public float Duration;

        public readonly bool IsActive  => Time.time >= StartTime + Delay;
        public readonly bool IsExpired => Time.time >= StartTime + Delay + Duration;
    }
    #endregion

    #region State
    private readonly List<ActiveEffect> _effects = new();
    #endregion

    #region Properties
    public float MoveSpeedMultiplier  => GetCombinedMultiplier(EffectType.MoveSpeed);
    public float FireRateMultiplier   => GetCombinedMultiplier(EffectType.FireRate);
    public float DamageMultiplier     => GetCombinedMultiplier(EffectType.DamageReduction);
    #endregion

    #region Lifecycle
    private void OnEnable()
    {
        if (UpdateManager.Instance != null)
            UpdateManager.Instance.OnUpdate += OnUpdateTick;
    }

    private void OnDisable()
    {
        if (UpdateManager.Instance != null)
            UpdateManager.Instance.OnUpdate -= OnUpdateTick;
    }
    #endregion

    #region Tick
    private void OnUpdateTick()
    {
        _effects.RemoveAll(e => e.IsExpired);
    }
    #endregion

    #region Public API
    public void AddEffect(EffectType type, float multiplier, float delay, float duration)
    {
        _effects.Add(new ActiveEffect
        {
            Type       = type,
            Multiplier = multiplier,
            StartTime  = Time.time,
            Delay      = delay,
            Duration   = duration
        });
    }
    #endregion

    #region Helpers
    private float GetCombinedMultiplier(EffectType type)
    {
        float result = 1f;
        foreach (ActiveEffect e in _effects)
            if (e.Type == type && e.IsActive)
                result *= e.Multiplier;
        return result;
    }
    #endregion
}
