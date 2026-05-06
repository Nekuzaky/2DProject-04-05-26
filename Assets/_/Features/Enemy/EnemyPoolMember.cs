using UnityEngine;

public class EnemyPoolMember : MonoBehaviour
{
    #region State
    private EntityHealth _health;
    private GameObject _sourcePrefab;
    #endregion

    #region Lifecycle
    private void Awake()
    {
        _health = GetComponent<EntityHealth>();
    }

    private void OnEnable()
    {
        if (_health != null)
            _health.ResetHealth();
    }
    #endregion

    #region Pool Management
    public void SetSourcePrefab(GameObject sourcePrefab)
    {
        _sourcePrefab = sourcePrefab;
    }

    public void ReturnToPool()
    {
        if (EnemyManager.Instance != null)
        {
            EnemyManager.Instance.ReturnEnemyToPool(gameObject, _sourcePrefab);
            return;
        }

        gameObject.SetActive(false);
    }
    #endregion
}
