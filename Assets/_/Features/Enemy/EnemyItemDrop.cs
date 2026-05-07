using System;
using UnityEngine;

[RequireComponent(typeof(EntityHealth))]
public class EnemyItemDrop : MonoBehaviour
{
    #region Nested Types
    [Serializable]
    public class DropEntry
    {
        public GameObject Prefab;

        [Range(0f, 100f)]
        public float DropChance = 30f;
    }
    #endregion

    #region Inspector Settings
    [Header("<color=yellow><b><size=15>Item Drops</size></b></color>")]
    [SerializeField] private DropEntry[] _drops;

    [SerializeField] private Vector2 _dropOffset = Vector2.zero;
    #endregion

    #region State
    private EntityHealth _health;
    #endregion

    #region Lifecycle
    private void Awake()
    {
        _health = GetComponent<EntityHealth>();
    }

    private void OnEnable()
    {
        if (_health != null)
            _health.OnDeath.AddListener(OnDeath);
    }

    private void OnDisable()
    {
        if (_health != null)
            _health.OnDeath.RemoveListener(OnDeath);
    }
    #endregion

    #region Drop Logic
    private void OnDeath()
    {
        if (_drops == null || _drops.Length == 0) return;

        Vector3 spawnPos = transform.position + (Vector3)_dropOffset;

        foreach (DropEntry entry in _drops)
        {
            if (entry.Prefab == null) continue;

            float roll = UnityEngine.Random.Range(0f, 100f);
            if (roll <= entry.DropChance)
                Instantiate(entry.Prefab, spawnPos, Quaternion.identity);
        }
    }
    #endregion
}
